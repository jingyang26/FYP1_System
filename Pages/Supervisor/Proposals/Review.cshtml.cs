using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;
using System.ComponentModel.DataAnnotations;

namespace FYP1System.Pages.Supervisor.Proposals
{
    [Authorize(Roles = "Supervisor")]
    public class ReviewModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Proposal Proposal { get; set; } = null!;
        public int ProposalId { get; set; }

        [BindProperty]
        public CommentInputModel Comment { get; set; } = new();

        public class CommentInputModel
        {
            [Required]
            [StringLength(2000, MinimumLength = 10)]
            public string Content { get; set; } = string.Empty;

            [Display(Name = "Private Comment")]
            public bool IsPrivate { get; set; } = false;
        }

        public async Task<IActionResult> OnGetAsync(int id)
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

            // Get the proposal with all related data
            var proposal = await _context.Proposals
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .Include(p => p.Student)
                    .ThenInclude(s => s.Program)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.Student.SupervisorId == lecturer.Id);

            if (proposal == null)
            {
                return NotFound();
            }

            // Check if proposal is in a state that allows supervisor comments
            if (proposal.Status != ProposalStatus.Submitted && proposal.Status != ProposalStatus.Resubmitted)
            {
                TempData["ErrorMessage"] = "Comments can only be added to submitted proposals.";
                return RedirectToPage("./Details", new { id });
            }

            Proposal = proposal;
            ProposalId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
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

            // Get the proposal
            var proposal = await _context.Proposals
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .Include(p => p.Student)
                    .ThenInclude(s => s.Program)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.Student.SupervisorId == lecturer.Id);

            if (proposal == null)
            {
                return NotFound();
            }

            Proposal = proposal;
            ProposalId = id;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Create the comment
            var comment = new Comment
            {
                ProposalId = id,
                UserId = user.Id,
                Content = Comment.Content,
                IsPrivate = Comment.IsPrivate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);

            // Update proposal status to indicate supervisor has reviewed
            if (proposal.Status == ProposalStatus.Submitted || proposal.Status == ProposalStatus.Resubmitted)
            {
                proposal.Status = ProposalStatus.UnderReview;
                proposal.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your comment has been added successfully.";
            return RedirectToPage("./Details", new { id });
        }
    }
}
