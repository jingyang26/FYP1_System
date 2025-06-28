using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;
using System.ComponentModel.DataAnnotations;

namespace FYP1System.Pages.Student
{
    [Authorize(Policy = "StudentOnly")]
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();
        public SelectList Programs { get; set; } = new(new List<Program>(), "Id", "Name");

        public class InputModel
        {
            [Required]
            [Display(Name = "Student ID")]
            [StringLength(20)]
            public string StudentId { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Program")]
            public int ProgramId { get; set; }

            [Required]
            [Display(Name = "Session")]
            [StringLength(20)]
            public string Session { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Semester")]
            public int Semester { get; set; }

            [Display(Name = "GPA")]
            [Range(0.0, 4.0, ErrorMessage = "GPA must be between 0.00 and 4.00")]
            public decimal? GPA { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            await LoadSelectListsAsync();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student != null)
            {
                Input.StudentId = student.StudentId ?? "";
                Input.ProgramId = student.ProgramId;
                Input.Session = student.Session ?? "";
                Input.Semester = student.Semester ?? 1;
                Input.GPA = student.GPA;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                // Create new student record
                student = new Models.Student
                {
                    UserId = user.Id,
                    StudentId = Input.StudentId,
                    ProgramId = Input.ProgramId,
                    Session = Input.Session,
                    Semester = Input.Semester,
                    GPA = Input.GPA,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.Students.Add(student);
            }
            else
            {
                // Update existing student record
                student.StudentId = Input.StudentId;
                student.ProgramId = Input.ProgramId;
                student.Session = Input.Session;
                student.Semester = Input.Semester;
                student.GPA = Input.GPA;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Personal details updated successfully!";
            return RedirectToPage("./Index");
        }

        private async Task LoadSelectListsAsync()
        {
            var programs = await _context.Programs
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
            Programs = new SelectList(programs, "Id", "Name");
        }
    }
}
