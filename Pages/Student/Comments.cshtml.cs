using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Student
{
    [Authorize(Policy = "StudentOnly")]
    public class CommentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Comment> Comments { get; set; } = new();
        public int TotalComments { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.UserId == user.Id);

                if (student != null)
                {
                    // Get all comments for the student's proposals
                    Comments = await _context.Comments
                        .Include(c => c.User)
                        .Include(c => c.Proposal)
                        .Where(c => c.Proposal.StudentId == student.Id)
                        .OrderByDescending(c => c.CreatedAt)
                        .ToListAsync();

                    TotalComments = Comments.Count;
                }
            }
        }
    }
}
