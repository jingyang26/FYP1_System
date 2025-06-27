using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;
using System.ComponentModel.DataAnnotations;

namespace FYP1System.Pages.Admin.Committee
{
    [Authorize(Policy = "AdminOnly")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string LecturerName { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public SelectList ProgramsList { get; set; } = default!;

        public class InputModel
        {
            public int Id { get; set; }

            [Display(Name = "Program")]
            public int ProgramId { get; set; }

            [Display(Name = "Role")]
            [StringLength(100)]
            public string? Role { get; set; }

            [Display(Name = "Is Active")]
            public bool IsActive { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committeeMember = await _context.CommitteeMembers
                .Include(c => c.Lecturer)
                .ThenInclude(l => l.User)
                .Include(c => c.Program)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (committeeMember == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = committeeMember.Id,
                ProgramId = committeeMember.ProgramId,
                Role = committeeMember.Role,
                IsActive = committeeMember.IsActive
            };

            LecturerName = committeeMember.Lecturer.User.FullName;
            ProgramName = committeeMember.Program.Name;
            AssignedDate = committeeMember.AssignedDate;

            await LoadSelectLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectLists();
                return Page();
            }

            var committeeMember = await _context.CommitteeMembers
                .Include(c => c.Lecturer)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(c => c.Id == Input.Id);

            if (committeeMember == null)
            {
                return NotFound();
            }

            // Check if another committee member exists for the same lecturer and program
            var existingMember = await _context.CommitteeMembers
                .Where(c => c.LecturerId == committeeMember.LecturerId && 
                           c.ProgramId == Input.ProgramId && 
                           c.Id != Input.Id)
                .FirstOrDefaultAsync();

            if (existingMember != null)
            {
                ModelState.AddModelError("Input.ProgramId", "This lecturer is already a committee member for the selected program.");
                await LoadSelectLists();
                return Page();
            }

            committeeMember.ProgramId = Input.ProgramId;
            committeeMember.Role = Input.Role;
            committeeMember.IsActive = Input.IsActive;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Committee member updated successfully.";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommitteeMemberExists(Input.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CommitteeMemberExists(int id)
        {
            return _context.CommitteeMembers.Any(e => e.Id == id);
        }

        private async Task LoadSelectLists()
        {
            var programs = await _context.Programs
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            ProgramsList = new SelectList(programs, "Id", "Name");

            if (!string.IsNullOrEmpty(LecturerName))
            {
                var committeeMember = await _context.CommitteeMembers
                    .Include(c => c.Lecturer)
                    .ThenInclude(l => l.User)
                    .FirstOrDefaultAsync(c => c.Id == Input.Id);

                if (committeeMember != null)
                {
                    LecturerName = committeeMember.Lecturer.User.FullName;
                }
            }
        }
    }
}
