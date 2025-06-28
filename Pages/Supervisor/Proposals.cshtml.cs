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
    public class ProposalsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProposalsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Proposal> Proposals { get; set; } = new();
        public string? SelectedSession { get; set; }
        public int? SelectedSemester { get; set; }
        public string? SelectedStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string? session, int? semester, string? status)
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
            SelectedStatus = status;

            // Build query for proposals from students supervised by this lecturer
            var query = _context.Proposals
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .Include(p => p.Student)
                    .ThenInclude(s => s.Program)
                .Include(p => p.Comments)
                .Include(p => p.ProposalEvaluations)
                .Where(p => p.Student.SupervisorId == lecturer.Id);

            // Apply filters
            if (!string.IsNullOrEmpty(session))
            {
                query = query.Where(p => p.Session == session);
            }

            if (semester.HasValue)
            {
                query = query.Where(p => p.Semester == semester);
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProposalStatus>(status, out var statusEnum))
            {
                query = query.Where(p => p.Status == statusEnum);
            }

            Proposals = await query
                .OrderByDescending(p => p.SubmittedAt ?? p.CreatedAt)
                .ToListAsync();

            return Page();
        }
    }
}
