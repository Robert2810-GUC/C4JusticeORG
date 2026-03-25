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
        private readonly IWebHostEnvironment _env;

        public ArticlesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
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

            // Handle image upload
            if (FeaturedImage != null && FeaturedImage.Length > 0)
            {
                var uploadUrl = await SaveArticleImageAsync(FeaturedImage);
                if (uploadUrl != null) model.FeaturedImageUrl = uploadUrl;
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

            // If a new image was uploaded, save it (replacing old one if present)
            if (newImage != null && newImage.Length > 0)
            {
                // Delete old image file if it lives in our uploads folder
                if (!string.IsNullOrWhiteSpace(existing.FeaturedImageUrl)
                    && existing.FeaturedImageUrl.StartsWith("/uploads/articles/"))
                {
                    var oldPath = Path.Combine(_env.WebRootPath,
                        existing.FeaturedImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var uploadUrl = await SaveArticleImageAsync(newImage);
                if (uploadUrl != null) existing.FeaturedImageUrl = uploadUrl;
            }
            else if (!string.IsNullOrWhiteSpace(model.FeaturedImageUrl))
            {
                // User manually typed/kept a URL in the hidden field
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

        // ── Helper ────────────────────────────────────────────────────────────
        private async Task<string?> SaveArticleImageAsync(IFormFile file)
        {
            // Validate extension
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) return null;

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "articles");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/articles/{fileName}";
        }
    }
}
