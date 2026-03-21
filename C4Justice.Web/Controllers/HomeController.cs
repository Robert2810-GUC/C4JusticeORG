using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using C4Justice.Web.Models;
using C4Justice.Web.Data;

namespace C4Justice.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        try
        {
            ViewBag.SliderImages = _db.SliderImages
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToList();

            ViewBag.UpcomingEvents = _db.Events
                .Where(e => e.IsActive && e.EventDate >= DateTime.UtcNow)
                .OrderBy(e => e.EventDate)
                .Take(3)
                .ToList();

            ViewBag.LatestArticles = _db.Articles
                .Where(a => a.IsPublished)
                .OrderByDescending(a => a.PublishedAt)
                .Take(3)
                .ToList();

            // Load site settings as dictionary for easy access in view
            ViewBag.Settings = _db.SiteSettings
                .ToDictionary(s => s.Key, s => s.Value ?? "");
        }
        catch
        {
            ViewBag.SliderImages = new List<SliderImage>();
            ViewBag.UpcomingEvents = new List<SiteEvent>();
            ViewBag.LatestArticles = new List<Article>();
            ViewBag.Settings = new Dictionary<string, string>();
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Subscribe(MailingListViewModel model)
    {
        if (ModelState.IsValid)
        {
            TempData["SubscribeSuccess"] = $"Thank you, {model.Name}! You've been added to our mailing list.";
            return RedirectToAction(nameof(Index));
        }
        return View(nameof(Index), model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
