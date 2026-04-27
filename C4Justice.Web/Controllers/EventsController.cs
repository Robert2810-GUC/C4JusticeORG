using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;

namespace C4Justice.Web.Controllers;

public class EventsController : Controller
{
    private readonly AppDbContext _db;
    public EventsController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        try
        {
            var today = DateTime.UtcNow.Date;

            // Upcoming: active, not completed, end date (or start date if no end) is today or future
            var upcoming = _db.Events
                .Where(e => e.IsActive && !e.IsCompleted)
                .Where(e => e.EndDate.HasValue ? e.EndDate.Value >= today : e.EventDate >= today)
                .OrderBy(e => e.EventDate)
                .ToList();

            // Past: completed OR end date (or start date) already passed
            var past = _db.Events
                .Where(e => e.IsActive && (e.IsCompleted ||
                    (e.EndDate.HasValue ? e.EndDate.Value < today : e.EventDate < today)))
                .OrderByDescending(e => e.EventDate)
                .Take(4)
                .ToList();

            ViewBag.UpcomingEvents = upcoming;
            ViewBag.PastEvents = past;
        }
        catch
        {
            ViewBag.UpcomingEvents = new List<object>();
            ViewBag.PastEvents = new List<object>();
        }

        return View();
    }
}
