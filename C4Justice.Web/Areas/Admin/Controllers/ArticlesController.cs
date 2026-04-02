using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Helpers;
using C4Justice.Web.Services;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class ArticlesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ICloudinaryService _cloudinary;

        public ArticlesController(AppDbContext db, ICloudinaryService cloudinary)
        {
            _db = db;
            _cloudinary = cloudinary;
        }

        public IActionResult Index()
            => View(_db.Articles.OrderByDescending(a => a.CreatedAt).ToList());

        [HttpGet]
        public IActionResult Create() => View(new Article());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article model, IFormFile? FeaturedImage)
        {
            ModelState.Remove("Slug");
            ModelState.Remove("FeaturedImageUrl");
            if (!ModelState.IsValid) return View(model);

            if (FeaturedImage != null && FeaturedImage.Length > 0)
            {
                var url = await _cloudinary.UploadImageAsync(FeaturedImage, "c4justice/articles");
                if (url != null) model.FeaturedImageUrl = url;
            }

            model.Slug = AuthHelper.GenerateSlug(model.Title);
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
        public async Task<IActionResult> Edit(Article model, IFormFile? newImage)
        {
            ModelState.Remove("Slug");
            ModelState.Remove("FeaturedImageUrl");
            if (!ModelState.IsValid) return View(model);

            var existing = await _db.Articles.FindAsync(model.Id);
            if (existing == null) return NotFound();

            existing.Title     = model.Title;
            existing.Content   = model.Content;
            existing.Excerpt   = model.Excerpt;
            existing.Category  = model.Category;
            existing.UpdatedAt = DateTime.UtcNow;

            if (newImage != null && newImage.Length > 0)
            {
                // Delete old image from Cloudinary if it lives there
                if (!string.IsNullOrWhiteSpace(existing.FeaturedImageUrl)
                    && existing.FeaturedImageUrl.Contains("cloudinary.com"))
                {
                    await _cloudinary.DeleteAsync(existing.FeaturedImageUrl);
                }

                var url = await _cloudinary.UploadImageAsync(newImage, "c4justice/articles");
                if (url != null) existing.FeaturedImageUrl = url;
            }
            else if (!string.IsNullOrWhiteSpace(model.FeaturedImageUrl))
            {
                existing.FeaturedImageUrl = model.FeaturedImageUrl;
            }

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
            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(item.FeaturedImageUrl)
                    && item.FeaturedImageUrl.Contains("cloudinary.com"))
                {
                    await _cloudinary.DeleteAsync(item.FeaturedImageUrl);
                }
                _db.Articles.Remove(item);
                await _db.SaveChangesAsync();
            }
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
