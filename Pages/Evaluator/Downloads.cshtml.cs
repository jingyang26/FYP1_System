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
    public class DownloadsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DownloadsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Proposal> AvailableDownloads { get; set; } = new();
        public string? SelectedSession { get; set; }
        public string? SelectedStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string? session, string? status)
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
            SelectedStatus = status;

            // Get proposals assigned to this evaluator that have files to download
            var query = _context.Proposals
                .Include(p => p.Student)
                    .ThenInclude(s => s!.User)
                .Include(p => p.Student)
                    .ThenInclude(s => s!.Program)
                .Include(p => p.EvaluatorAssignments)
                .Where(p => p.EvaluatorAssignments.Any(ea => ea.EvaluatorId == lecturer.Id))
                .Where(p => !string.IsNullOrEmpty(p.FilePath) || !string.IsNullOrEmpty(p.AgreementFilePath))
                .OrderByDescending(p => p.SubmittedAt ?? p.CreatedAt);

            // Apply filters
            if (!string.IsNullOrEmpty(session))
            {
                query = (IOrderedQueryable<Proposal>)query.Where(p => p.Student!.Session == session);
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<ProposalStatus>(status, out var statusEnum))
                {
                    query = (IOrderedQueryable<Proposal>)query.Where(p => p.Status == statusEnum);
                }
            }

            AvailableDownloads = await query.ToListAsync();

            return Page();
        }
    }
}
