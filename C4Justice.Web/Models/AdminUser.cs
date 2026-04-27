using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models
{
    public class AdminUser
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = "";

        [Required, MaxLength(500)]
        public string PasswordHash { get; set; } = "";

        [Required, MaxLength(200)]
        public string Email { get; set; } = "";

        [MaxLength(20)]
        public string Role { get; set; } = "admin"; // "super" or "admin"

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }
    }
}
