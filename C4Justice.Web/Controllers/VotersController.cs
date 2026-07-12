using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using System.Text;

namespace C4Justice.Web.Controllers;

public class VotersController : Controller
{
    private readonly AppDbContext _db;

    public VotersController(AppDbContext db) => _db = db;

    public IActionResult Index(
        string? month,
        [FromQuery(Name = "counties")] List<string>? counties,
        string? status,
        string? gender,
        [FromQuery(Name = "ages")] List<string>? ages,
        [FromQuery(Name = "races")] List<string>? races)
    {
        // Check dashboard protection setting
        var protectedSetting = _db.SiteSettings.FirstOrDefault(s => s.Key == "voters_dashboard_protected");
        if (string.Equals(protectedSetting?.Value, "true", StringComparison.OrdinalIgnoreCase)
            && HttpContext.Session.GetInt32("AdminUserId") == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }

        // Available months for dropdown
        var availableMonths = _db.VoterData
            .Select(v => v.AsOfDate)
            .Distinct()
            .OrderByDescending(d => d)
            .AsEnumerable()
            .Select(d => d.ToString("yyyy-MM"))
            .ToList();

        // All counties for multi-select
        var allCounties = _db.VoterData
            .Select(v => v.County)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        var vm = new VoterReportViewModel
        {
            Month        = month,
            Counties     = counties ?? new(),
            Status       = status,
            Gender       = gender,
            AgeGroups    = ages ?? new(),
            Races        = races ?? new(),
            AvailableMonths = availableMonths,
            AllCounties  = allCounties,
        };

        bool hasAnyFilter = !string.IsNullOrEmpty(month)
            || (counties?.Any() == true)
            || !string.IsNullOrEmpty(status)
            || !string.IsNullOrEmpty(gender)
            || (ages?.Any() == true)
            || (races?.Any() == true);

        // Default to most recent month when no filter at all
        if (!hasAnyFilter && availableMonths.Any())
        {
            vm.Month = availableMonths[0];
            hasAnyFilter = true;
        }

        if (hasAnyFilter)
        {
            vm.Results = RunQuery(vm);
            vm.TotalVoterCount = vm.Results.Sum(r => r.VoterCount);
            vm.CountyTotals = vm.Results
                .GroupBy(r => r.County)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.VoterCount));
            vm.HasSearched = true;
        }

        return View(vm);
    }

    public IActionResult Export(
        string? month,
        [FromQuery(Name = "counties")] List<string>? counties,
        string? status,
        string? gender,
        [FromQuery(Name = "ages")] List<string>? ages,
        [FromQuery(Name = "races")] List<string>? races)
    {
        // Respect protection setting for export too
        var protectedSetting = _db.SiteSettings.FirstOrDefault(s => s.Key == "voters_dashboard_protected");
        if (string.Equals(protectedSetting?.Value, "true", StringComparison.OrdinalIgnoreCase)
            && HttpContext.Session.GetInt32("AdminUserId") == null)
        {
            return Unauthorized();
        }

        var vm = new VoterReportViewModel
        {
            Month     = month,
            Counties  = counties ?? new(),
            Status    = status,
            Gender    = gender,
            AgeGroups = ages ?? new(),
            Races     = races ?? new(),
        };

        var rows = RunQuery(vm);

        var countyTotals = rows
            .GroupBy(r => r.County)
            .ToDictionary(g => g.Key, g => g.Sum(r => r.VoterCount));

        var sb = new StringBuilder();
        sb.AppendLine("County,Voter Status,Age Group,Gender,Race,Voter Count,County Total");
        foreach (var r in rows)
        {
            var countyTotal = countyTotals.TryGetValue(r.County, out var ct) ? ct : 0;
            sb.AppendLine($"\"{r.County}\",\"{r.VoterStatus}\",\"{r.AgeGroup}\",\"{r.Gender}\",\"{r.Race}\",{r.VoterCount},{countyTotal}");
        }

        var filename = $"voters_{month ?? "all"}_{DateTime.Now:yyyyMMdd}.csv";
        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", filename);
    }

    // ── Private ──────────────────────────────────────────────

    private List<VoterResultRow> RunQuery(VoterReportViewModel vm)
    {
        var q = _db.VoterData.AsQueryable();

        if (!string.IsNullOrEmpty(vm.Month)
            && DateTime.TryParseExact(vm.Month + "-01", "yyyy-MM-dd", null,
                System.Globalization.DateTimeStyles.None, out var monthDate))
        {
            var monthEnd = monthDate.AddMonths(1);
            q = q.Where(v => v.AsOfDate >= monthDate && v.AsOfDate < monthEnd);
        }

        if (vm.Counties.Any())
            q = q.Where(v => vm.Counties.Contains(v.County));

        if (!string.IsNullOrEmpty(vm.Status) && vm.Status != "All")
            q = q.Where(v => v.VoterStatus == vm.Status);

        if (!string.IsNullOrEmpty(vm.Gender) && vm.Gender != "All")
            q = q.Where(v => v.Gender == vm.Gender);

        if (vm.AgeGroups.Any())
            q = q.Where(v => vm.AgeGroups.Contains(v.AgeGroup));

        if (vm.Races.Any())
            q = q.Where(v => vm.Races.Contains(v.Race));

        return q
            .GroupBy(v => new { v.County, v.VoterStatus, v.AgeGroup, v.Gender, v.Race })
            .Select(g => new VoterResultRow
            {
                County      = g.Key.County,
                VoterStatus = g.Key.VoterStatus,
                AgeGroup    = g.Key.AgeGroup,
                Gender      = g.Key.Gender,
                Race        = g.Key.Race,
                VoterCount  = g.Sum(v => v.VoterCount),
            })
            .OrderBy(r => r.County)
            .ThenBy(r => r.VoterStatus)
            .ThenBy(r => r.AgeGroup)
            .ThenBy(r => r.Gender)
            .ThenBy(r => r.Race)
            .ToList();
    }
}
