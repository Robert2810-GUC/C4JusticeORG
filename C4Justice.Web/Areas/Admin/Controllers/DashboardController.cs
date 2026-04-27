using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        public DashboardController(AppDbContext db) => _db = db;

        public IActionResult Index()
        {
            ViewBag.SubscriberCount  = _db.NewsletterSubscriptions.Count(s => s.IsActive);
            ViewBag.UpcomingEvents   = _db.Events.Count(e => e.EventDate >= DateTime.UtcNow && e.IsActive);
            ViewBag.DocumentCount    = _db.Documents.Count();
            ViewBag.PendingRequests  = _db.ContactSubmissions.Count(c => !c.IsRead);

            ViewBag.RecentEvents     = _db.Events.OrderByDescending(e => e.CreatedAt).Take(5).ToList();
            ViewBag.RecentDocuments  = _db.Documents.OrderByDescending(d => d.CreatedAt).Take(4).ToList();
            ViewBag.RecentSubscribers = _db.NewsletterSubscriptions.OrderByDescending(s => s.SubscribedAt).Take(4).ToList();
            ViewBag.RecentMessages   = _db.ContactSubmissions.OrderByDescending(s => s.SubmittedAt).Take(4).ToList();

            return View();
        }
    }
}
