using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Areas.Admin.Filters;
using C4Justice.Web.Data;
using C4Justice.Web.Models;

namespace C4Justice.Web.Areas.Admin.Controllers;

[Area("Admin")]
[AdminAuthFilter]
public class SettingsController : Controller
{
    private readonly AppDbContext _db;
    public SettingsController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        var settings = _db.SiteSettings.OrderBy(s => s.Group).ThenBy(s => s.Label).ToList();
        return View(settings);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Save(Dictionary<string, string> values)
    {
        foreach (var kv in values)
        {
            var setting = _db.SiteSettings.FirstOrDefault(s => s.Key == kv.Key);
            if (setting != null)
                setting.Value = kv.Value;
        }
        _db.SaveChanges();
        TempData["Success"] = "Settings saved successfully!";
        return RedirectToAction(nameof(Index));
    }
}
