using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Committee.LecturerDomains
{
    [Authorize(Policy = "CommitteeOnly")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Lecturer> Lecturers { get; set; } = new();

        public async Task OnGetAsync()
        {
            Lecturers = await _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.Program)
                .Where(l => l.IsActive)
                .OrderBy(l => l.User.FullName)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAssignDomainAsync(int lecturerId, string domain)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == lecturerId);
                
            if (lecturer == null)
            {
                return NotFound();
            }

            lecturer.Domain = domain;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Domain '{domain}' assigned to {lecturer.User?.FullName} successfully.";
            return RedirectToPage();
        }
    }
}
