using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class VolunteersController : Controller
    {
        private readonly AppDbContext _db;
        public VolunteersController(AppDbContext db) => _db = db;

        public IActionResult Index()
            => View(_db.VolunteerSignups.OrderByDescending(v => v.SubmittedAt).ToList());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            var item = await _db.VolunteerSignups.FindAsync(id);
            if (item != null) { item.IsRead = true; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.VolunteerSignups.FindAsync(id);
            if (item != null) { _db.VolunteerSignups.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Record deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
