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
    public class StudentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Models.Student> MyStudents { get; set; } = new();
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

            // Build query for students supervised by this lecturer
            var query = _context.Students
                .Include(s => s.User)
                .Include(s => s.Program)
                .Include(s => s.Proposals)
                .Where(s => s.SupervisorId == lecturer.Id);

            // Note: We don't have session/semester on Student directly,
            // so we'll filter by their proposals if needed
            if (!string.IsNullOrEmpty(session) || semester.HasValue)
            {
                query = query.Where(s => s.Proposals.Any(p => 
                    (string.IsNullOrEmpty(session) || p.Session == session) &&
                    (!semester.HasValue || p.Semester == semester)));
            }

            MyStudents = await query
                .OrderBy(s => s.User.FullName)
                .ToListAsync();

            return Page();
        }
    }
}
