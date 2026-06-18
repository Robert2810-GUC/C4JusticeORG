using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Models;
using C4Justice.Web.Data;
using C4Justice.Web.Helpers;
using C4Justice.Web.Services;

namespace C4Justice.Web.Controllers;

public class HomeController(AppDbContext db, RecaptchaService recaptcha) : Controller
{
    public IActionResult Index()
    {
        try
        {
            ViewBag.SliderImages = db.SliderImages
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToList();

            var today = DateTime.UtcNow.Date;
            ViewBag.UpcomingEvents = db.Events
                .Where(e => e.IsActive && !e.IsCompleted)
                .Where(e => e.EndDate.HasValue ? e.EndDate.Value >= today : e.EventDate >= today)
                .OrderBy(e => e.EventDate)
                .Take(3)
                .ToList();

            ViewBag.LatestArticles = db.Articles
                .Where(a => a.IsPublished)
                .OrderByDescending(a => a.PublishedAt)
                .Take(3)
                .ToList();

            ViewBag.CollagePhotos = db.GalleryPhotos
                .Where(p => p.GalleryType == "collage" && p.IsActive)
                .OrderBy(p => p.SortOrder)
                .Take(3)
                .ToList();

            // Load site settings as dictionary for easy access in view
            ViewBag.Settings = db.SiteSettings
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

        if (!await recaptcha.VerifyAsync(recaptchaToken))
        {
            TempData["SubscribeError"] = "Security check failed. Please try again.";
            return RedirectToAction(nameof(Index));
        }

        var existing = db.NewsletterSubscriptions
            .FirstOrDefault(n => n.Email == model.Email.Trim());
        if (existing == null)
        {
            db.NewsletterSubscriptions.Add(new NewsletterSubscription
            {
                FullName     = model.Name.Trim(),
                Email        = model.Email.Trim(),
                Phone        = model.Phone.Trim(),
                ZipCode      = model.ZipCode.Trim(),
                SubscribedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
        TempData["SubscribeSuccess"] = $"Thank you, {model.Name}! You've been added to our mailing list.";
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
