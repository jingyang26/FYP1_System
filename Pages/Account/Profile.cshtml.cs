using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public ApplicationUser UserInfo { get; set; } = default!;
        public IList<string> UserRoles { get; set; } = new List<string>();
        public Models.Student? StudentInfo { get; set; }
        public Models.Lecturer? LecturerInfo { get; set; }
        public bool IsStudent { get; set; }
        public bool IsLecturer { get; set; }
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Load user with program information
            UserInfo = await _context.Users
                .Include(u => u.Program)
                .FirstOrDefaultAsync(u => u.Id == user.Id) ?? user;

            // Get user roles
            UserRoles = await _userManager.GetRolesAsync(user);

            // Check if user is a student
            IsStudent = UserRoles.Contains("Student");
            if (IsStudent)
            {
                StudentInfo = await _context.Students
                    .Include(s => s.Program)
                    .Include(s => s.Supervisor)
                        .ThenInclude(s => s!.User)
                    .FirstOrDefaultAsync(s => s.UserId == user.Id);
            }

            // Check if user is a lecturer (Supervisor, Committee, or Evaluator)
            IsLecturer = UserRoles.Any(r => r == "Supervisor" || r == "Committee" || r == "Evaluator");
            if (IsLecturer)
            {
                LecturerInfo = await _context.Lecturers
                    .Include(l => l.Program)
                    .FirstOrDefaultAsync(l => l.UserId == user.Id);
            }

            // Set message if coming from login redirect
            if (TempData.ContainsKey("LoginMessage"))
            {
                Message = TempData["LoginMessage"]?.ToString();
            }

            return Page();
        }
    }
}
