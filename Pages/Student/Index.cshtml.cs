using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Student
{
    [Authorize(Policy = "StudentOnly")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Models.Student? StudentInfo { get; set; }
        public List<Proposal> Proposals { get; set; } = new();
        public Proposal? LatestProposal { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                StudentInfo = await _context.Students
                    .Include(s => s.Program)
                    .Include(s => s.Supervisor)
                    .ThenInclude(s => s!.User)
                    .FirstOrDefaultAsync(s => s.UserId == user.Id);

                if (StudentInfo != null)
                {
                    Proposals = await _context.Proposals
                        .Where(p => p.StudentId == StudentInfo.Id)
                        .OrderByDescending(p => p.CreatedAt)
                        .ToListAsync();

                    LatestProposal = Proposals.FirstOrDefault();
                }
            }
        }
    }
}
