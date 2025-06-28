using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Student.Proposals
{
    [Authorize(Policy = "StudentOnly")]
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
        public List<ProposalEvaluation> Evaluations { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null) return NotFound();

            Proposal = await _context.Proposals
                .Include(p => p.Student)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == student.Id);

            if (Proposal == null) return NotFound();

            // Get evaluations
            Evaluations = await _context.ProposalEvaluations
                .Include(pe => pe.Evaluator)
                .ThenInclude(e => e.User)
                .Where(pe => pe.ProposalId == id)
                .OrderByDescending(pe => pe.EvaluatedAt)
                .ToListAsync();

            // Get comments
            Comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ProposalId == id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Page();
        }
    }
}
