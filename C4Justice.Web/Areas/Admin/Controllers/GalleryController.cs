using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Services;
using C4Justice.Web.Areas.Admin.Filters;
using Microsoft.EntityFrameworkCore;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class GalleryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IStorageService _storage;

        public GalleryController(AppDbContext db, IStorageService storage)
        {
            _db = db;
            _storage = storage;
        }

        public IActionResult Index()
        {
            ViewBag.SliderImages = _db.SliderImages.OrderBy(s => s.SortOrder).ThenByDescending(s => s.CreatedAt).ToList();
            ViewBag.GalleryPhotos = _db.GalleryPhotos.Where(p => p.GalleryType == "community").OrderBy(p => p.SortOrder).ToList();
            ViewBag.CollagePhotos = _db.GalleryPhotos.Where(p => p.GalleryType == "collage").OrderBy(p => p.SortOrder).Take(3).ToList();
            return View();
        }

        // Upload to community gallery
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadGallery(IFormFile imageFile, string? label)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var url = await _storage.UploadImageAsync(imageFile, "c4justice/gallery");
                if (url != null)
                {
                    _db.GalleryPhotos.Add(new GalleryPhoto
                    {
                        ImageUrl = url,
                        Label = label,
                        GalleryType = "community",
                        SortOrder = _db.GalleryPhotos.Count(p => p.GalleryType == "community"),
                        CreatedAt = DateTime.UtcNow
                    });
                    await _db.SaveChangesAsync();
                }
            }
            TempData["Success"] = "Photo uploaded to gallery.";
            return RedirectToAction(nameof(Index));
        }

        // Upload to collage
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCollage(IFormFile imageFile, string? label, int slot)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var url = await _storage.UploadImageAsync(imageFile, "c4justice/collage");
                if (url != null)
                {
                    var existing = _db.GalleryPhotos.FirstOrDefault(p => p.GalleryType == "collage" && p.SortOrder == slot);
                    if (existing != null)
                    {
                        if (!string.IsNullOrWhiteSpace(existing.ImageUrl))
                            await _storage.DeleteAsync(existing.ImageUrl);
                        existing.ImageUrl = url;
                        existing.Label = label;
                    }
                    else
                    {
                        _db.GalleryPhotos.Add(new GalleryPhoto
                        {
                            ImageUrl = url, Label = label,
                            GalleryType = "collage", SortOrder = slot,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    await _db.SaveChangesAsync();
                }
            }
            TempData["Success"] = "Collage photo updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGalleryPhoto(int id, string? label)
        {
            var photo = await _db.GalleryPhotos.FindAsync(id);
            if (photo != null) { photo.Label = label; await _db.SaveChangesAsync(); }
            TempData["Success"] = "Photo updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGalleryPhoto(int id)
        {
            var photo = await _db.GalleryPhotos.FindAsync(id);
            if (photo != null)
            {
                if (!string.IsNullOrWhiteSpace(photo.ImageUrl))
                    await _storage.DeleteAsync(photo.ImageUrl);
                _db.GalleryPhotos.Remove(photo);
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "Photo deleted.";
            return RedirectToAction(nameof(Index));
        }

        // Promote gallery photo to hero slider
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToSlider(int galleryId)
        {
            var photo = await _db.GalleryPhotos.FindAsync(galleryId);
            if (photo != null)
            {
                _db.SliderImages.Add(new SliderImage
                {
                    ImageUrl = photo.ImageUrl,
                    Title = photo.Label ?? "Community Photo",
                    SortOrder = _db.SliderImages.Count(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();
                TempData["Success"] = "Photo added to hero slider.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Slider actions (previously in SliderController, kept for compatibility)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSlider(IFormFile imageFile, string? title, string? subtitle, int sortOrder = 0)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var url = await _storage.UploadImageAsync(imageFile, "c4justice/slider");
                if (url != null)
                {
                    _db.SliderImages.Add(new SliderImage
                    {
                        ImageUrl = url, Title = title ?? "", Subtitle = subtitle,
                        SortOrder = sortOrder, IsActive = true, CreatedAt = DateTime.UtcNow
                    });
                    await _db.SaveChangesAsync();
                }
            }
            TempData["Success"] = "Slider image added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSlider(int id, string? title, string? subtitle, int sortOrder, bool isActive, IFormFile? imageFile)
        {
            var item = await _db.SliderImages.FindAsync(id);
            if (item != null)
            {
                item.Title = title ?? "";
                item.Subtitle = subtitle;
                item.SortOrder = sortOrder;
                item.IsActive = isActive;

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(item.ImageUrl))
                        await _storage.DeleteAsync(item.ImageUrl);
                    var url = await _storage.UploadImageAsync(imageFile, "c4justice/slider");
                    if (url != null) item.ImageUrl = url;
                }
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "Slider updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSlider(int id)
        {
            var item = await _db.SliderImages.FindAsync(id);
            if (item != null) { item.IsActive = !item.IsActive; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        // Reorder endpoints — called via AJAX from drag-and-drop UI
        [HttpPost]
        public async Task<IActionResult> ReorderSlider([FromBody] int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var item = await _db.SliderImages.FindAsync(ids[i]);
                if (item != null) item.SortOrder = i;
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ReorderGallery([FromBody] int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var photo = await _db.GalleryPhotos.FindAsync(ids[i]);
                if (photo != null) photo.SortOrder = i;
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSlider(int id)
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
