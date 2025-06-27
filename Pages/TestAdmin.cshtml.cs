using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages
{
    [Authorize(Policy = "AdminOnly")]
    public class TestAdminModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TestAdminModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Program> Programs { get; set; } = default!;
        public int LecturerCount { get; set; }
        public int SoftwareEngLecturers { get; set; }
        public int DataEngLecturers { get; set; }
        public int CommitteeCount { get; set; }
        public int ChairCount { get; set; }
        public int MemberCount { get; set; }

        public async Task OnGetAsync()
        {
            // Get all programs
            Programs = await _context.Programs
                .OrderBy(p => p.Name)
                .ToListAsync();

            // Get lecturer statistics
            LecturerCount = await _context.Lecturers.CountAsync();
            
            var softwareEngProgram = await _context.Programs
                .FirstOrDefaultAsync(p => p.Name == "Software Engineering");
            var dataEngProgram = await _context.Programs
                .FirstOrDefaultAsync(p => p.Name == "Data Engineering");

            if (softwareEngProgram != null)
            {
                SoftwareEngLecturers = await _context.Lecturers
                    .CountAsync(l => l.ProgramId == softwareEngProgram.Id);
            }

            if (dataEngProgram != null)
            {
                DataEngLecturers = await _context.Lecturers
                    .CountAsync(l => l.ProgramId == dataEngProgram.Id);
            }

            // Get committee statistics
            CommitteeCount = await _context.CommitteeMembers.CountAsync();
            ChairCount = await _context.CommitteeMembers
                .CountAsync(c => c.Role == "Chair");
            MemberCount = await _context.CommitteeMembers
                .CountAsync(c => c.Role == "Member");
        }
    }
}
