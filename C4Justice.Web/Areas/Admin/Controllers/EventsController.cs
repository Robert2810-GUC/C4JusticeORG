using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class EventsController : Controller
    {
        private readonly AppDbContext _db;
        public EventsController(AppDbContext db) => _db = db;

        public IActionResult Index()
            => View(_db.Events.OrderByDescending(e => e.EventDate).ToList());

        [HttpGet]
        public IActionResult Create() => View(new SiteEvent { EventDate = DateTime.Now.AddDays(7) });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SiteEvent model)
        {
            if (!ModelState.IsValid) return View(model);
            model.CreatedAt = DateTime.UtcNow;
            _db.Events.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Event created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var e = await _db.Events.FindAsync(id);
            if (e == null) return NotFound();
            return View(e);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SiteEvent model)
        {
            if (!ModelState.IsValid) return View(model);
            var existing = await _db.Events.FindAsync(model.Id);
            if (existing == null) return NotFound();

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.EventDate = model.EventDate;
            existing.Location = model.Location;
            existing.ImageUrl = model.ImageUrl;
            existing.RegisterUrl = model.RegisterUrl;
            existing.Category = model.Category;
            existing.IsActive = model.IsActive;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Event updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Events.FindAsync(id);
            if (item != null) { _db.Events.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Event deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
