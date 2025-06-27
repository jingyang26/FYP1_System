using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Admin.Programs
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

        public async Task OnGetAsync()
        {
            Programs = await _context.Programs
                .Include(p => p.Students)
                .Include(p => p.Lecturers)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
