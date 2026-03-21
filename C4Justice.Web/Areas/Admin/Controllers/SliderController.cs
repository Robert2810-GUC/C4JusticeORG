using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class SliderController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
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
                var ext = Path.GetExtension(imageFile.FileName).ToLower();
                var fileName = $"slider_{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(_env.WebRootPath, "uploads", "slider", fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                model.ImageUrl = $"/uploads/slider/{fileName}";
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
            if (item != null) { _db.SliderImages.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Slider image deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
