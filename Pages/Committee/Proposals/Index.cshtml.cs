using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Committee.Proposals
{
    [Authorize(Policy = "CommitteeOnly")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Proposal> Proposals { get; set; } = new();
        public SelectList ProgramList { get; set; } = new(new List<SelectListItem>());
        public SelectList StatusList { get; set; } = new(new List<SelectListItem>());
        public SelectList TypeList { get; set; } = new(new List<SelectListItem>());
        
        [BindProperty]
        public string? SelectedSession { get; set; }
        
        [BindProperty]
        public int? SelectedSemester { get; set; }
        
        [BindProperty]
        public int? SelectedProgramId { get; set; }
        
        [BindProperty]
        public ProposalStatus? SelectedStatus { get; set; }
        
        [BindProperty]
        public ProposalType? SelectedType { get; set; }

        public async Task OnGetAsync(string? session, int? semester, int? programId, 
            ProposalStatus? status, ProposalType? type)
        {
            SelectedSession = session;
            SelectedSemester = semester;
            SelectedProgramId = programId;
            SelectedStatus = status;
            SelectedType = type;

            await LoadDataAsync();
        }

        public IActionResult OnPostFilter()
        {
            return RedirectToPage(new { 
                session = SelectedSession, 
                semester = SelectedSemester, 
                programId = SelectedProgramId,
                status = SelectedStatus,
                type = SelectedType
            });
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int proposalId, ProposalStatus newStatus)
        {
            var proposal = await _context.Proposals.FindAsync(proposalId);
            if (proposal == null)
            {
                return NotFound();
            }

            proposal.Status = newStatus;
            proposal.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Proposal status updated to {newStatus}.";
            return RedirectToPage(new { 
                session = SelectedSession, 
                semester = SelectedSemester, 
                programId = SelectedProgramId,
                status = SelectedStatus,
                type = SelectedType
            });
        }

        private async Task LoadDataAsync()
        {
            // Load proposals with filters
            var proposalsQuery = _context.Proposals
                .Include(p => p.Student)
                .ThenInclude(s => s.User)
                .Include(p => p.Student.Program)
                .Include(p => p.Student.Supervisor)
                .ThenInclude(sup => sup!.User)
                .Include(p => p.EvaluatorAssignments)
                .ThenInclude(ea => ea.Evaluator.User)
                .Include(p => p.ProposalEvaluations)
                .Include(p => p.Comments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SelectedSession))
            {
                proposalsQuery = proposalsQuery.Where(p => p.Session == SelectedSession);
            }

            if (SelectedSemester.HasValue)
            {
                proposalsQuery = proposalsQuery.Where(p => p.Semester == SelectedSemester);
            }

            if (SelectedProgramId.HasValue)
            {
                proposalsQuery = proposalsQuery.Where(p => p.Student.ProgramId == SelectedProgramId);
            }

            if (SelectedStatus.HasValue)
            {
                proposalsQuery = proposalsQuery.Where(p => p.Status == SelectedStatus);
            }

            if (SelectedType.HasValue)
            {
                proposalsQuery = proposalsQuery.Where(p => p.Type == SelectedType);
            }

            Proposals = await proposalsQuery
                .OrderByDescending(p => p.SubmittedAt ?? p.CreatedAt)
                .ToListAsync();

            // Load filter options
            var programs = await _context.Programs.OrderBy(p => p.Name).ToListAsync();
            ProgramList = new SelectList(programs, "Id", "Name", SelectedProgramId);

            StatusList = new SelectList(Enum.GetValues<ProposalStatus>()
                .Select(s => new { Value = (int)s, Text = s.ToString() }), 
                "Value", "Text", (int?)SelectedStatus);

            TypeList = new SelectList(Enum.GetValues<ProposalType>()
                .Select(t => new { Value = (int)t, Text = t.ToString() }), 
                "Value", "Text", (int?)SelectedType);
        }
    }
}
