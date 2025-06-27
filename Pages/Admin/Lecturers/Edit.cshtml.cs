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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public SelectList ProgramSelectList { get; set; } = default!;

        public class InputModel
        {
            public int Id { get; set; }
            public string UserId { get; set; } = string.Empty;

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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.Program)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lecturer == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = lecturer.Id,
                UserId = lecturer.UserId,
                FullName = lecturer.User!.FullName,
                Email = lecturer.User.Email!,
                PhoneNumber = lecturer.User.PhoneNumber,
                ProgramId = lecturer.ProgramId,
                Domain = lecturer.Domain,
                OfficeLocation = lecturer.OfficeLocation,
                IsCommittee = lecturer.IsCommittee,
                IsActive = lecturer.IsActive
            };

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

            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == Input.Id);

            if (lecturer == null)
            {
                return NotFound();
            }

            try
            {
                // Update user information
                lecturer.User!.FullName = Input.FullName;
                lecturer.User.Email = Input.Email;
                lecturer.User.UserName = Input.Email;
                lecturer.User.PhoneNumber = Input.PhoneNumber;

                // Update lecturer information
                lecturer.ProgramId = Input.ProgramId;
                lecturer.Domain = Input.Domain;
                lecturer.OfficeLocation = Input.OfficeLocation;
                lecturer.IsCommittee = Input.IsCommittee;
                lecturer.IsActive = Input.IsActive;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Lecturer '{Input.FullName}' has been updated successfully.";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LecturerExists(Input.Id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the lecturer.";
                await LoadSelectListsAsync();
                return Page();
            }
        }

        private bool LecturerExists(int id)
        {
            return _context.Lecturers.Any(e => e.Id == id);
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
