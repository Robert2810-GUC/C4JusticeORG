using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = "";

        [Required, MaxLength(300)]
        public string Slug { get; set; } = "";

        [Required]
        public string Content { get; set; } = "";

        [MaxLength(1000)]
        public string? Excerpt { get; set; }

        [MaxLength(500)]
        public string? FeaturedImageUrl { get; set; }

        [Required, MaxLength(100)]
        public string Category { get; set; } = "General";

        public bool IsPublished { get; set; } = false;
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
