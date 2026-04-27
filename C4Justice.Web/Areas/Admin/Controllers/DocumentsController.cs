using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Services;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class DocumentsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IStorageService _storage;

        public DocumentsController(AppDbContext db, IStorageService storage)
        {
            _db = db;
            _storage = storage;
        }

        public IActionResult Index()
            => View(_db.Documents.OrderByDescending(d => d.CreatedAt).ToList());

        [HttpGet]
        public IActionResult Create() => View(new SiteDocument());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SiteDocument model, IFormFile? docFile)
        {
            if (docFile == null || docFile.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a file.");
                return View(model);
            }

            var ext = Path.GetExtension(docFile.FileName).ToLower();
            var url = await _storage.UploadRawAsync(docFile, "c4justice/documents");

            if (url == null)
            {
                ModelState.AddModelError("", "Upload failed. Please try again.");
                return View(model);
            }

            model.FileUrl   = url;
            model.FileName  = docFile.FileName;
            model.FileType  = ext.TrimStart('.');
            model.FileSize  = docFile.Length;
            model.CreatedAt = DateTime.UtcNow;
            if (model.IsPublished) model.PublishedAt = DateTime.UtcNow;

            ModelState.Remove("FileUrl");
            ModelState.Remove("FileName");
            ModelState.Remove("FileType");
            if (!ModelState.IsValid) return View(model);

            _db.Documents.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Document uploaded.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Documents.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string title, string? description, string category, bool isPublished)
        {
            var item = await _db.Documents.FindAsync(id);
            if (item == null) return NotFound();
            item.Title = title;
            item.Description = description;
            item.Category = category;
            if (isPublished && !item.IsPublished && item.PublishedAt == null)
                item.PublishedAt = DateTime.UtcNow;
            item.IsPublished = isPublished;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Document updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var item = await _db.Documents.FindAsync(id);
            if (item != null)
            {
                item.IsPublished = !item.IsPublished;
                if (item.IsPublished && item.PublishedAt == null)
                    item.PublishedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Documents.FindAsync(id);
            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(item.FileUrl))
                    await _storage.DeleteAsync(item.FileUrl, isRaw: true);

                _db.Documents.Remove(item);
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "Document deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
