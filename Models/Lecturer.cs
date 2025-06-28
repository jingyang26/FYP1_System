using System.ComponentModel.DataAnnotations;

namespace FYP1System.Models
{
    public class Lecturer
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        [Display(Name = "Staff ID")]
        [StringLength(20)]
        public string? StaffId { get; set; }

        [Display(Name = "Program")]
        public int ProgramId { get; set; }
        public virtual Program Program { get; set; } = null!;

        [Display(Name = "Domain/Specialization")]
        [StringLength(200)]
        public string? Domain { get; set; }

        [Display(Name = "Specialization Details")]
        [StringLength(500)]
        public string? Specialization { get; set; }

        [Display(Name = "Office Location")]
        [StringLength(100)]
        public string? OfficeLocation { get; set; }

        [Display(Name = "Can Supervise")]
        public bool CanSupervise { get; set; } = true;

        [Display(Name = "Is Committee Member")]
        public bool IsCommittee { get; set; } = false;

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ICollection<Student> SupervisedStudents { get; set; } = new List<Student>();
        public virtual ICollection<CommitteeMember> CommitteeRoles { get; set; } = new List<CommitteeMember>();
        public virtual ICollection<EvaluatorAssignment> EvaluatorAssignments { get; set; } = new List<EvaluatorAssignment>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
