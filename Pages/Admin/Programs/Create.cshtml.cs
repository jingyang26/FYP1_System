using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;
using System.ComponentModel.DataAnnotations;

namespace FYP1System.Pages.Admin.Programs
{
    [Authorize(Policy = "AdminOnly")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Program Program { get; set; } = default!;

        public IActionResult OnGet()
        {
            Program = new Models.Program 
            { 
                IsActive = true 
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if program name already exists
            var existingProgram = await _context.Programs
                .FirstOrDefaultAsync(p => p.Name.ToLower() == Program.Name.ToLower());

            if (existingProgram != null)
            {
                ModelState.AddModelError("Program.Name", "A program with this name already exists.");
                return Page();
            }

            try
            {
                _context.Programs.Add(Program);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Program '{Program.Name}' has been created successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while creating the program. Please try again.");
                return Page();
            }
        }
    }
}
