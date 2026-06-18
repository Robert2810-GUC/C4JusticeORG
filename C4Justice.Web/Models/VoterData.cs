using System.ComponentModel.DataAnnotations;

namespace C4Justice.Web.Models;

public class VoterData
{
    public int Id { get; set; }

    public DateTime AsOfDate { get; set; }

    [MaxLength(100)]
    public string County { get; set; } = string.Empty;

    [MaxLength(20)]
    public string VoterStatus { get; set; } = string.Empty;

    [MaxLength(20)]
    public string AgeGroup { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Race { get; set; } = string.Empty;

    public int VoterCount { get; set; }
}
