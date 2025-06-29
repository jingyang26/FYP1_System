using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;
using Microsoft.AspNetCore.Authorization;

namespace FYP1System.Pages.Supervisor.Students
{
    [Authorize(Roles = "Supervisor")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public FYP1System.Models.Student Student { get; set; } = default!;
        public List<Proposal> StudentProposals { get; set; } = new List<Proposal>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Program)
                .Include(s => s.Supervisor)
                .ThenInclude(sup => sup!.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            // Verify that the current user is the supervisor of this student
            var currentUserId = User.Identity?.Name;
            var currentLecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.User.Email == currentUserId);

            if (currentLecturer == null || student.SupervisorId != currentLecturer.Id)
            {
                return Forbid();
            }

            Student = student;

            // Get student's proposals
            StudentProposals = await _context.Proposals
                .Where(p => p.StudentId == student.Id)
                .OrderByDescending(p => p.SubmittedAt)
                .ToListAsync();

            return Page();
        }
    }
}
