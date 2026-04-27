using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class SpeakersController : Controller
    {
        private readonly AppDbContext _db;
        public SpeakersController(AppDbContext db) => _db = db;

        public IActionResult Index()
            => View(_db.SpeakerRequests.OrderByDescending(s => s.SubmittedAt).ToList());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            var item = await _db.SpeakerRequests.FindAsync(id);
            if (item != null) { item.IsRead = true; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.SpeakerRequests.FindAsync(id);
            if (item != null) { _db.SpeakerRequests.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Request deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
