using System.ComponentModel.DataAnnotations;

namespace FYP1System.Models
{
    public class EvaluatorAssignment
    {
        public int Id { get; set; }

        [Display(Name = "Proposal")]
        public int ProposalId { get; set; }
        public virtual Proposal Proposal { get; set; } = null!;

        [Display(Name = "Evaluator")]
        public int EvaluatorId { get; set; }
        public virtual Lecturer Evaluator { get; set; } = null!;

        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Assigned At")]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Assigned By")]
        public string? AssignedBy { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation property for the evaluation (if exists)
        public virtual ProposalEvaluation? Evaluation { get; set; }
    }
}
