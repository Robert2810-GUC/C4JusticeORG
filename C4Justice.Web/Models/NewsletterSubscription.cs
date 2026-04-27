using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models;

public class NewsletterSubscription
{
    public int Id { get; set; }
    [MaxLength(100)] public string FullName     { get; set; } = "";
    [MaxLength(200)] public string Email        { get; set; } = "";
    [MaxLength(20)]  public string? Phone       { get; set; }
    [MaxLength(10)]  public string? ZipCode     { get; set; }
    [MaxLength(100)] public string? TopicInterest { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
}
