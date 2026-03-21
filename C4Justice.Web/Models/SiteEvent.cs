using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models
{
    public class SiteEvent
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required]
        public DateTime EventDate { get; set; }

        [MaxLength(300)]
        public string? Location { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? RegisterUrl { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsUpcoming => EventDate >= DateTime.UtcNow;
    }
}
