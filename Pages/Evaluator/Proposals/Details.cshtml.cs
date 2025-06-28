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
        public ProposalEvaluation? CurrentEvaluation { get; set; }

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
                            .ThenInclude(sup => sup.User)
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
            CurrentEvaluation = evaluation;

            return Page();
        }
    }
}
