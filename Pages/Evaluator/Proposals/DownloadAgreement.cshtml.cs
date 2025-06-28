using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Evaluator.Proposals
{
    [Authorize(Roles = "Evaluator")]
    public class DownloadAgreementModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public DownloadAgreementModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
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

            // Get the lecturer record for the current user (acting as evaluator)
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (lecturer == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Check if user is assigned to evaluate this proposal
            var assignment = await _context.EvaluatorAssignments
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(ea => ea.ProposalId == id && ea.EvaluatorId == lecturer.Id && ea.IsActive);

            if (assignment == null || string.IsNullOrEmpty(assignment.Proposal.AgreementFilePath))
            {
                TempData["ErrorMessage"] = "Agreement file not found or not assigned to you.";
                return RedirectToPage("/Evaluator/Proposals");
            }

            // Build the full file path
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsPath, assignment.Proposal.AgreementFilePath);

            if (!System.IO.File.Exists(filePath))
            {
                TempData["ErrorMessage"] = "Agreement file not found on server.";
                return RedirectToPage("/Evaluator/Proposals");
            }

            // Generate a user-friendly filename
            var fileExtension = Path.GetExtension(assignment.Proposal.AgreementFilePath);
            var fileName = $"{assignment.Proposal.Student.StudentId}_{assignment.Proposal.Title.Replace(" ", "_").Replace("/", "_")}_Agreement{fileExtension}";

            // Return the file for download
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}
