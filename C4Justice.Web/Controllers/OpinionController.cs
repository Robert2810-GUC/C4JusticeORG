using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;

namespace C4Justice.Web.Controllers
{
    public class OpinionController : Controller
    {
        private readonly AppDbContext _db;
        public OpinionController(AppDbContext db) => _db = db;

        public IActionResult Index(string? category)
        {
            try
            {
                var query = _db.Articles.Where(a => a.IsPublished);
                if (!string.IsNullOrWhiteSpace(category))
                    query = query.Where(a => a.Category == category);

                ViewBag.Articles = query.OrderByDescending(a => a.PublishedAt).ToList();
                ViewBag.ActiveCategory = category;
            }
            catch
            {
                ViewBag.Articles = new List<object>();
            }
            return View();
        }

        public IActionResult Detail(string slug)
        {
            try
            {
                var article = _db.Articles.FirstOrDefault(a => a.Slug == slug && a.IsPublished);
                if (article == null) return NotFound();
                return View(article);
            }
            catch { return NotFound(); }
        }
    }
}
