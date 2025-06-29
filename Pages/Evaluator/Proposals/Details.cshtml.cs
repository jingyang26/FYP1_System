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
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Proposal Proposal { get; set; } = null!;
        public EvaluatorAssignment Assignment { get; set; } = null!;
        public ProposalEvaluation? Evaluation { get; set; }

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

            // Get the evaluator assignment for this proposal
            var assignment = await _context.EvaluatorAssignments
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.User)
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.Supervisor)
                            .ThenInclude(sup => sup!.User)
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.Program)
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Comments)
                        .ThenInclude(c => c.User)
                .Include(ea => ea.Evaluator)
                    .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(ea => ea.ProposalId == id && ea.EvaluatorId == lecturer.Id && ea.IsActive);

            if (assignment == null)
            {
                TempData["ErrorMessage"] = "Proposal not found or not assigned to you.";
                return RedirectToPage("/Evaluator/Proposals");
            }

            // Get current evaluation if exists
            var evaluation = await _context.ProposalEvaluations
                .FirstOrDefaultAsync(pe => pe.ProposalId == id && pe.EvaluatorId == lecturer.Id);

            Assignment = assignment;
            Proposal = assignment.Proposal;
            Evaluation = evaluation;

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadAsync(int id)
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
                .FirstOrDefaultAsync(ea => ea.ProposalId == id && ea.EvaluatorId == lecturer.Id && ea.IsActive);

            if (assignment == null)
            {
                return NotFound("Proposal not found or not assigned to you.");
            }

            var proposal = assignment.Proposal;
            if (string.IsNullOrEmpty(proposal.FilePath))
            {
                TempData["ErrorMessage"] = "No file available for this proposal.";
                return RedirectToPage("./Details", new { id });
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", proposal.FilePath.TrimStart('/'));
            
            if (!System.IO.File.Exists(filePath))
            {
                TempData["ErrorMessage"] = "File not found.";
                return RedirectToPage("./Details", new { id });
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = $"{proposal.Student.StudentId}_{proposal.Title}_{Path.GetFileName(filePath)}";
            
            // Clean filename
            fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

            return File(fileBytes, "application/octet-stream", fileName);
        }

        public async Task<IActionResult> OnGetDownloadAgreementAsync(int id)
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
                .FirstOrDefaultAsync(ea => ea.ProposalId == id && ea.EvaluatorId == lecturer.Id && ea.IsActive);

            if (assignment == null)
            {
                return NotFound("Proposal not found or not assigned to you.");
            }

            var proposal = assignment.Proposal;
            if (string.IsNullOrEmpty(proposal.AgreementFilePath))
            {
                TempData["ErrorMessage"] = "No agreement file available for this proposal.";
                return RedirectToPage("./Details", new { id });
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", proposal.AgreementFilePath.TrimStart('/'));
            
            if (!System.IO.File.Exists(filePath))
            {
                TempData["ErrorMessage"] = "Agreement file not found.";
                return RedirectToPage("./Details", new { id });
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = $"{proposal.Student.StudentId}_Agreement_{Path.GetFileName(filePath)}";
            
            // Clean filename
            fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}
