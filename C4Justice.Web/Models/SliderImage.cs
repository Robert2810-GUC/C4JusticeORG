using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models
{
    public class SliderImage
    {
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string ImageUrl { get; set; } = "";

        [MaxLength(300)]
        public string? Title { get; set; }

        [MaxLength(600)]
        public string? Subtitle { get; set; }

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
