using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Areas.Admin.Filters;
using ExcelDataReader;

namespace C4Justice.Web.Areas.Admin.Controllers;

[Area("Admin")]
[AdminAuthFilter]
public class VoterUploadController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public VoterUploadController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public IActionResult Index()
    {
        // Auto-seed the protection toggle if it was never inserted
        if (!_db.SiteSettings.Any(s => s.Key == "voters_dashboard_protected"))
        {
            var nextId = (_db.SiteSettings.Max(s => (int?)s.Id) ?? 0) + 1;
            _db.SiteSettings.Add(new SiteSetting
            {
                Id    = nextId,
                Key   = "voters_dashboard_protected",
                Value = "false",
                Label = "Voter Dashboard — Require Admin Login to Access (true / false)",
                Group = "Voters"
            });
            _db.SaveChanges();
        }

        var months = _db.VoterData
            .Select(v => v.AsOfDate)
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        ViewBag.UploadedMonths = months;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequestSizeLimit(52_428_800)]         // 50 MB
    [RequestFormLimits(MultipartBodyLengthLimit = 52_428_800)]
    public async Task<IActionResult> Upload(IFormFile ExcelFile)
    {
        if (ExcelFile == null || ExcelFile.Length == 0)
        {
            TempData["Error"] = "Please select an Excel file to upload.";
            return RedirectToAction(nameof(Index));
        }

        var ext = Path.GetExtension(ExcelFile.FileName).ToLowerInvariant();
        if (ext != ".xlsx" && ext != ".xls")
        {
            TempData["Error"] = "Only .xlsx and .xls files are supported.";
            return RedirectToAction(nameof(Index));
        }

        // Save to temp location
        var tempDir = Path.Combine(_env.WebRootPath, "uploads", "temp");
        Directory.CreateDirectory(tempDir);

        // Clean up stale temp files (> 2 hours old)
        foreach (var old in Directory.GetFiles(tempDir, "voters_*.xls*"))
        {
            if (System.IO.File.GetCreationTime(old) < DateTime.Now.AddHours(-2))
                System.IO.File.Delete(old);
        }

        var tempPath = Path.Combine(tempDir, $"voters_{Guid.NewGuid()}{ext}");
        await using (var fs = System.IO.File.Create(tempPath))
            await ExcelFile.CopyToAsync(fs);

        List<VoterData> records;
        try
        {
            records = ParseExcel(tempPath);
        }
        catch (Exception ex)
        {
            System.IO.File.Delete(tempPath);
            TempData["Error"] = $"Failed to parse Excel file: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }

        if (records.Count == 0)
        {
            System.IO.File.Delete(tempPath);
            TempData["Error"] = "No data rows found. Check that Sheet1 has data starting at row 4 (header on row 3).";
            return RedirectToAction(nameof(Index));
        }

        var asOfDate = records[0].AsOfDate.Date;
        var existingCount = _db.VoterData.Count(v => v.AsOfDate == asOfDate);

        if (existingCount > 0)
        {
            HttpContext.Session.SetString("VoterTempFile", tempPath);
            HttpContext.Session.SetString("VoterAsOfDate", asOfDate.ToString("yyyy-MM-dd"));
            ViewBag.ExistingCount = existingCount;
            ViewBag.NewCount = records.Count;
            ViewBag.AsOfDate = asOfDate;
            return View("ConfirmReplace");
        }

        await InsertBatchedAsync(records);
        System.IO.File.Delete(tempPath);
        TempData["Success"] = $"Successfully imported {records.Count:N0} records for {asOfDate:MMMM yyyy}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmReplace()
    {
        var tempPath = HttpContext.Session.GetString("VoterTempFile");
        var asOfDateStr = HttpContext.Session.GetString("VoterAsOfDate");

        if (string.IsNullOrEmpty(tempPath) || !System.IO.File.Exists(tempPath))
        {
            TempData["Error"] = "Session expired or file not found. Please upload the file again.";
            return RedirectToAction(nameof(Index));
        }

        var asOfDate = DateTime.Parse(asOfDateStr!).Date;

        List<VoterData> records;
        try
        {
            records = ParseExcel(tempPath);
        }
        catch (Exception ex)
        {
            System.IO.File.Delete(tempPath);
            TempData["Error"] = $"Failed to re-parse file: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }

        // Remove old records for this month
        var old = _db.VoterData.Where(v => v.AsOfDate == asOfDate).ToList();
        _db.VoterData.RemoveRange(old);
        await _db.SaveChangesAsync();

        await InsertBatchedAsync(records);

        System.IO.File.Delete(tempPath);
        HttpContext.Session.Remove("VoterTempFile");
        HttpContext.Session.Remove("VoterAsOfDate");

        TempData["Success"] = $"Replaced {old.Count:N0} old records with {records.Count:N0} new records for {asOfDate:MMMM yyyy}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CancelReplace()
    {
        var tempPath = HttpContext.Session.GetString("VoterTempFile");
        if (!string.IsNullOrEmpty(tempPath) && System.IO.File.Exists(tempPath))
            System.IO.File.Delete(tempPath);

        HttpContext.Session.Remove("VoterTempFile");
        HttpContext.Session.Remove("VoterAsOfDate");

        TempData["Info"] = "Upload cancelled. Existing data was not changed.";
        return RedirectToAction(nameof(Index));
    }

    // ── Private helpers ─────────────────────────────────────

    private static List<VoterData> ParseExcel(string filePath)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var records = new List<VoterData>();

        using var stream = System.IO.File.OpenRead(filePath);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        int rowIndex = 0;
        while (reader.Read())
        {
            rowIndex++;
            if (rowIndex <= 3) continue; // rows 1-2 = pre-header, row 3 = header labels

            // Skip fully empty rows
            if (reader.IsDBNull(0) && reader.IsDBNull(1)) continue;

            DateTime asOfDate;
            try
            {
                asOfDate = reader.IsDBNull(0)
                    ? throw new Exception($"Missing AsOfDate on row {rowIndex}")
                    : reader.GetDateTime(0).Date;
            }
            catch
            {
                // Some files store the date as a string
                var raw = reader.GetValue(0)?.ToString() ?? string.Empty;
                if (!DateTime.TryParse(raw, out asOfDate))
                    continue; // skip rows with unparseable dates
            }

            records.Add(new VoterData
            {
                AsOfDate    = asOfDate,
                County      = reader.GetValue(1)?.ToString()?.Trim() ?? string.Empty,
                VoterStatus = reader.GetValue(2)?.ToString()?.Trim() ?? string.Empty,
                AgeGroup    = reader.GetValue(3)?.ToString()?.Trim() ?? string.Empty,
                Gender      = reader.GetValue(4)?.ToString()?.Trim() ?? string.Empty,
                Race        = reader.GetValue(5)?.ToString()?.Trim() ?? string.Empty,
                VoterCount  = Convert.ToInt32(reader.GetValue(6) ?? 0),
            });
        }

        return records;
    }

    private async Task InsertBatchedAsync(List<VoterData> records, int batchSize = 500)
    {
        for (int i = 0; i < records.Count; i += batchSize)
        {
            var batch = records.Skip(i).Take(batchSize);
            await _db.VoterData.AddRangeAsync(batch);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();
        }
    }
}
