using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models
{
    public class GalleryPhoto
    {
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string ImageUrl { get; set; } = "";

        [MaxLength(200)]
        public string? Label { get; set; }

        [MaxLength(50)]
        public string GalleryType { get; set; } = "community";

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
