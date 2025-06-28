using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Student
{
    [Authorize(Policy = "StudentOnly")]
    public class SelectSupervisorModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SelectSupervisorModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Models.Student? Student { get; set; }
        public List<Lecturer> AvailableLecturers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            Student = await _context.Students
                .Include(s => s.Supervisor)
                .ThenInclude(s => s!.User)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (Student == null)
            {
                TempData["ErrorMessage"] = "Please complete your profile first.";
                return RedirectToPage("./Profile");
            }

            // Get available lecturers (active lecturers who can supervise)
            AvailableLecturers = await _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.Students)
                .Where(l => l.IsActive && l.CanSupervise)
                .OrderBy(l => l.User.FullName)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int lecturerId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Student profile not found.";
                return RedirectToPage("./Profile");
            }

            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == lecturerId && l.IsActive && l.CanSupervise);

            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Selected supervisor is not available.";
                return RedirectToPage();
            }

            // Update student's supervisor
            student.SupervisorId = lecturerId;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully selected {lecturer.User.FullName} as your preferred supervisor. Your selection is pending committee approval.";
            
            return RedirectToPage("./Index");
        }
    }
}
