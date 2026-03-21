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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
