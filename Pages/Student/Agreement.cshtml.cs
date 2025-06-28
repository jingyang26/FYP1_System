using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Student
{
    [Authorize(Policy = "StudentOnly")]
    public class AgreementModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AgreementModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Models.Student? Student { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            Student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Program)
                .Include(s => s.Supervisor)
                .ThenInclude(s => s!.User)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (Student == null)
            {
                TempData["ErrorMessage"] = "Please complete your profile first.";
                return RedirectToPage("./Profile");
            }

            return Page();
        }
    }
}
