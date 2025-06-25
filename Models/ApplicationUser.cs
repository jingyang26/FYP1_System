using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FYP1System.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Program")]
        public int? ProgramId { get; set; }
        public virtual Program? Program { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        public virtual Student? Student { get; set; }
        public virtual Lecturer? Lecturer { get; set; }
    }
}
