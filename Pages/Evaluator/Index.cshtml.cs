using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Evaluator
{
    [Authorize(Roles = "Evaluator")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Models.Lecturer? LecturerInfo { get; set; }
        public int AssignedProposals { get; set; }
        public int CompletedEvaluations { get; set; }
        public int PendingEvaluations { get; set; }
        public List<EvaluatorAssignment> MyAssignments { get; set; } = new();
        public List<Models.Proposal> RecentAssignments { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get lecturer information
                LecturerInfo = await _context.Lecturers
                    .Include(l => l.Program)
                    .FirstOrDefaultAsync(l => l.UserId == user.Id);

                if (LecturerInfo != null)
                {
                    // Get my evaluator assignments
                    MyAssignments = await _context.EvaluatorAssignments
                        .Include(ea => ea.Proposal)
                            .ThenInclude(p => p.Student)
                                .ThenInclude(s => s.User)
                        .Include(ea => ea.Proposal)
                            .ThenInclude(p => p.Student)
                                .ThenInclude(s => s.Program)
                        .Where(ea => ea.EvaluatorId == LecturerInfo.Id)
                        .ToListAsync();

                    AssignedProposals = MyAssignments.Count;
                    
                    // Check for completed evaluations
                    CompletedEvaluations = await _context.ProposalEvaluations
                        .Where(pe => pe.EvaluatorId == LecturerInfo.Id && pe.Status == EvaluationStatus.Completed)
                        .CountAsync();

                    PendingEvaluations = AssignedProposals - CompletedEvaluations;

                    // Get recent assignments (last 5)
                    RecentAssignments = MyAssignments
                        .OrderByDescending(ea => ea.AssignedDate)
                        .Take(5)
                        .Select(ea => ea.Proposal)
                        .ToList();
                }
            }
        }
    }
}
