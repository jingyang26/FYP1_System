using System.ComponentModel.DataAnnotations;

namespace FYP1System.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Display(Name = "Proposal")]
        public int ProposalId { get; set; }
        public virtual Proposal Proposal { get; set; } = null!;

        [Display(Name = "User")]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        [Display(Name = "Content")]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Parent Comment")]
        public int? ParentCommentId { get; set; }
        public virtual Comment? ParentComment { get; set; }

        [Display(Name = "Is Private")]
        public bool IsPrivate { get; set; } = false;

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
