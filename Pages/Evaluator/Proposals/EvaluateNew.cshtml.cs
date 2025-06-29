using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1System.Data;
using FYP1System.Models;
using System.ComponentModel.DataAnnotations;

namespace FYP1System.Pages.Evaluator.Proposals
{
    [Authorize(Roles = "Evaluator")]
    public class EvaluateNewModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EvaluateNewModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Proposal Proposal { get; set; } = null!;
        public ProposalEvaluation? ExistingEvaluation { get; set; }
        public Models.Lecturer? LecturerInfo { get; set; }
        public bool CanEdit { get; set; } = true;

        [BindProperty]
        public EvaluationInputModel Input { get; set; } = new();

        public class EvaluationInputModel
        {
            public int Id { get; set; }

            [Display(Name = "Technical Merit Score")]
            [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
            public decimal? TechnicalMeritScore { get; set; }

            [Display(Name = "Innovation Score")]
            [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
            public decimal? InnovationScore { get; set; }

            [Display(Name = "Feasibility Score")]
            [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
            public decimal? FeasibilityScore { get; set; }

            [Display(Name = "Literature Review Score")]
            [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
            public decimal? LiteratureReviewScore { get; set; }

            [Required]
            [Display(Name = "Evaluation Status")]
            public EvaluationStatus Status { get; set; } = EvaluationStatus.Pending;

            [Display(Name = "Strengths")]
            [StringLength(1000, ErrorMessage = "Strengths cannot exceed 1000 characters")]
            public string? Strengths { get; set; }

            [Display(Name = "Weaknesses")]
            [StringLength(1000, ErrorMessage = "Weaknesses cannot exceed 1000 characters")]
            public string? Weaknesses { get; set; }

            [Display(Name = "Recommendations")]
            [StringLength(1000, ErrorMessage = "Recommendations cannot exceed 1000 characters")]
            public string? Recommendations { get; set; }

            [Display(Name = "Additional Comments")]
            [StringLength(2000, ErrorMessage = "Comments cannot exceed 2000 characters")]
            public string? Comments { get; set; }

            public decimal? OverallScore => CalculateOverallScore();

            private decimal? CalculateOverallScore()
            {
                if (TechnicalMeritScore.HasValue && InnovationScore.HasValue && 
                    FeasibilityScore.HasValue && LiteratureReviewScore.HasValue)
                {
                    return (TechnicalMeritScore.Value + InnovationScore.Value + 
                           FeasibilityScore.Value + LiteratureReviewScore.Value) / 4;
                }
                return null;
            }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            LecturerInfo = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (LecturerInfo == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Get proposal with all related data
            Proposal = await _context.Proposals
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .Include(p => p.Student)
                    .ThenInclude(s => s.Program)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Proposal == null)
            {
                return NotFound("Proposal not found.");
            }

            // Check if this evaluator is assigned to this proposal
            var assignment = await _context.EvaluatorAssignments
                .FirstOrDefaultAsync(ea => ea.ProposalId == id && ea.EvaluatorId == LecturerInfo.Id && ea.IsActive);

            if (assignment == null)
            {
                return Forbid("You are not assigned to evaluate this proposal.");
            }

            // Get existing evaluation if any
            ExistingEvaluation = await _context.ProposalEvaluations
                .FirstOrDefaultAsync(pe => pe.ProposalId == id && pe.EvaluatorId == LecturerInfo.Id);

            if (ExistingEvaluation != null)
            {
                // Populate form with existing data
                Input = new EvaluationInputModel
                {
                    Id = ExistingEvaluation.Id,
                    TechnicalMeritScore = ExistingEvaluation.TechnicalMeritScore,
                    InnovationScore = ExistingEvaluation.InnovationScore,
                    FeasibilityScore = ExistingEvaluation.FeasibilityScore,
                    LiteratureReviewScore = ExistingEvaluation.LiteratureReviewScore,
                    Status = ExistingEvaluation.Status,
                    Strengths = ExistingEvaluation.Strengths,
                    Weaknesses = ExistingEvaluation.Weaknesses,
                    Recommendations = ExistingEvaluation.Recommendations,
                    Comments = ExistingEvaluation.Comments
                };

                // Check if evaluation is final (cannot be edited)
                CanEdit = ExistingEvaluation.Status == EvaluationStatus.Pending || 
                         ExistingEvaluation.Status == EvaluationStatus.InProgress;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, string action)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            LecturerInfo = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (LecturerInfo == null)
            {
                return RedirectToPage("/Account/Login");
            }

            Proposal = await _context.Proposals
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .Include(p => p.Student)
                    .ThenInclude(s => s.Program)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Proposal == null)
            {
                return NotFound("Proposal not found.");
            }

            // Validate assignment
            var assignment = await _context.EvaluatorAssignments
                .FirstOrDefaultAsync(ea => ea.ProposalId == id && ea.EvaluatorId == LecturerInfo.Id && ea.IsActive);

            if (assignment == null)
            {
                return Forbid("You are not assigned to evaluate this proposal.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Handle different actions
            switch (action?.ToLower())
            {
                case "save":
                    Input.Status = EvaluationStatus.InProgress;
                    break;
                case "submit":
                    // Validate that all required fields are filled for submission
                    if (!Input.TechnicalMeritScore.HasValue || !Input.InnovationScore.HasValue ||
                        !Input.FeasibilityScore.HasValue || !Input.LiteratureReviewScore.HasValue)
                    {
                        ModelState.AddModelError("", "All scores must be provided before submission.");
                        return Page();
                    }
                    
                    if (Input.Status == EvaluationStatus.Pending || Input.Status == EvaluationStatus.InProgress)
                    {
                        ModelState.AddModelError("", "Please select a final evaluation status (Accepted, Accepted with Conditions, or Rejected).");
                        return Page();
                    }
                    break;
                default:
                    ModelState.AddModelError("", "Invalid action.");
                    return Page();
            }

            // Get or create evaluation
            var evaluation = await _context.ProposalEvaluations
                .FirstOrDefaultAsync(pe => pe.ProposalId == id && pe.EvaluatorId == LecturerInfo.Id);

            if (evaluation == null)
            {
                evaluation = new ProposalEvaluation
                {
                    ProposalId = id,
                    EvaluatorId = LecturerInfo.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ProposalEvaluations.Add(evaluation);
            }

            // Update evaluation
            evaluation.TechnicalMeritScore = Input.TechnicalMeritScore;
            evaluation.InnovationScore = Input.InnovationScore;
            evaluation.FeasibilityScore = Input.FeasibilityScore;
            evaluation.LiteratureReviewScore = Input.LiteratureReviewScore;
            evaluation.OverallScore = Input.OverallScore;
            evaluation.Status = Input.Status;
            evaluation.Strengths = Input.Strengths;
            evaluation.Weaknesses = Input.Weaknesses;
            evaluation.Recommendations = Input.Recommendations;
            evaluation.Comments = Input.Comments;
            evaluation.UpdatedAt = DateTime.UtcNow;

            if (action?.ToLower() == "submit")
            {
                evaluation.EvaluatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            if (action?.ToLower() == "submit")
            {
                TempData["SuccessMessage"] = "Evaluation submitted successfully!";
                return RedirectToPage("/Evaluator/Proposals");
            }
            else
            {
                TempData["SuccessMessage"] = "Evaluation saved as draft.";
                return RedirectToPage("./Evaluate", new { id = id });
            }
        }
    }
}
