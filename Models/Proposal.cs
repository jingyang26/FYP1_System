using System.ComponentModel.DataAnnotations;

namespace FYP1System.Models
{
    public enum ProposalType
    {
        Research,
        Development,
        Analysis,
        Design
    }

    public enum ProposalStatus
    {
        Draft,
        Submitted,
        UnderReview,
        Approved,
        Rejected,
        NeedsRevision,
        Resubmitted
    }

    public class Proposal
    {
        public int Id { get; set; }

        [Display(Name = "Student")]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; } = null!;

        [Required]
        [Display(Name = "Title")]
        [StringLength(300)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Type")]
        public ProposalType Type { get; set; }

        [Display(Name = "Status")]
        public ProposalStatus Status { get; set; } = ProposalStatus.Draft;

        [Display(Name = "Description")]
        [StringLength(2000)]
        public string? Description { get; set; }

        [Display(Name = "Objectives")]
        [StringLength(1000)]
        public string? Objectives { get; set; }

        [Display(Name = "Methodology")]
        [StringLength(1000)]
        public string? Methodology { get; set; }

        [Display(Name = "Expected Outcomes")]
        [StringLength(1000)]
        public string? ExpectedOutcomes { get; set; }

        [Display(Name = "Session")]
        [StringLength(20)]
        public string? Session { get; set; }

        [Display(Name = "Semester")]
        public int? Semester { get; set; }

        [Display(Name = "Proposal File")]
        public string? FilePath { get; set; }

        [Display(Name = "Agreement File")]
        public string? AgreementFilePath { get; set; }

        [Display(Name = "Submitted Date")]
        public DateTime? SubmittedAt { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<EvaluatorAssignment> EvaluatorAssignments { get; set; } = new List<EvaluatorAssignment>();
        public virtual ICollection<ProposalEvaluation> ProposalEvaluations { get; set; } = new List<ProposalEvaluation>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
