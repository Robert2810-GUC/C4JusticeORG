using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class DocumentsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
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
            var fileName = $"doc_{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(_env.WebRootPath, "uploads", "documents", fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await docFile.CopyToAsync(stream);

            model.FileUrl = $"/uploads/documents/{fileName}";
            model.FileName = docFile.FileName;
            model.FileType = ext.TrimStart('.');
            model.FileSize = docFile.Length;
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
            if (item != null) { _db.Documents.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Document deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
