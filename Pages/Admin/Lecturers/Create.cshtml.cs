using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Admin.Lecturers
{
    [Authorize(Policy = "AdminOnly")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public SelectList ProgramSelectList { get; set; } = default!;

        public class InputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            [StringLength(100)]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "Phone Number")]
            [StringLength(20)]
            public string? PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Program")]
            public int ProgramId { get; set; }

            [Display(Name = "Domain/Specialization")]
            [StringLength(200)]
            public string? Domain { get; set; }

            [Display(Name = "Office Location")]
            [StringLength(100)]
            public string? OfficeLocation { get; set; }

            [Display(Name = "Is Committee Member")]
            public bool IsCommittee { get; set; } = false;

            [Display(Name = "Is Active")]
            public bool IsActive { get; set; } = true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSelectListsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Input.Email", "A user with this email already exists.");
                await LoadSelectListsAsync();
                return Page();
            }

            try
            {
                // Create new user account
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FullName = Input.FullName,
                    PhoneNumber = Input.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, "DefaultPassword123!");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await LoadSelectListsAsync();
                    return Page();
                }

                // Add user to Supervisor role
                await _userManager.AddToRoleAsync(user, "Supervisor");

                // Create lecturer record
                var lecturer = new Lecturer
                {
                    UserId = user.Id,
                    ProgramId = Input.ProgramId,
                    Domain = Input.Domain,
                    OfficeLocation = Input.OfficeLocation,
                    IsCommittee = Input.IsCommittee,
                    IsActive = Input.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Lecturers.Add(lecturer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Lecturer '{Input.FullName}' has been created successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the lecturer.";
                await LoadSelectListsAsync();
                return Page();
            }
        }

        private async Task LoadSelectListsAsync()
        {
            var programs = await _context.Programs
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            ProgramSelectList = new SelectList(programs, "Id", "Name");
        }
    }
}
