using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models;

public class SpeakerRequest
{
    public int Id { get; set; }
    [MaxLength(100)] public string ContactName       { get; set; } = "";
    [MaxLength(200)] public string Organization      { get; set; } = "";
    [MaxLength(200)] public string Email             { get; set; } = "";
    [MaxLength(20)]  public string Phone             { get; set; } = "";
    public DateTime EventDate { get; set; }
    [MaxLength(100)] public string? EventType        { get; set; }
    [MaxLength(50)]  public string? ExpectedAttendance { get; set; }
    [MaxLength(100)] public string? TopicInterest    { get; set; }
    [MaxLength(300)] public string EventLocation     { get; set; } = "";
    public string? AdditionalInfo { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
