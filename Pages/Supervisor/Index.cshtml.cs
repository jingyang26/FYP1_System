using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Supervisor
{
    [Authorize(Roles = "Supervisor")]
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
        public int MyStudentsCount { get; set; }
        public int TotalProposals { get; set; }
        public int PendingProposals { get; set; }
        public int ApprovedProposals { get; set; }
        public List<Models.Student> MyStudents { get; set; } = new();
        public List<Models.Proposal> RecentProposals { get; set; } = new();

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
                    // Get my students
                    MyStudents = await _context.Students
                        .Include(s => s.User)
                        .Include(s => s.Program)
                        .Where(s => s.SupervisorId == LecturerInfo.Id)
                        .ToListAsync();

                    MyStudentsCount = MyStudents.Count;

                    // Get proposals from my students
                    var studentIds = MyStudents.Select(s => s.Id).ToList();
                    
                    var allProposals = await _context.Proposals
                        .Include(p => p.Student)
                            .ThenInclude(s => s.User)
                        .Where(p => studentIds.Contains(p.StudentId))
                        .ToListAsync();

                    TotalProposals = allProposals.Count;
                    PendingProposals = allProposals.Count(p => p.Status == ProposalStatus.UnderReview || p.Status == ProposalStatus.Submitted);
                    ApprovedProposals = allProposals.Count(p => p.Status == ProposalStatus.Approved);

                    // Get recent proposals (last 5)
                    RecentProposals = allProposals
                        .OrderByDescending(p => p.SubmittedAt)
                        .Take(5)
                        .ToList();
                }
            }
        }
    }
}
