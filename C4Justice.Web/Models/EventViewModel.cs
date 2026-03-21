namespace C4Justice.Web.Models;

public class EventViewModel
{
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? RegisterUrl { get; set; }
    public string Category { get; set; } = "Community";
}
