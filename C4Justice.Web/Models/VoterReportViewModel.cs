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
    public int TotalResultCount { get; set; }
    public bool HasSearched { get; set; }
    public Dictionary<string, int> CountyTotals { get; set; } = new();

    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int TotalCountyCount { get; set; }
    public int TotalPages => (PageSize == 0 || TotalResultCount == 0) ? 1 : (int)Math.Ceiling((double)TotalResultCount / PageSize);
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
