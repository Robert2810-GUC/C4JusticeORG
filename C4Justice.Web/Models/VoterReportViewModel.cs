namespace C4Justice.Web.Models;

public class VoterReportViewModel
{
    // Active filter values
    public string? Month { get; set; }
    public List<string> Counties { get; set; } = new();
    public string? Status { get; set; }
    public string? Gender { get; set; }
    public List<string> AgeGroups { get; set; } = new();
    public List<string> Races { get; set; } = new();

    // Dropdown options (populated from DB)
    public List<string> AvailableMonths { get; set; } = new();
    public List<string> AllCounties { get; set; } = new();

    // Query results
    public List<VoterResultRow> Results { get; set; } = new();
    public int TotalVoterCount { get; set; }
    public bool HasSearched { get; set; }
}

public class VoterResultRow
{
    public string County { get; set; } = string.Empty;
    public string VoterStatus { get; set; } = string.Empty;
    public string AgeGroup { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public int VoterCount { get; set; }
}
