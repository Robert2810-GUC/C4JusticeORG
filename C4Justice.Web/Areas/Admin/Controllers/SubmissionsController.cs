using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class SubmissionsController : Controller
    {
        private readonly AppDbContext _db;
        public SubmissionsController(AppDbContext db) => _db = db;

        public IActionResult Index()
            => View(_db.ContactSubmissions.OrderByDescending(s => s.SubmittedAt).ToList());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            var item = await _db.ContactSubmissions.FindAsync(id);
            if (item != null) { item.IsRead = true; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            var unread = _db.ContactSubmissions.Where(s => !s.IsRead).ToList();
            foreach (var item in unread) item.IsRead = true;
            await _db.SaveChangesAsync();
            TempData["Success"] = "All messages marked as read.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.ContactSubmissions.FindAsync(id);
            if (item != null) { _db.ContactSubmissions.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Submission deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
