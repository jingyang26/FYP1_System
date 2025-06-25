using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Committee
{
    [Authorize(Policy = "CommitteeOnly")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalStudents { get; set; }
        public int TotalProposals { get; set; }
        public int PendingProposals { get; set; }
        public int ApprovedProposals { get; set; }
        public List<Proposal> RecentProposals { get; set; } = new();

        public async Task OnGetAsync()
        {
            TotalStudents = await _context.Students.CountAsync();
            TotalProposals = await _context.Proposals.CountAsync();
            PendingProposals = await _context.Proposals
                .CountAsync(p => p.Status == ProposalStatus.Submitted || p.Status == ProposalStatus.UnderReview);
            ApprovedProposals = await _context.Proposals
                .CountAsync(p => p.Status == ProposalStatus.Approved);

            RecentProposals = await _context.Proposals
                .Include(p => p.Student)
                .ThenInclude(s => s.User)
                .OrderByDescending(p => p.SubmittedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}
