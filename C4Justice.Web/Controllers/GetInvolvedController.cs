using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Helpers;
using C4Justice.Web.Services;

namespace C4Justice.Web.Controllers;

public class GetInvolvedController : Controller
{
    private readonly AppDbContext _db;
    private readonly RecaptchaService _recaptcha;

    public GetInvolvedController(AppDbContext db, RecaptchaService recaptcha)
    {
        _db = db;
        _recaptcha = recaptcha;
    }

    public IActionResult Index() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Volunteer(
        string FirstName, string LastName, string Email, string Phone,
        string ZipCode, string? Availability, string? Notes,
        string[]? interests, string? recaptchaToken)
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(Email))
        {
            TempData["GIError"] = "volunteer";
            return RedirectToAction(nameof(Index));
        }

        if (!EmailValidator.IsValid(Email))
        {
            TempData["GIError"] = "volunteer_email";
            return RedirectToAction(nameof(Index));
        }

        if (!await _recaptcha.VerifyAsync(recaptchaToken))
        {
            TempData["GIError"] = "recaptcha";
            return RedirectToAction(nameof(Index));
        }

        _db.VolunteerSignups.Add(new VolunteerSignup
        {
            FirstName    = FirstName.Trim(),
            LastName     = LastName?.Trim() ?? "",
            Email        = Email.Trim(),
            Phone        = Phone?.Trim() ?? "",
            ZipCode      = ZipCode?.Trim() ?? "",
            Availability = Availability,
            Interests    = interests != null ? string.Join(", ", interests) : null,
            Notes        = Notes?.Trim(),
            SubmittedAt  = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        TempData["GISuccess"] = "volunteer";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Newsletter(
        string FullName, string Email, string? Phone,
        string? ZipCode, string? TopicInterest, string? recaptchaToken)
    {
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
        {
            TempData["GIError"] = "newsletter";
            return RedirectToAction(nameof(Index));
        }

        if (!EmailValidator.IsValid(Email))
        {
            TempData["GIError"] = "newsletter_email";
            return RedirectToAction(nameof(Index));
        }

        if (!await _recaptcha.VerifyAsync(recaptchaToken))
        {
            TempData["GIError"] = "recaptcha";
            return RedirectToAction(nameof(Index));
        }

        // Avoid duplicate subscriptions by email
        var existing = _db.NewsletterSubscriptions
            .FirstOrDefault(n => n.Email == Email.Trim());

        if (existing == null)
        {
            _db.NewsletterSubscriptions.Add(new NewsletterSubscription
            {
                FullName      = FullName.Trim(),
                Email         = Email.Trim(),
                Phone         = Phone?.Trim(),
                ZipCode       = ZipCode?.Trim(),
                TopicInterest = TopicInterest,
                SubscribedAt  = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }

        TempData["GISuccess"] = "newsletter";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Speaker(
        string ContactName, string Organization, string Email, string Phone,
        DateTime EventDate, string? EventType, string? ExpectedAttendance,
        string? TopicInterest, string EventLocation, string? AdditionalInfo,
        string? recaptchaToken)
    {
        if (string.IsNullOrWhiteSpace(ContactName) || string.IsNullOrWhiteSpace(Email))
        {
            TempData["GIError"] = "speaker";
            return RedirectToAction(nameof(Index));
        }

        if (!EmailValidator.IsValid(Email))
        {
            TempData["GIError"] = "speaker_email";
            return RedirectToAction(nameof(Index));
        }

        if (!await _recaptcha.VerifyAsync(recaptchaToken))
        {
            TempData["GIError"] = "recaptcha";
            return RedirectToAction(nameof(Index));
        }

        _db.SpeakerRequests.Add(new SpeakerRequest
        {
            ContactName        = ContactName.Trim(),
            Organization       = Organization?.Trim() ?? "",
            Email              = Email.Trim(),
            Phone              = Phone?.Trim() ?? "",
            EventDate          = EventDate,
            EventType          = EventType,
            ExpectedAttendance = ExpectedAttendance,
            TopicInterest      = TopicInterest,
            EventLocation      = EventLocation?.Trim() ?? "",
            AdditionalInfo     = AdditionalInfo?.Trim(),
            SubmittedAt        = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        TempData["GISuccess"] = "speaker";
        return RedirectToAction(nameof(Index));
    }
}
