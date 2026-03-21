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
            ViewBag.ArticleCount   = _db.Articles.Count();
            ViewBag.EventCount     = _db.Events.Count();
            ViewBag.DocumentCount  = _db.Documents.Count();
            ViewBag.SliderCount    = _db.SliderImages.Count();
            ViewBag.UpcomingEvents = _db.Events.Count(e => e.EventDate >= DateTime.UtcNow && e.IsActive);
            ViewBag.PublishedArticles = _db.Articles.Count(a => a.IsPublished);
            ViewBag.RecentArticles = _db.Articles.OrderByDescending(a => a.CreatedAt).Take(5).ToList();
            ViewBag.RecentEvents   = _db.Events.OrderByDescending(e => e.CreatedAt).Take(5).ToList();
            return View();
        }
    }
}
