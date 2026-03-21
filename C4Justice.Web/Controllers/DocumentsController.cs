using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;

namespace C4Justice.Web.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly AppDbContext _db;
        public DocumentsController(AppDbContext db) => _db = db;

        public IActionResult Index(string? category)
        {
            try
            {
                var query = _db.Documents.Where(d => d.IsPublished);
                if (!string.IsNullOrWhiteSpace(category))
                    query = query.Where(d => d.Category == category);

                ViewBag.Documents = query.OrderByDescending(d => d.CreatedAt).ToList();
                ViewBag.Categories = _db.Documents
                    .Where(d => d.IsPublished)
                    .Select(d => d.Category)
                    .Distinct()
                    .ToList();
                ViewBag.ActiveCategory = category;
            }
            catch
            {
                ViewBag.Documents = new List<object>();
                ViewBag.Categories = new List<string>();
            }
            return View();
        }
    }
}
