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
                var query = _db.OpinionPosts.Where(o => o.IsPublished);
                if (!string.IsNullOrWhiteSpace(category))
                    query = query.Where(o => o.Category == category);

                ViewBag.Posts = query.OrderByDescending(o => o.PublishedAt).ToList();
                ViewBag.ActiveCategory = category;
            }
            catch
            {
                ViewBag.Posts = new List<object>();
            }
            return View();
        }

        public IActionResult Detail(string slug)
        {
            try
            {
                var post = _db.OpinionPosts.FirstOrDefault(o => o.Slug == slug && o.IsPublished);
                if (post == null) return NotFound();
                return View(post);
            }
            catch { return NotFound(); }
        }
    }
}
