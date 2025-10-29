using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TutoringSession.Data;
using static TutoringSession.Data.TutoringDbContext;

namespace TutoringSession.Controllers
{
    public class AuthController : Controller
    {
        private readonly TutoringDbContext _db;
        public AuthController(TutoringDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Check user credentials
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == vm.Id);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid user ID or password.");
                return View(vm);
            }
            // Verify password
            var hash = PasswordHasher.Hash(vm.Password);
            if (!string.Equals(hash, user.PasswordHash, StringComparison.Ordinal))
            {
                ModelState.AddModelError(string.Empty, "Invalid user ID or password.");
                return View(vm);
            }

            // Successful Login - store in session
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role);

            // Redirect based on role (NOT Id's)
            if (user.Role == "Tutor")
            {
                return RedirectToAction("Index", "Session"); // tutor dashboard
            }
            else if (user.Role == "Student")
            {
                return RedirectToAction("StudentPage", "Home"); // student landing
            } else
            {
                return RedirectToAction("Login"); // fallback
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // clear stored role & Id
            return RedirectToAction("Login");
        }
    }

    public class LoginVM
    {
        [Required, Display(Name = "User ID")]
        public string Id { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
