using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Models;

namespace C4Justice.Web.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ContactViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            TempData["ContactSuccess"] = "Thank you for reaching out! We'll respond within 48 hours.";
            return RedirectToAction(nameof(Index));
        }
    }
}
