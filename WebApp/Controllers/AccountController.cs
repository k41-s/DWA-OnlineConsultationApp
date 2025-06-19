using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Models;
using WebApp.ViewModels;
using System.Text;
using System.Security.Cryptography;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ConsultationsContext _context;

        public AccountController(ConsultationsContext context)
        {
            _context = context;
        }

        // Helper method to hash a password string
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null, string? reason = null)
        {
            if (reason == "unauthorized")
                TempData["Message"] = "You must be logged in to access that page";

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string pwdHash = HashPassword(model.Password);

            var user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == pwdHash);

            if (user == null || user.Role != "admin")
            {
                ModelState.AddModelError("", "Invalid credentials or not an administrator.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Mentor");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
