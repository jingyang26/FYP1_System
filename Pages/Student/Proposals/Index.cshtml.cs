using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Student.Proposals
{
    [Authorize(Policy = "StudentOnly")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Proposal> Proposals { get; set; } = new();
        public bool CanSubmitNewProposal { get; set; }
        public bool CanResubmit { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.UserId == user.Id);

                if (student != null)
                {
                    Proposals = await _context.Proposals
                        .Where(p => p.StudentId == student.Id)
                        .OrderByDescending(p => p.CreatedAt)
                        .ToListAsync();

                    // Determine if student can submit new proposal
                    var latestProposal = Proposals.FirstOrDefault();
                    CanSubmitNewProposal = latestProposal == null || 
                                         latestProposal.Status == ProposalStatus.Rejected;

                    // Determine if student can resubmit
                    CanResubmit = latestProposal?.Status == ProposalStatus.Rejected ||
                                latestProposal?.Status == ProposalStatus.NeedsRevision;
                }
            }
        }
    }
}
