using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models
{
    public class SiteDocument
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = "";

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required, MaxLength(100)]
        public string Category { get; set; } = "General";

        [Required, MaxLength(500)]
        public string FileUrl { get; set; } = "";

        [Required, MaxLength(300)]
        public string FileName { get; set; } = "";

        [Required, MaxLength(50)]
        public string FileType { get; set; } = "";

        public long? FileSize { get; set; }
        public bool IsPublished { get; set; } = true;
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string FileSizeDisplay => FileSize.HasValue
            ? FileSize < 1024 * 1024
                ? $"{FileSize / 1024.0:F1} KB"
                : $"{FileSize / (1024.0 * 1024):F1} MB"
            : "";
    }
}
