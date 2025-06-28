using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Supervisor
{
    [Authorize(Roles = "Supervisor")]
    public class ReportsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int TotalStudents { get; set; }
        public int TotalProposals { get; set; }
        public int ApprovedProposals { get; set; }
        public int RejectedProposals { get; set; }
        public int PendingProposals { get; set; }
        public int TotalComments { get; set; }

        public List<ProposalStatusSummary> ProposalsByStatus { get; set; } = new();
        public List<SessionSummary> ProposalsBySession { get; set; } = new();
        public List<Models.Student> RecentStudents { get; set; } = new();
        public List<Proposal> RecentProposals { get; set; } = new();

        public string? SelectedSession { get; set; }
        public int? SelectedSemester { get; set; }

        public async Task<IActionResult> OnGetAsync(string? session, int? semester)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Get the lecturer record for the current user
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (lecturer == null)
            {
                return RedirectToPage("/Account/Login");
            }

            SelectedSession = session;
            SelectedSemester = semester;

            // Build base queries for students and proposals supervised by this lecturer
            var studentsQuery = _context.Students
                .Where(s => s.SupervisorId == lecturer.Id);

            var proposalsQuery = _context.Proposals
                .Where(p => p.Student!.SupervisorId == lecturer.Id);

            // Apply filters if specified
            if (!string.IsNullOrEmpty(session))
            {
                studentsQuery = studentsQuery.Where(s => s.Session == session);
                proposalsQuery = proposalsQuery.Where(p => p.Student!.Session == session);
            }

            if (semester.HasValue)
            {
                studentsQuery = studentsQuery.Where(s => s.Semester == semester.Value);
                proposalsQuery = proposalsQuery.Where(p => p.Student!.Semester == semester.Value);
            }

            // Calculate statistics
            TotalStudents = await studentsQuery.CountAsync();
            TotalProposals = await proposalsQuery.CountAsync();
            ApprovedProposals = await proposalsQuery.CountAsync(p => p.Status == ProposalStatus.Approved);
            RejectedProposals = await proposalsQuery.CountAsync(p => p.Status == ProposalStatus.Rejected);
            PendingProposals = await proposalsQuery.CountAsync(p => p.Status != ProposalStatus.Approved && p.Status != ProposalStatus.Rejected);
            
            TotalComments = await _context.Comments
                .CountAsync(c => c.UserId == user.Id);

            // Proposals by status
            ProposalsByStatus = await proposalsQuery
                .GroupBy(p => p.Status)
                .Select(g => new ProposalStatusSummary
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            // Proposals by session
            ProposalsBySession = await proposalsQuery
                .GroupBy(p => p.Student!.Session)
                .Select(g => new SessionSummary
                {
                    Session = g.Key,
                    Count = g.Count(),
                    ApprovedCount = g.Count(p => p.Status == ProposalStatus.Approved),
                    RejectedCount = g.Count(p => p.Status == ProposalStatus.Rejected)
                })
                .OrderBy(x => x.Session)
                .ToListAsync();

            // Recent students (last 10)
            RecentStudents = await studentsQuery
                .Include(s => s.User)
                .Include(s => s.Program)
                .OrderByDescending(s => s.Id)
                .Take(10)
                .ToListAsync();

            // Recent proposals (last 10)
            RecentProposals = await proposalsQuery
                .Include(p => p.Student)
                    .ThenInclude(s => s!.User)
                .OrderByDescending(p => p.SubmittedAt ?? p.CreatedAt)
                .Take(10)
                .ToListAsync();

            return Page();
        }
    }

    public class ProposalStatusSummary
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class SessionSummary
    {
        public string Session { get; set; } = string.Empty;
        public int Count { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
    }
}
