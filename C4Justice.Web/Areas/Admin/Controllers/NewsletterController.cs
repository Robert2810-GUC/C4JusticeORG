using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class NewsletterController : Controller
    {
        private readonly AppDbContext _db;
        public NewsletterController(AppDbContext db) => _db = db;

        public IActionResult Index()
            => View(_db.NewsletterSubscriptions.OrderByDescending(n => n.SubscribedAt).ToList());

        public IActionResult Export()
        {
            var subs = _db.NewsletterSubscriptions.OrderByDescending(n => n.SubscribedAt).ToList();
            var csv = "Full Name,Email,Phone,Zip Code,Topic Interest,Date Joined\n"
                + string.Join("\n", subs.Select(s =>
                    $"\"{s.FullName}\",\"{s.Email}\",\"{s.Phone ?? ""}\",\"{s.ZipCode ?? ""}\",\"{s.TopicInterest ?? ""}\",\"{s.SubscribedAt:yyyy-MM-dd}\""));
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "subscribers.csv");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.NewsletterSubscriptions.FindAsync(id);
            if (item != null) { _db.NewsletterSubscriptions.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Subscriber removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
