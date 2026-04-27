using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models
{
    public class OpinionPost
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = "";

        [Required, MaxLength(200)]
        public string Slug { get; set; } = "";

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(100)]
        public string Author { get; set; } = "CU4J Editorial";

        [MaxLength(500)]
        public string? Excerpt { get; set; }

        public string? Body { get; set; }

        public string? CoverImageUrl { get; set; }

        public bool IsPublished { get; set; } = false;

        public DateTime? PublishedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int ViewCount { get; set; } = 0;
    }
}
