using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models;

public class VolunteerSignup
{
    public int Id { get; set; }
    [MaxLength(50)]  public string FirstName    { get; set; } = "";
    [MaxLength(50)]  public string LastName     { get; set; } = "";
    [MaxLength(200)] public string Email        { get; set; } = "";
    [MaxLength(20)]  public string Phone        { get; set; } = "";
    [MaxLength(10)]  public string ZipCode      { get; set; } = "";
    [MaxLength(50)]  public string? Availability { get; set; }
    [MaxLength(500)] public string? Interests   { get; set; }  // comma-separated
    public string? Notes { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
