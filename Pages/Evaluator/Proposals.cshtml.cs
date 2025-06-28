using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Evaluator
{
    [Authorize(Roles = "Evaluator")]
    public class ProposalsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProposalsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<EvaluatorAssignment> Assignments { get; set; } = new();
        public string? SelectedSession { get; set; }
        public string? SelectedStatus { get; set; }
        public string? SelectedType { get; set; }

        public async Task<IActionResult> OnGetAsync(string? session, string? status, string? type)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Get the lecturer record for the current user (acting as evaluator)
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (lecturer == null)
            {
                return RedirectToPage("/Account/Login");
            }

            SelectedSession = session;
            SelectedStatus = status;
            SelectedType = type;

            // Build query for evaluator assignments
            var query = _context.EvaluatorAssignments
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.User)
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.Supervisor)
                            .ThenInclude(sup => sup.User)
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.Program)
                .Include(ea => ea.Evaluator)
                    .ThenInclude(e => e.User)
                .Where(ea => ea.EvaluatorId == lecturer.Id);

            // Apply filters
            if (!string.IsNullOrEmpty(session))
            {
                query = query.Where(ea => ea.Proposal.Session == session);
            }

            if (!string.IsNullOrEmpty(type) && Enum.TryParse<ProposalType>(type, out var typeEnum))
            {
                query = query.Where(ea => ea.Proposal.Type == typeEnum);
            }

            // Get assignments with evaluations
            var assignments = await query
                .Select(ea => new 
                {
                    Assignment = ea,
                    Evaluation = _context.ProposalEvaluations
                        .FirstOrDefault(pe => pe.ProposalId == ea.ProposalId && pe.EvaluatorId == lecturer.Id)
                })
                .OrderByDescending(x => x.Assignment.AssignedAt)
                .ToListAsync();

            // Create assignment list with evaluations
            var assignmentList = assignments.Select(x => 
            {
                var assignment = x.Assignment;
                assignment.Evaluation = x.Evaluation;
                return assignment;
            }).ToList();

            // Apply evaluation status filter
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<EvaluationStatus>(status, out var statusEnum))
            {
                assignmentList = assignmentList.Where(a => 
                    (a.Evaluation?.Status == statusEnum) || 
                    (statusEnum == EvaluationStatus.Pending && a.Evaluation == null)
                ).ToList();
            }

            Assignments = assignmentList;
            return Page();
        }
    }
}
