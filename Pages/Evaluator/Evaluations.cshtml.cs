using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Evaluator
{
    [Authorize(Roles = "Evaluator")]
    public class EvaluationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EvaluationsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<ProposalEvaluation> MyEvaluations { get; set; } = new();
        public string? SelectedStatus { get; set; }
        public string? SelectedSession { get; set; }

        public async Task<IActionResult> OnGetAsync(string? status, string? session)
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

            SelectedStatus = status;
            SelectedSession = session;

            // Build query for evaluations done by this evaluator
            var query = _context.ProposalEvaluations
                .Include(pe => pe.Proposal)
                    .ThenInclude(p => p!.Student)
                        .ThenInclude(s => s!.User)
                .Include(pe => pe.Proposal)
                    .ThenInclude(p => p!.Student)
                        .ThenInclude(s => s!.Program)
                .Include(pe => pe.Evaluator)
                    .ThenInclude(e => e!.User)
                .Where(pe => pe.EvaluatorId == lecturer.Id)
                .OrderByDescending(pe => pe.EvaluatedAt);

            // Apply filters
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<EvaluationStatus>(status, out var statusEnum))
                {
                    query = (IOrderedQueryable<ProposalEvaluation>)query.Where(pe => pe.Status == statusEnum);
                }
            }

            if (!string.IsNullOrEmpty(session))
            {
                query = (IOrderedQueryable<ProposalEvaluation>)query.Where(pe => pe.Proposal!.Student!.Session == session);
            }

            MyEvaluations = await query.ToListAsync();

            return Page();
        }
    }
}
