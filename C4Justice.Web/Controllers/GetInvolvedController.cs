using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Models;

namespace C4Justice.Web.Controllers;

public class GetInvolvedController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Contact(ContactViewModel model)
    {
        if (ModelState.IsValid)
        {
            // TODO: Integrate with email service
            TempData["ContactSuccess"] = "Thank you for reaching out! We'll get back to you within 48 hours.";
            return RedirectToAction(nameof(Index));
        }
        return View(nameof(Index), model);
    }
}
