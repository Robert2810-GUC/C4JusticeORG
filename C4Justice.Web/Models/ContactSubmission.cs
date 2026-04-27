using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models;

public class ContactSubmission
{
    public int Id { get; set; }
    [MaxLength(100)] public string Name    { get; set; } = "";
    [MaxLength(200)] public string Email   { get; set; } = "";
    [MaxLength(20)]  public string? Phone  { get; set; }
    [MaxLength(200)] public string Subject { get; set; } = "";
    public string Message { get; set; } = "";
    public bool IsRead { get; set; } = false;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
