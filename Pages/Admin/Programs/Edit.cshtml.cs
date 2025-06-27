using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;

namespace FYP1System.Pages.Admin.Programs
{
    [Authorize(Policy = "AdminOnly")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Program Program { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var program = await _context.Programs
                .Include(p => p.Students)
                .Include(p => p.Lecturers)
                .Include(p => p.CommitteeMembers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (program == null)
            {
                return NotFound();
            }

            Program = program;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if another program with the same name exists (excluding current program)
            var existingProgram = await _context.Programs
                .FirstOrDefaultAsync(p => p.Name.ToLower() == Program.Name.ToLower() && p.Id != Program.Id);

            if (existingProgram != null)
            {
                ModelState.AddModelError("Program.Name", "A program with this name already exists.");
                return Page();
            }

            _context.Attach(Program).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Program '{Program.Name}' has been updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgramExists(Program.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProgramExists(int id)
        {
            return _context.Programs.Any(e => e.Id == id);
        }
    }
}
