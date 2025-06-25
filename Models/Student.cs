using System.ComponentModel.DataAnnotations;

namespace FYP1System.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        [Display(Name = "Program")]
        public int ProgramId { get; set; }
        public virtual Program Program { get; set; } = null!;

        [Display(Name = "Student ID")]
        [StringLength(20)]
        public string? StudentId { get; set; }

        [Display(Name = "Supervisor")]
        public int? SupervisorId { get; set; }
        public virtual Lecturer? Supervisor { get; set; }

        [Display(Name = "Session")]
        [StringLength(20)]
        public string? Session { get; set; }

        [Display(Name = "Semester")]
        public int? Semester { get; set; }

        [Display(Name = "GPA")]
        public decimal? GPA { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
    }
}
