using System.ComponentModel.DataAnnotations;

namespace FYP1System.Models
{
    public class CommitteeMember
    {
        public int Id { get; set; }

        [Display(Name = "Lecturer")]
        public int LecturerId { get; set; }
        public virtual Lecturer Lecturer { get; set; } = null!;

        [Display(Name = "Program")]
        public int ProgramId { get; set; }
        public virtual Program Program { get; set; } = null!;

        [Display(Name = "Role")]
        [StringLength(100)]
        public string? Role { get; set; }

        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
