using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Helpers;
using C4Justice.Web.Services;

namespace C4Justice.Web.Controllers
{
    public class ContactController(AppDbContext db, RecaptchaService recaptcha) : Controller
    {
        public IActionResult Index() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactViewModel model, string? phone, string? recaptchaToken)
        {
            if (!EmailValidator.IsValid(model.Email))
                ModelState.AddModelError("Email", "Please enter a valid email address.");

            if (!ModelState.IsValid)
                return View(model);

            if (!await recaptcha.VerifyAsync(recaptchaToken))
            {
                ModelState.AddModelError("", "Security check failed. Please try again.");
                return View(model);
            }

            db.ContactSubmissions.Add(new ContactSubmission
            {
                Name        = model.Name.Trim(),
                Email       = model.Email.Trim(),
                Phone       = phone?.Trim(),
                Subject     = model.Subject.Trim(),
                Message     = model.Message.Trim(),
                SubmittedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            TempData["ContactSuccess"] = "Thank you for reaching out! We'll respond within 2 business days.";
            return RedirectToAction(nameof(Index));
        }
    }
}
