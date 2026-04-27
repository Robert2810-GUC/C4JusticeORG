using Microsoft.AspNetCore.Mvc;
using C4Justice.Web.Data;
using C4Justice.Web.Models;
using C4Justice.Web.Helpers;
using C4Justice.Web.Areas.Admin.Filters;

namespace C4Justice.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class UsersController : Controller
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        private bool IsSuperAdmin => HttpContext.Session.GetString("AdminRole") == "super";
        private int CurrentUserId => HttpContext.Session.GetInt32("AdminUserId") ?? 0;

        // ── LIST ─────────────────────────────────────────────────────────────
        public IActionResult Index()
        {
            if (!IsSuperAdmin) return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            var users = _db.AdminUsers.OrderBy(u => u.CreatedAt).ToList();
            return View(users);
        }

        // ── CREATE ───────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsSuperAdmin) return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(string username, string email, string password, string confirmPassword, string role)
        {
            if (!IsSuperAdmin) return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "All fields are required.";
                return View();
            }

            if (password != confirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return View();
            }

            if (password.Length < 8)
            {
                TempData["Error"] = "Password must be at least 8 characters.";
                return View();
            }

            if (_db.AdminUsers.Any(u => u.Username == username.Trim()))
            {
                TempData["Error"] = "That username is already taken.";
                return View();
            }

            _db.AdminUsers.Add(new AdminUser
            {
                Username     = username.Trim(),
                Email        = email.Trim(),
                PasswordHash = AuthHelper.HashPassword(password),
                Role         = role == "super" ? "super" : "admin",
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow
            });
            _db.SaveChanges();

            TempData["Success"] = $"Admin user '{username}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ── TOGGLE ACTIVE ────────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ToggleActive(int id)
        {
            if (!IsSuperAdmin) return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            if (id == CurrentUserId)
            {
                TempData["Error"] = "You cannot deactivate your own account.";
                return RedirectToAction(nameof(Index));
            }
            var user = _db.AdminUsers.Find(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                _db.SaveChanges();
                TempData["Success"] = $"User '{user.Username}' {(user.IsActive ? "activated" : "deactivated")}.";
            }
            return RedirectToAction(nameof(Index));
        }

        // ── DELETE ───────────────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (!IsSuperAdmin) return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            if (id == CurrentUserId)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }
            var user = _db.AdminUsers.Find(id);
            if (user != null) { _db.AdminUsers.Remove(user); _db.SaveChanges(); }
            TempData["Success"] = "User deleted.";
            return RedirectToAction(nameof(Index));
        }

        // ── CHANGE PASSWORD ──────────────────────────────────────────────────
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var user = _db.AdminUsers.Find(CurrentUserId);
            if (user == null) return RedirectToAction("Login", "Auth", new { area = "Admin" });

            if (!AuthHelper.VerifyPassword(currentPassword, user.PasswordHash))
            {
                TempData["Error"] = "Current password is incorrect.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                TempData["Error"] = "New password must be at least 8 characters.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                return View();
            }

            user.PasswordHash = AuthHelper.HashPassword(newPassword);
            _db.SaveChanges();

            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction(nameof(ChangePassword));
        }
    }
}
