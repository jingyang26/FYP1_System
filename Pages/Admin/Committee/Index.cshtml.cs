using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Admin.Committee
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Program> Programs { get; set; } = default!;
        public IDictionary<string, IList<CommitteeMember>> CommitteesByProgram { get; set; } = new Dictionary<string, IList<CommitteeMember>>();

        public async Task OnGetAsync(int? programId, string? role)
        {
            // Get all active programs
            Programs = await _context.Programs
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            // Build committee query
            var committeeQuery = _context.CommitteeMembers
                .Include(c => c.Lecturer)
                    .ThenInclude(l => l!.User)
                .Include(c => c.Program)
                .Where(c => c.IsActive)
                .AsQueryable();

            // Apply filters
            if (programId.HasValue)
            {
                committeeQuery = committeeQuery.Where(c => c.ProgramId == programId.Value);
            }

            if (!string.IsNullOrEmpty(role))
            {
                committeeQuery = committeeQuery.Where(c => c.Role == role);
            }

            var committees = await committeeQuery
                .OrderBy(c => c.Program!.Name)
                .ThenBy(c => c.Lecturer!.User!.FullName)
                .ToListAsync();

            // Group by program
            CommitteesByProgram = committees
                .GroupBy(c => c.Program!.Name)
                .ToDictionary(g => g.Key, g => (IList<CommitteeMember>)g.ToList());
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var committee = await _context.CommitteeMembers
                .Include(c => c.Lecturer)
                    .ThenInclude(l => l!.User)
                .Include(c => c.Program)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (committee == null)
            {
                TempData["ErrorMessage"] = "Committee member not found.";
                return RedirectToPage();
            }

            try
            {
                // Remove committee member
                _context.CommitteeMembers.Remove(committee);

                // Update lecturer's committee status if no other committee roles
                var lecturer = committee.Lecturer;
                if (lecturer != null)
                {
                    var otherCommitteeRoles = await _context.CommitteeMembers
                        .Where(c => c.LecturerId == lecturer.Id && c.Id != id && c.IsActive)
                        .CountAsync();

                    if (otherCommitteeRoles == 0)
                    {
                        lecturer.IsCommittee = false;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Committee member '{committee.Lecturer?.User?.FullName}' has been removed from '{committee.Program?.Name}' committee.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while removing the committee member.";
            }

            return RedirectToPage();
        }
    }
}
