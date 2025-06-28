using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Committee.Evaluators
{
    [Authorize(Policy = "CommitteeOnly")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Proposal> ProposalsNeedingEvaluators { get; set; } = new();
        public List<Lecturer> AvailableEvaluators { get; set; } = new();
        public SelectList ProposalTypeList { get; set; } = new(new List<SelectListItem>());
        
        [BindProperty]
        public ProposalType? SelectedProposalType { get; set; }

        public async Task OnGetAsync(ProposalType? proposalType)
        {
            SelectedProposalType = proposalType;
            await LoadDataAsync();
        }

        public IActionResult OnPostFilter()
        {
            return RedirectToPage(new { proposalType = SelectedProposalType });
        }

        public async Task<IActionResult> OnPostAssignEvaluatorAsync(int proposalId, int evaluatorId)
        {
            var proposal = await _context.Proposals
                .Include(p => p.Student)
                .ThenInclude(s => s.Supervisor)
                .Include(p => p.EvaluatorAssignments)
                .FirstOrDefaultAsync(p => p.Id == proposalId);

            if (proposal == null)
            {
                TempData["ErrorMessage"] = "Proposal not found.";
                return RedirectToPage();
            }

            var evaluator = await _context.Lecturers.FindAsync(evaluatorId);
            if (evaluator == null)
            {
                TempData["ErrorMessage"] = "Evaluator not found.";
                return RedirectToPage();
            }

            // Check if evaluator is the supervisor
            if (proposal.Student.SupervisorId == evaluatorId)
            {
                TempData["ErrorMessage"] = "Cannot assign supervisor as evaluator for their own student's proposal.";
                return RedirectToPage(new { proposalType = SelectedProposalType });
            }

            // Check if already assigned
            if (proposal.EvaluatorAssignments.Any(ea => ea.EvaluatorId == evaluatorId && ea.IsActive))
            {
                TempData["ErrorMessage"] = "This evaluator is already assigned to this proposal.";
                return RedirectToPage(new { proposalType = SelectedProposalType });
            }

            // Check if proposal already has 2 evaluators
            if (proposal.EvaluatorAssignments.Count(ea => ea.IsActive) >= 2)
            {
                TempData["ErrorMessage"] = "This proposal already has 2 evaluators assigned.";
                return RedirectToPage(new { proposalType = SelectedProposalType });
            }

            // Create new evaluator assignment
            var assignment = new EvaluatorAssignment
            {
                ProposalId = proposalId,
                EvaluatorId = evaluatorId,
                AssignedDate = DateTime.UtcNow,
                AssignedBy = User.Identity?.Name ?? "Committee",
                IsActive = true
            };

            _context.EvaluatorAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Evaluator assigned successfully to proposal.";
            return RedirectToPage(new { proposalType = SelectedProposalType });
        }

        public async Task<IActionResult> OnPostRemoveEvaluatorAsync(int assignmentId)
        {
            var assignment = await _context.EvaluatorAssignments.FindAsync(assignmentId);
            if (assignment == null)
            {
                TempData["ErrorMessage"] = "Assignment not found.";
                return RedirectToPage();
            }

            assignment.IsActive = false;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Evaluator removed successfully.";
            return RedirectToPage(new { proposalType = SelectedProposalType });
        }

        public async Task<IActionResult> OnPostAutoAssignEvaluatorsAsync(int proposalId)
        {
            var proposal = await _context.Proposals
                .Include(p => p.Student)
                .ThenInclude(s => s.Supervisor)
                .Include(p => p.EvaluatorAssignments)
                .FirstOrDefaultAsync(p => p.Id == proposalId);

            if (proposal == null)
            {
                TempData["ErrorMessage"] = "Proposal not found.";
                return RedirectToPage();
            }

            // Get evaluators based on proposal type and exclude supervisor
            var availableEvaluators = await _context.Lecturers
                .Where(l => l.IsActive && 
                           l.Id != proposal.Student.SupervisorId &&
                           (proposal.Type == ProposalType.Research ? l.Domain == "Research" : l.Domain == "Development"))
                .Where(l => !proposal.EvaluatorAssignments.Any(ea => ea.EvaluatorId == l.Id && ea.IsActive))
                .Take(2 - proposal.EvaluatorAssignments.Count(ea => ea.IsActive))
                .ToListAsync();

            foreach (var evaluator in availableEvaluators)
            {
                var assignment = new EvaluatorAssignment
                {
                    ProposalId = proposalId,
                    EvaluatorId = evaluator.Id,
                    AssignedDate = DateTime.UtcNow,
                    AssignedBy = User.Identity?.Name ?? "Committee (Auto)",
                    IsActive = true
                };

                _context.EvaluatorAssignments.Add(assignment);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{availableEvaluators.Count} evaluator(s) auto-assigned successfully.";
            return RedirectToPage(new { proposalType = SelectedProposalType });
        }

        private async Task LoadDataAsync()
        {
            // Load proposals that need evaluators
            var proposalsQuery = _context.Proposals
                .Include(p => p.Student)
                .ThenInclude(s => s.User)
                .Include(p => p.Student.Program)
                .Include(p => p.Student.Supervisor)
                .ThenInclude(sup => sup!.User)
                .Include(p => p.EvaluatorAssignments)
                .ThenInclude(ea => ea.Evaluator.User)
                .Where(p => p.Status == ProposalStatus.Submitted || p.Status == ProposalStatus.UnderReview)
                .AsQueryable();

            if (SelectedProposalType.HasValue)
            {
                proposalsQuery = proposalsQuery.Where(p => p.Type == SelectedProposalType);
            }

            ProposalsNeedingEvaluators = await proposalsQuery
                .OrderByDescending(p => p.SubmittedAt)
                .ToListAsync();

            // Load available evaluators
            AvailableEvaluators = await _context.Lecturers
                .Include(l => l.User)
                .Where(l => l.IsActive)
                .OrderBy(l => l.User.FullName)
                .ToListAsync();

            // Load filter options
            ProposalTypeList = new SelectList(Enum.GetValues<ProposalType>()
                .Select(t => new { Value = (int)t, Text = t.ToString() }), 
                "Value", "Text", (int?)SelectedProposalType);
        }
    }
}
