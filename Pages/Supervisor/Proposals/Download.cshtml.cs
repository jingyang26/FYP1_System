using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Supervisor.Proposals
{
    [Authorize(Roles = "Supervisor")]
    public class DownloadModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public DownloadModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Get the lecturer record for the current user
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (lecturer == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Get the proposal
            var proposal = await _context.Proposals
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.Student.SupervisorId == lecturer.Id);

            if (proposal == null || string.IsNullOrEmpty(proposal.FilePath))
            {
                TempData["ErrorMessage"] = "Proposal file not found.";
                return RedirectToPage("/Supervisor/Proposals");
            }

            // Build the full file path
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsPath, proposal.FilePath);

            if (!System.IO.File.Exists(filePath))
            {
                TempData["ErrorMessage"] = "Proposal file not found on server.";
                return RedirectToPage("/Supervisor/Proposals");
            }

            // Generate a user-friendly filename
            var fileExtension = Path.GetExtension(proposal.FilePath);
            var fileName = $"{proposal.Student.StudentId}_{proposal.Title.Replace(" ", "_").Replace("/", "_")}_Proposal{fileExtension}";

            // Return the file for download
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}
