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
    public class CommentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Comment> MyComments { get; set; } = new();
        public string? SelectedProposalTitle { get; set; }
        public string? SelectedStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string? proposalTitle, string? status)
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

            SelectedProposalTitle = proposalTitle;
            SelectedStatus = status;

            // Build query for comments made by this supervisor
            var query = _context.Comments
                .Include(c => c.User)
                .Include(c => c.Proposal)
                    .ThenInclude(p => p!.Student)
                        .ThenInclude(s => s!.User)
                .Include(c => c.Proposal)
                    .ThenInclude(p => p!.Student)
                        .ThenInclude(s => s!.Program)
                .Where(c => c.UserId == user.Id)
                .OrderByDescending(c => c.CreatedAt);

            // Apply filters
            if (!string.IsNullOrEmpty(proposalTitle))
            {
                query = (IOrderedQueryable<Comment>)query.Where(c => c.Proposal!.Title.Contains(proposalTitle));
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<ProposalStatus>(status, out var statusEnum))
                {
                    query = (IOrderedQueryable<Comment>)query.Where(c => c.Proposal!.Status == statusEnum);
                }
            }

            MyComments = await query.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int commentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == user.Id);

            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Comment deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Comment not found or you don't have permission to delete it.";
            }

            return RedirectToPage();
        }
    }
}
