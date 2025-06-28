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

            // Get the proposal with all related data
            var proposal = await _context.Proposals
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .Include(p => p.Student)
                    .ThenInclude(s => s.Program)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.ProposalEvaluations)
                    .ThenInclude(pe => pe.Evaluator)
                        .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.Student.SupervisorId == lecturer.Id);

            if (proposal == null)
            {
                return NotFound();
            }

            Proposal = proposal;
            return Page();
        }
    }
}
