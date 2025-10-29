using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TutoringSession.Models;

namespace TutoringSession.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult StudentPage()
        {
            // Use session to check if user is logged in and is a Student
            var role = HttpContext.Session.GetString("Role");
            if (role != "Student")
                return RedirectToAction("Login", "Auth");

            return View();
        }
    }
}
