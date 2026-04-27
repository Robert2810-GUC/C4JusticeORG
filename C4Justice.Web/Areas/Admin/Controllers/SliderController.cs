using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Services;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class SliderController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IStorageService _storage;

        public SliderController(AppDbContext db, IStorageService storage)
        {
            _db = db;
            _storage = storage;
        }

        public IActionResult Index()
            => View(_db.SliderImages.OrderBy(s => s.SortOrder).ThenByDescending(s => s.CreatedAt).ToList());

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderImage model, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var url = await _storage.UploadImageAsync(imageFile, "c4justice/slider");
                if (url != null) model.ImageUrl = url;
            }

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                ModelState.AddModelError("", "Please upload an image.");
                return View(model);
            }

            model.CreatedAt = DateTime.UtcNow;
            _db.SliderImages.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Slider image added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.SliderImages.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SliderImage model, IFormFile? imageFile)
        {
            var item = await _db.SliderImages.FindAsync(id);
            if (item == null) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(item.ImageUrl))
                    await _storage.DeleteAsync(item.ImageUrl);

                var url = await _storage.UploadImageAsync(imageFile, "c4justice/slider");
                if (url != null) item.ImageUrl = url;
            }

            item.Title     = model.Title;
            item.Subtitle  = model.Subtitle;
            item.SortOrder = model.SortOrder;
            item.IsActive  = model.IsActive;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Slider image updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var item = await _db.SliderImages.FindAsync(id);
            if (item != null) { item.IsActive = !item.IsActive; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.SliderImages.FindAsync(id);
            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(item.ImageUrl))
                    await _storage.DeleteAsync(item.ImageUrl);

                _db.SliderImages.Remove(item);
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "Slider image deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
