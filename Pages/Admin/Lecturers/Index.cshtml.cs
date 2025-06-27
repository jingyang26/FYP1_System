using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Admin.Lecturers
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Lecturer> Lecturers { get; set; } = default!;
        public IList<Models.Program> Programs { get; set; } = default!;

        public async Task OnGetAsync(int? programId, string? search)
        {
            // Get all programs for filter dropdown
            Programs = await _context.Programs
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            // Build lecturer query
            var lecturersQuery = _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.Program)
                .AsQueryable();

            // Apply filters
            if (programId.HasValue)
            {
                lecturersQuery = lecturersQuery.Where(l => l.ProgramId == programId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                lecturersQuery = lecturersQuery.Where(l => 
                    l.User!.FullName.Contains(search) || 
                    l.User.Email.Contains(search) ||
                    (l.Domain != null && l.Domain.Contains(search)));
            }

            Lecturers = await lecturersQuery
                .OrderBy(l => l.User!.FullName)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.CommitteeRoles)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToPage();
            }

            // Check if lecturer has committee responsibilities
            if (lecturer.CommitteeRoles?.Any() == true)
            {
                TempData["ErrorMessage"] = "Cannot delete lecturer with committee responsibilities.";
                return RedirectToPage();
            }

            try
            {
                // Remove lecturer record
                _context.Lecturers.Remove(lecturer);
                
                // Remove associated user account
                if (lecturer.User != null)
                {
                    _context.Users.Remove(lecturer.User);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Lecturer '{lecturer.User?.FullName}' has been deleted successfully.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the lecturer.";
            }

            return RedirectToPage();
        }
    }
}
