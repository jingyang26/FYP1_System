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
    public class EvaluateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EvaluateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Proposal Proposal { get; set; } = null!;
        public int ProposalId { get; set; }

        [BindProperty]
        public EvaluationInputModel Evaluation { get; set; } = new();

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

            [Display(Name = "Overall Score")]
            [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
            public decimal? OverallScore { get; set; }

            [Display(Name = "Status")]
            [Required(ErrorMessage = "Please select an evaluation status")]
            public EvaluationStatus Status { get; set; } = EvaluationStatus.InProgress;

            [Display(Name = "Comments")]
            [StringLength(2000)]
            public string? Comments { get; set; }

            [Display(Name = "Strengths")]
            [StringLength(1000)]
            public string? Strengths { get; set; }

            [Display(Name = "Weaknesses")]
            [StringLength(1000)]
            public string? Weaknesses { get; set; }

            [Display(Name = "Recommendations")]
            [StringLength(1000)]
            public string? Recommendations { get; set; }

            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Get the lecturer record for the current user (acting as evaluator)
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (lecturer == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Check if user is assigned to evaluate this proposal
            var assignment = await _context.EvaluatorAssignments
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.User)
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.Program)
                .FirstOrDefaultAsync(ea => ea.ProposalId == id && ea.EvaluatorId == lecturer.Id && ea.IsActive);

            if (assignment == null)
            {
                TempData["ErrorMessage"] = "Proposal not found or not assigned to you.";
                return RedirectToPage("/Evaluator/Proposals");
            }

            // Get existing evaluation if any
            var existingEvaluation = await _context.ProposalEvaluations
                .FirstOrDefaultAsync(pe => pe.ProposalId == id && pe.EvaluatorId == lecturer.Id);

            Proposal = assignment.Proposal;
            ProposalId = id;

            // Load existing evaluation data if available
            if (existingEvaluation != null)
            {
                Evaluation = new EvaluationInputModel
                {
                    Id = existingEvaluation.Id,
                    TechnicalMeritScore = existingEvaluation.TechnicalMeritScore,
                    InnovationScore = existingEvaluation.InnovationScore,
                    FeasibilityScore = existingEvaluation.FeasibilityScore,
                    LiteratureReviewScore = existingEvaluation.LiteratureReviewScore,
                    OverallScore = existingEvaluation.OverallScore,
                    Status = existingEvaluation.Status,
                    Comments = existingEvaluation.Comments,
                    Strengths = existingEvaluation.Strengths,
                    Weaknesses = existingEvaluation.Weaknesses,
                    Recommendations = existingEvaluation.Recommendations,
                    UpdatedAt = existingEvaluation.UpdatedAt
                };
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

            // Get the lecturer record for the current user (acting as evaluator)
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (lecturer == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Check if user is assigned to evaluate this proposal
            var assignment = await _context.EvaluatorAssignments
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.User)
                .Include(ea => ea.Proposal)
                    .ThenInclude(p => p.Student)
                        .ThenInclude(s => s.Program)
                .FirstOrDefaultAsync(ea => ea.ProposalId == id && ea.EvaluatorId == lecturer.Id && ea.IsActive);

            if (assignment == null)
            {
                TempData["ErrorMessage"] = "Proposal not found or not assigned to you.";
                return RedirectToPage("/Evaluator/Proposals");
            }

            Proposal = assignment.Proposal;
            ProposalId = id;

            // Validate based on action
            if (action == "submit")
            {
                // For final submission, require more validation
                if (Evaluation.Status == EvaluationStatus.InProgress)
                {
                    ModelState.AddModelError("Evaluation.Status", "Please select a final evaluation status for submission.");
                }

                if ((Evaluation.Status == EvaluationStatus.Approved || Evaluation.Status == EvaluationStatus.Rejected || Evaluation.Status == EvaluationStatus.NeedsRevision) 
                    && string.IsNullOrWhiteSpace(Evaluation.Comments))
                {
                    ModelState.AddModelError("Evaluation.Comments", "Comments are required for final evaluation decisions.");
                }
            }
            else
            {
                // For save action, allow in-progress status
                if (Evaluation.Status == EvaluationStatus.Pending)
                {
                    Evaluation.Status = EvaluationStatus.InProgress;
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get or create evaluation record
            var evaluation = await _context.ProposalEvaluations
                .FirstOrDefaultAsync(pe => pe.ProposalId == id && pe.EvaluatorId == lecturer.Id);

            if (evaluation == null)
            {
                evaluation = new ProposalEvaluation
                {
                    ProposalId = id,
                    EvaluatorId = lecturer.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ProposalEvaluations.Add(evaluation);
            }

            // Update evaluation
            evaluation.TechnicalMeritScore = Evaluation.TechnicalMeritScore;
            evaluation.InnovationScore = Evaluation.InnovationScore;
            evaluation.FeasibilityScore = Evaluation.FeasibilityScore;
            evaluation.LiteratureReviewScore = Evaluation.LiteratureReviewScore;
            evaluation.OverallScore = Evaluation.OverallScore;
            evaluation.Status = Evaluation.Status;
            evaluation.Comments = Evaluation.Comments;
            evaluation.Strengths = Evaluation.Strengths;
            evaluation.Weaknesses = Evaluation.Weaknesses;
            evaluation.Recommendations = Evaluation.Recommendations;
            evaluation.UpdatedAt = DateTime.UtcNow;

            if (action == "submit" && (Evaluation.Status == EvaluationStatus.Completed || 
                                       Evaluation.Status == EvaluationStatus.Approved || 
                                       Evaluation.Status == EvaluationStatus.Rejected || 
                                       Evaluation.Status == EvaluationStatus.NeedsRevision))
            {
                evaluation.EvaluatedAt = DateTime.UtcNow;
                
                // Update proposal status based on evaluation
                if (Evaluation.Status == EvaluationStatus.Approved)
                {
                    assignment.Proposal.Status = ProposalStatus.Approved;
                }
                else if (Evaluation.Status == EvaluationStatus.Rejected)
                {
                    assignment.Proposal.Status = ProposalStatus.Rejected;
                }
                else if (Evaluation.Status == EvaluationStatus.NeedsRevision)
                {
                    assignment.Proposal.Status = ProposalStatus.NeedsRevision;
                }
                
                assignment.Proposal.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            if (action == "submit")
            {
                TempData["SuccessMessage"] = "Evaluation submitted successfully.";
                return RedirectToPage("./Details", new { id });
            }
            else
            {
                TempData["SuccessMessage"] = "Evaluation progress saved.";
                return RedirectToPage("./Evaluate", new { id });
            }
        }
    }
}
