using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Admin.Lecturers
{
    [Authorize(Policy = "AdminOnly")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Lecturer Lecturer { get; set; } = default!;
        public IList<CommitteeMember> CommitteeRoles { get; set; } = default!;
        public int SupervisedStudentsCount { get; set; }
        public int CommitteeRolesCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.Program)
                .Include(l => l.SupervisedStudents)
                .Include(l => l.CommitteeRoles)
                    .ThenInclude(c => c.Program)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lecturer == null)
            {
                return NotFound();
            }

            Lecturer = lecturer;
            CommitteeRoles = lecturer.CommitteeRoles.Where(c => c.IsActive).ToList();
            SupervisedStudentsCount = lecturer.SupervisedStudents.Count(s => s.IsActive);
            CommitteeRolesCount = CommitteeRoles.Count;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.CommitteeRoles)
                .Include(l => l.SupervisedStudents)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToPage("./Index");
            }

            try
            {
                // Remove committee roles
                if (lecturer.CommitteeRoles?.Any() == true)
                {
                    _context.CommitteeMembers.RemoveRange(lecturer.CommitteeRoles);
                }

                // Remove lecturer record
                _context.Lecturers.Remove(lecturer);
                
                // Remove associated user account
                if (lecturer.User != null)
                {
                    _context.Users.Remove(lecturer.User);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Lecturer '{lecturer.User?.FullName}' has been deleted successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the lecturer.";
                return RedirectToPage("./Index");
            }
        }
    }
}
