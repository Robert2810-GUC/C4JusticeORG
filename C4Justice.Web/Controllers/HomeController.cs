using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using C4Justice.Web.Models;
using C4Justice.Web.Data;
using C4Justice.Web.Helpers;
using C4Justice.Web.Services;

namespace C4Justice.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    private readonly RecaptchaService _recaptcha;

    public HomeController(AppDbContext db, RecaptchaService recaptcha)
    {
        _db = db;
        _recaptcha = recaptcha;
    }

    public IActionResult Index()
    {
        try
        {
            ViewBag.SliderImages = _db.SliderImages
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToList();

            var _today = DateTime.UtcNow.Date;
            ViewBag.UpcomingEvents = _db.Events
                .Where(e => e.IsActive && !e.IsCompleted)
                .Where(e => e.EndDate.HasValue ? e.EndDate.Value >= _today : e.EventDate >= _today)
                .OrderBy(e => e.EventDate)
                .Take(3)
                .ToList();

            ViewBag.LatestArticles = _db.Articles
                .Where(a => a.IsPublished)
                .OrderByDescending(a => a.PublishedAt)
                .Take(3)
                .ToList();

            ViewBag.CollagePhotos = _db.GalleryPhotos
                .Where(p => p.GalleryType == "collage" && p.IsActive)
                .OrderBy(p => p.SortOrder)
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
            ViewBag.CollagePhotos = new List<GalleryPhoto>();
            ViewBag.Settings = new Dictionary<string, string>();
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(MailingListViewModel model, string? recaptchaToken)
    {
        if (!EmailValidator.IsValid(model.Email))
            ModelState.AddModelError("Email", "Please enter a valid email address.");

        if (!ModelState.IsValid)
        {
            TempData["SubscribeError"] = "Please enter a valid name and email.";
            return RedirectToAction(nameof(Index));
        }

        if (!await _recaptcha.VerifyAsync(recaptchaToken))
        {
            TempData["SubscribeError"] = "Security check failed. Please try again.";
            return RedirectToAction(nameof(Index));
        }

        var existing = _db.NewsletterSubscriptions
            .FirstOrDefault(n => n.Email == model.Email.Trim());
        if (existing == null)
        {
            _db.NewsletterSubscriptions.Add(new Models.NewsletterSubscription
            {
                FullName     = model.Name.Trim(),
                Email        = model.Email.Trim(),
                Phone        = model.Phone?.Trim(),
                ZipCode      = model.ZipCode?.Trim(),
                SubscribedAt = DateTime.UtcNow
            });
            _db.SaveChanges();
        }
        TempData["SubscribeSuccess"] = $"Thank you, {model.Name}! You've been added to our mailing list.";
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
