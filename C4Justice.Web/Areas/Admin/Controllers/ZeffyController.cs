using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class ZeffyController : Controller
    {
        private readonly AppDbContext _db;
        public ZeffyController(AppDbContext db) => _db = db;

        public IActionResult Index()
        {
            ViewBag.ZeffyUrl = _db.SiteSettings.FirstOrDefault(s => s.Key == "zeffy_form_url")?.Value ?? "";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string? zeffyUrl)
        {
            var setting = _db.SiteSettings.FirstOrDefault(s => s.Key == "zeffy_form_url");
            if (setting == null)
                _db.SiteSettings.Add(new SiteSetting { Key = "zeffy_form_url", Value = zeffyUrl ?? "", Label = "Zeffy Donation URL", Group = "Zeffy" });
            else
                setting.Value = zeffyUrl ?? "";

            await _db.SaveChangesAsync();
            TempData["Success"] = "Zeffy donation URL saved. All donate buttons across the site now use this link.";
            return RedirectToAction(nameof(Index));
        }
    }
}
