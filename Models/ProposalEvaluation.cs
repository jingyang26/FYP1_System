using System.ComponentModel.DataAnnotations;

namespace FYP1System.Models
{
    public enum EvaluationStatus
    {
        Pending,
        InProgress,
        Completed,
        Approved,
        Rejected,
        NeedsRevision
    }

    public class ProposalEvaluation
    {
        public int Id { get; set; }

        [Display(Name = "Proposal")]
        public int ProposalId { get; set; }
        public virtual Proposal Proposal { get; set; } = null!;

        [Display(Name = "Evaluator")]
        public int EvaluatorId { get; set; }
        public virtual Lecturer Evaluator { get; set; } = null!;

        [Display(Name = "Status")]
        public EvaluationStatus Status { get; set; } = EvaluationStatus.Pending;

        [Display(Name = "Overall Score")]
        [Range(0, 100)]
        public decimal? OverallScore { get; set; }

        [Display(Name = "Technical Merit Score")]
        [Range(0, 100)]
        public decimal? TechnicalMeritScore { get; set; }

        [Display(Name = "Innovation Score")]
        [Range(0, 100)]
        public decimal? InnovationScore { get; set; }

        [Display(Name = "Feasibility Score")]
        [Range(0, 100)]
        public decimal? FeasibilityScore { get; set; }

        [Display(Name = "Literature Review Score")]
        [Range(0, 100)]
        public decimal? LiteratureReviewScore { get; set; }

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

        [Display(Name = "Evaluated Date")]
        public DateTime? EvaluatedAt { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
