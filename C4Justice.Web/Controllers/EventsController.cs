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
            var upcoming = _db.Events
                .Where(e => e.IsActive && e.EventDate >= DateTime.UtcNow)
                .OrderBy(e => e.EventDate)
                .ToList();

            var past = _db.Events
                .Where(e => e.IsActive && e.EventDate < DateTime.UtcNow)
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
