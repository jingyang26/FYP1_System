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
        public Models.Lecturer? LecturerInfo { get; set; }
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
            LecturerInfo = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (LecturerInfo == null)
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
                        .ThenInclude(s => s.Program)
                .Include(ea => ea.Evaluation)
                .Where(ea => ea.EvaluatorId == LecturerInfo.Id && ea.IsActive);

            // Apply filters
            if (!string.IsNullOrEmpty(session))
            {
                query = query.Where(ea => ea.Proposal.Student.Session == session);
            }

            if (!string.IsNullOrEmpty(status))
            {
                switch (status.ToLower())
                {
                    case "pending":
                        query = query.Where(ea => ea.Evaluation == null || ea.Evaluation.Status == EvaluationStatus.Pending);
                        break;
                    case "completed":
                        query = query.Where(ea => ea.Evaluation != null && 
                            (ea.Evaluation.Status == EvaluationStatus.Accepted || 
                             ea.Evaluation.Status == EvaluationStatus.AcceptedWithConditions || 
                             ea.Evaluation.Status == EvaluationStatus.Rejected));
                        break;
                    case "inprogress":
                        query = query.Where(ea => ea.Evaluation != null && ea.Evaluation.Status == EvaluationStatus.InProgress);
                        break;
                }
            }

            Assignments = await query
                .OrderByDescending(ea => ea.AssignedDate)
                .ToListAsync();

            return Page();
        }
    }
}
