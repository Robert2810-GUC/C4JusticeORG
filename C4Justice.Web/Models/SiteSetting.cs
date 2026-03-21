using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models;

/// <summary>
/// Key/value store for site-wide settings managed through the admin panel.
/// </summary>
public class SiteSetting
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    public string? Value { get; set; }

    [MaxLength(200)]
    public string? Label { get; set; }   // Human-readable label shown in admin

    [MaxLength(50)]
    public string? Group { get; set; }   // e.g. "Hero", "Stats", "About", "Footer"
}
