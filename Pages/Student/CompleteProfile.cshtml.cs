using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;
using System.ComponentModel.DataAnnotations;

namespace FYP1System.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class CompleteProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CompleteProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public StudentProfileInput Input { get; set; } = new();

        public SelectList ProgramList { get; set; } = new(new List<SelectListItem>());

        public class StudentProfileInput
        {
            [Required]
            [Display(Name = "Student ID")]
            [StringLength(20, ErrorMessage = "Student ID cannot exceed 20 characters")]
            public string StudentId { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Program")]
            public int ProgramId { get; set; }

            [Required]
            [Display(Name = "Session")]
            [StringLength(20, ErrorMessage = "Session cannot exceed 20 characters")]
            [RegularExpression(@"^\d{4}/\d{4}$", ErrorMessage = "Session must be in format YYYY/YYYY (e.g., 2024/2025)")]
            public string Session { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Semester")]
            [Range(1, 3, ErrorMessage = "Semester must be between 1 and 3")]
            public int Semester { get; set; }

            [Display(Name = "GPA")]
            [Range(0.00, 4.00, ErrorMessage = "GPA must be between 0.00 and 4.00")]
            public decimal? GPA { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            // Check if student profile already exists
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.User.Email == userId);

            if (existingStudent != null)
            {
                // Profile already exists, redirect to student dashboard
                return RedirectToPage("/Student/Index");
            }

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            // Get the user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                await LoadDataAsync();
                return Page();
            }

            // Check if student ID is already taken
            var existingStudentId = await _context.Students
                .AnyAsync(s => s.StudentId == Input.StudentId);

            if (existingStudentId)
            {
                ModelState.AddModelError("Input.StudentId", "This Student ID is already taken.");
                await LoadDataAsync();
                return Page();
            }

            // Check if student profile already exists for this user
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (existingStudent != null)
            {
                ModelState.AddModelError("", "Student profile already exists for this user.");
                await LoadDataAsync();
                return Page();
            }

            // Create student profile
            var student = new Models.Student
            {
                UserId = user.Id,
                ProgramId = Input.ProgramId,
                StudentId = Input.StudentId,
                Session = Input.Session,
                Semester = Input.Semester,
                GPA = Input.GPA,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Student profile completed successfully!";
            return RedirectToPage("/Student/Index");
        }

        private async Task LoadDataAsync()
        {
            var programs = await _context.Programs
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            ProgramList = new SelectList(programs, "Id", "Name");
        }
    }
}
