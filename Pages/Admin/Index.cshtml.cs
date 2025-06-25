using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalPrograms { get; set; }
        public int TotalLecturers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalProposals { get; set; }

        public async Task OnGetAsync()
        {
            TotalPrograms = await _context.Programs.CountAsync();
            TotalLecturers = await _context.Lecturers.CountAsync();
            TotalStudents = await _context.Students.CountAsync();
            TotalProposals = await _context.Proposals.CountAsync();
        }
    }
}
