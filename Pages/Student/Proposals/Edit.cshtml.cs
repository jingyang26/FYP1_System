using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;
using System.ComponentModel.DataAnnotations;

namespace FYP1System.Pages.Student.Proposals
{
    [Authorize(Policy = "StudentOnly")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [BindProperty]
        public Proposal Proposal { get; set; } = null!;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<ProposalEvaluation> Evaluations { get; set; } = new();

        public class InputModel
        {
            [Required]
            [Display(Name = "Project Title")]
            [StringLength(300)]
            public string Title { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Project Type")]
            public ProposalType Type { get; set; }

            [Display(Name = "Project Description")]
            [StringLength(2000)]
            public string? Description { get; set; }

            [Display(Name = "Project Objectives")]
            [StringLength(1000)]
            public string? Objectives { get; set; }

            [Display(Name = "Methodology")]
            [StringLength(1000)]
            public string? Methodology { get; set; }

            [Display(Name = "Expected Outcomes")]
            [StringLength(1000)]
            public string? ExpectedOutcomes { get; set; }

            [Display(Name = "Proposal File")]
            public IFormFile? ProposalFile { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null) return NotFound();

            Proposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == student.Id);

            if (Proposal == null) return NotFound();

            // Check if proposal can be edited
            if (Proposal.Status != ProposalStatus.Draft && 
                Proposal.Status != ProposalStatus.NeedsRevision)
            {
                TempData["ErrorMessage"] = "This proposal cannot be edited at this time.";
                return RedirectToPage("./Details", new { id = Proposal.Id });
            }

            // Load evaluations for feedback
            Evaluations = await _context.ProposalEvaluations
                .Include(pe => pe.Evaluator)
                .ThenInclude(e => e.User)
                .Where(pe => pe.ProposalId == id)
                .OrderByDescending(pe => pe.EvaluatedAt)
                .ToListAsync();

            // Populate input model
            Input.Title = Proposal.Title;
            Input.Type = Proposal.Type;
            Input.Description = Proposal.Description;
            Input.Objectives = Proposal.Objectives;
            Input.Methodology = Proposal.Methodology;
            Input.ExpectedOutcomes = Proposal.ExpectedOutcomes;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null) return NotFound();

            Proposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Id == Proposal.Id && p.StudentId == student.Id);

            if (Proposal == null) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadEvaluationsAsync();
                return Page();
            }

            // Handle file upload if provided
            if (Input.ProposalFile != null)
            {
                var filePath = await SaveFileAsync(Input.ProposalFile, student.Id);
                Proposal.FilePath = filePath;
            }

            // Update proposal
            Proposal.Title = Input.Title;
            Proposal.Type = Input.Type;
            Proposal.Description = Input.Description;
            Proposal.Objectives = Input.Objectives;
            Proposal.Methodology = Input.Methodology;
            Proposal.ExpectedOutcomes = Input.ExpectedOutcomes;
            Proposal.UpdatedAt = DateTime.UtcNow;

            if (action == "submit")
            {
                Proposal.Status = ProposalStatus.Resubmitted;
                Proposal.SubmittedAt = DateTime.UtcNow;
            }
            else
            {
                Proposal.Status = ProposalStatus.Draft;
            }

            await _context.SaveChangesAsync();

            if (action == "submit")
            {
                TempData["SuccessMessage"] = "Proposal updated and resubmitted successfully!";
            }
            else
            {
                TempData["SuccessMessage"] = "Proposal saved as draft successfully!";
            }

            return RedirectToPage("./Details", new { id = Proposal.Id });
        }

        private async Task LoadEvaluationsAsync()
        {
            Evaluations = await _context.ProposalEvaluations
                .Include(pe => pe.Evaluator)
                .ThenInclude(e => e.User)
                .Where(pe => pe.ProposalId == Proposal.Id)
                .OrderByDescending(pe => pe.EvaluatedAt)
                .ToListAsync();
        }

        private async Task<string> SaveFileAsync(IFormFile file, int studentId)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "proposals");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"proposal_{studentId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/proposals/{fileName}";
        }
    }
}
