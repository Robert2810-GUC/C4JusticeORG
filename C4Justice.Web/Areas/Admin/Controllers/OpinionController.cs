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
    public class OpinionController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IStorageService _storage;

        public OpinionController(AppDbContext db, IStorageService storage)
        {
            _db = db;
            _storage = storage;
        }

        public IActionResult Index()
            => View(_db.OpinionPosts.OrderByDescending(o => o.CreatedAt).ToList());

        [HttpGet]
        public IActionResult Create() => View(new OpinionPost());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OpinionPost model, IFormFile? coverImage, string? action)
        {
            ModelState.Remove("Slug");
            ModelState.Remove("CoverImageUrl");

            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                model.Slug = AuthHelper.GenerateSlug(model.Title);
                var baseSlug = model.Slug;
                int i = 1;
                while (_db.OpinionPosts.Any(o => o.Slug == model.Slug))
                    model.Slug = $"{baseSlug}-{i++}";
            }

            if (coverImage != null && coverImage.Length > 0)
            {
                var url = await _storage.UploadImageAsync(coverImage, "c4justice/opinion");
                if (url != null) model.CoverImageUrl = url;
            }

            model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;
            model.IsPublished = action == "publish";
            if (model.IsPublished) model.PublishedAt = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("", "Title is required.");
                return View(model);
            }

            _db.OpinionPosts.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = model.IsPublished ? "Post published." : "Draft saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _db.OpinionPosts.FindAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OpinionPost model, IFormFile? coverImage, string? action)
        {
            var existing = await _db.OpinionPosts.FindAsync(model.Id);
            if (existing == null) return NotFound();

            existing.Title    = model.Title;
            existing.Category = model.Category;
            existing.Author   = model.Author;
            existing.Excerpt  = model.Excerpt;
            existing.Body     = model.Body;
            existing.UpdatedAt = DateTime.UtcNow;

            if (coverImage != null && coverImage.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(existing.CoverImageUrl))
                    await _storage.DeleteAsync(existing.CoverImageUrl);
                var url = await _storage.UploadImageAsync(coverImage, "c4justice/opinion");
                if (url != null) existing.CoverImageUrl = url;
            }

            if (action == "publish" && !existing.IsPublished)
            {
                existing.IsPublished = true;
                existing.PublishedAt = DateTime.UtcNow;
            }
            else if (action == "unpublish")
            {
                existing.IsPublished = false;
            }
            else if (action == "draft")
            {
                existing.IsPublished = false;
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Post updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var post = await _db.OpinionPosts.FindAsync(id);
            if (post != null)
            {
                post.IsPublished = !post.IsPublished;
                if (post.IsPublished && post.PublishedAt == null)
                    post.PublishedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _db.OpinionPosts.FindAsync(id);
            if (post != null)
            {
                if (!string.IsNullOrWhiteSpace(post.CoverImageUrl))
                    await _storage.DeleteAsync(post.CoverImageUrl);
                _db.OpinionPosts.Remove(post);
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "Post deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
