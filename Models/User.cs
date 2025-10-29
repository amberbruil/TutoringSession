using System.ComponentModel.DataAnnotations;

namespace TutoringSession.Models
{
    public class User
    {
        // PK: Tutor/student ID required for login
        [Key]
        [Required, StringLength(32)]
        public string Id { get; set; } = string.Empty;

        // Store hash, not plain text
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // "Tutor" or "Student"
        [Required, StringLength(20)]
        public string Role { get; set; } = "Tutor"; // Default role is Tutor
    }
}
