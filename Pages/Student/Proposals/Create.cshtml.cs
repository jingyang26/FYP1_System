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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

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

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Supervisor)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Please complete your profile first.";
                return RedirectToPage("../Profile");
            }

            if (student.Supervisor == null)
            {
                TempData["ErrorMessage"] = "Please select a supervisor before submitting a proposal.";
                return RedirectToPage("../SelectSupervisor");
            }

            // Check if student can submit new proposal
            var latestProposal = await _context.Proposals
                .Where(p => p.StudentId == student.Id)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestProposal != null && 
                latestProposal.Status != ProposalStatus.Rejected && 
                latestProposal.Status != ProposalStatus.Draft)
            {
                TempData["ErrorMessage"] = "You already have a pending proposal. Please wait for evaluation results.";
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Student profile not found.";
                return RedirectToPage("../Profile");
            }

            // Validate based on submission type
            var submissionType = Request.Form["SubmissionType"].ToString();
            
            if (submissionType == "pdf")
            {
                // For PDF submission, only title and type are required
                ModelState.Remove("Input.Description");
                ModelState.Remove("Input.Objectives");
                ModelState.Remove("Input.Methodology");
                ModelState.Remove("Input.ExpectedOutcomes");

                if (Input.ProposalFile == null)
                {
                    ModelState.AddModelError("Input.ProposalFile", "Please upload the signed proposal form.");
                }
            }
            else
            {
                // For online submission, remove file requirement
                ModelState.Remove("Input.ProposalFile");
                
                // Validate required fields for online submission
                if (string.IsNullOrWhiteSpace(Input.Description))
                {
                    ModelState.AddModelError("Input.Description", "Project description is required for online submission.");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Handle file upload if provided
            string? filePath = null;
            if (Input.ProposalFile != null)
            {
                filePath = await SaveFileAsync(Input.ProposalFile, student.Id);
            }

            // Create new proposal
            var proposal = new Proposal
            {
                StudentId = student.Id,
                Title = Input.Title,
                Type = Input.Type,
                Description = Input.Description,
                Objectives = Input.Objectives,
                Methodology = Input.Methodology,
                ExpectedOutcomes = Input.ExpectedOutcomes,
                FilePath = filePath,
                Session = student.Session,
                Semester = student.Semester,
                Status = action == "submit" ? ProposalStatus.Submitted : ProposalStatus.Draft,
                SubmittedAt = action == "submit" ? DateTime.UtcNow : null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Proposals.Add(proposal);
            await _context.SaveChangesAsync();

            if (action == "submit")
            {
                TempData["SuccessMessage"] = "Proposal submitted successfully! You will be notified once the evaluation is complete.";
            }
            else
            {
                TempData["SuccessMessage"] = "Proposal saved as draft successfully! You can edit and submit it later.";
            }

            return RedirectToPage("./Index");
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
