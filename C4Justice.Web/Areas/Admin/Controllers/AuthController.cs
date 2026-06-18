using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Helpers;
using C4Justice.Web.Services;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly RecaptchaService _recaptcha;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, RecaptchaService recaptcha, IConfiguration config)
        {
            _db = db;
            _recaptcha = recaptcha;
            _config = config;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("AdminUserId") != null)
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            ViewBag.RecaptchaSiteKey = _config["Recaptcha:SiteKey"] ?? "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string recaptchaToken)
        {
            ViewBag.RecaptchaSiteKey = _config["Recaptcha:SiteKey"] ?? "";
            if (!await _recaptcha.VerifyAsync(recaptchaToken))
            {
                ViewBag.Error = "reCAPTCHA verification failed. Please try again.";
                return View();
            }
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter username and password.";
                return View();
            }

            var hash = AuthHelper.HashPassword(password);
            var user = _db.AdminUsers.FirstOrDefault(u => u.Username == username && u.PasswordHash == hash);

            if (user == null || !user.IsActive)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            user.LastLoginAt = DateTime.UtcNow;
            _db.SaveChanges();

            HttpContext.Session.SetInt32("AdminUserId", user.Id);
            HttpContext.Session.SetString("AdminUsername", user.Username);
            HttpContext.Session.SetString("AdminRole", user.Role);
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet, Route("Admin/Logout")]
        public IActionResult LogoutGet()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
