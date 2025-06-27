using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Admin.Committee
{
    [Authorize(Policy = "AdminOnly")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public SelectList ProgramSelectList { get; set; } = default!;
        public SelectList LecturerSelectList { get; set; } = default!;

        public class InputModel
        {
            [Required]
            [Display(Name = "Program")]
            public int ProgramId { get; set; }

            [Required]
            [Display(Name = "Lecturer")]
            public int LecturerId { get; set; }

            [Display(Name = "Committee Role")]
            [StringLength(100)]
            public string? Role { get; set; }

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

            // Check if lecturer is already a committee member for this program
            var existingCommittee = await _context.CommitteeMembers
                .FirstOrDefaultAsync(c => c.LecturerId == Input.LecturerId && 
                                         c.ProgramId == Input.ProgramId && 
                                         c.IsActive);

            if (existingCommittee != null)
            {
                ModelState.AddModelError("Input.LecturerId", "This lecturer is already a committee member for the selected program.");
                await LoadSelectListsAsync();
                return Page();
            }

            // Check if the lecturer belongs to the selected program
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == Input.LecturerId && l.ProgramId == Input.ProgramId);

            if (lecturer == null)
            {
                ModelState.AddModelError("Input.LecturerId", "The selected lecturer does not belong to the selected program.");
                await LoadSelectListsAsync();
                return Page();
            }

            try
            {
                // Create committee member
                var committee = new CommitteeMember
                {
                    LecturerId = Input.LecturerId,
                    ProgramId = Input.ProgramId,
                    Role = Input.Role,
                    IsActive = Input.IsActive,
                    AssignedDate = DateTime.UtcNow
                };

                _context.CommitteeMembers.Add(committee);

                // Update lecturer's committee status
                lecturer.IsCommittee = true;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Committee member has been added successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the committee member.";
                await LoadSelectListsAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnGetLecturersAsync(int programId)
        {
            var lecturers = await _context.Lecturers
                .Include(l => l.User)
                .Where(l => l.ProgramId == programId && l.IsActive)
                .Select(l => new { Value = l.Id, Text = l.User!.FullName })
                .ToListAsync();

            return new JsonResult(lecturers);
        }

        private async Task LoadSelectListsAsync()
        {
            var programs = await _context.Programs
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            ProgramSelectList = new SelectList(programs, "Id", "Name");
            LecturerSelectList = new SelectList(new List<SelectListItem>(), "Value", "Text");
        }
    }
}
