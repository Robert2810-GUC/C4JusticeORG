using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Helpers;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class ArticlesController : Controller
    {
        private readonly AppDbContext _db;
        public ArticlesController(AppDbContext db) => _db = db;

        public IActionResult Index()
            => View(_db.Articles.OrderByDescending(a => a.CreatedAt).ToList());

        [HttpGet]
        public IActionResult Create() => View(new Article());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article model)
        {
            ModelState.Remove("Slug");
            if (!ModelState.IsValid) return View(model);

            model.Slug = AuthHelper.GenerateSlug(model.Title);
            // Ensure unique slug
            var baseSlug = model.Slug;
            int i = 1;
            while (_db.Articles.Any(a => a.Slug == model.Slug))
                model.Slug = $"{baseSlug}-{i++}";

            model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;
            if (model.IsPublished) model.PublishedAt = DateTime.UtcNow;

            _db.Articles.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Article created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _db.Articles.FindAsync(id);
            if (article == null) return NotFound();
            return View(article);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Article model)
        {
            ModelState.Remove("Slug");
            if (!ModelState.IsValid) return View(model);

            var existing = await _db.Articles.FindAsync(model.Id);
            if (existing == null) return NotFound();

            existing.Title = model.Title;
            existing.Content = model.Content;
            existing.Excerpt = model.Excerpt;
            existing.FeaturedImageUrl = model.FeaturedImageUrl;
            existing.Category = model.Category;
            existing.UpdatedAt = DateTime.UtcNow;

            if (model.IsPublished && !existing.IsPublished)
                existing.PublishedAt = DateTime.UtcNow;
            existing.IsPublished = model.IsPublished;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Article updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Articles.FindAsync(id);
            if (item != null) { _db.Articles.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Article deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var item = await _db.Articles.FindAsync(id);
            if (item != null)
            {
                item.IsPublished = !item.IsPublished;
                if (item.IsPublished && item.PublishedAt == null)
                    item.PublishedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
