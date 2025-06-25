using System.ComponentModel.DataAnnotations;

namespace FYP1System.Models
{
    public class Program
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Program Name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<Lecturer> Lecturers { get; set; } = new List<Lecturer>();
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ICollection<CommitteeMember> CommitteeMembers { get; set; } = new List<CommitteeMember>();
    }
}
