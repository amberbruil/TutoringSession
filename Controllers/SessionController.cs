using Microsoft.AspNetCore.Mvc;
using TutoringSession.Models;

namespace TutoringSession.Controllers
{
    public class SessionController : Controller
    {
        // GET: /Session/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View(new Session());
        }

        // POSTL /Session/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Session model)
        {
            if (ModelState.IsValid)
            {
                // Redisplay form with validation errors
                return View(model);
            }

            // TempData carries data across a redirect
            TempData["Lecturer"] = model.LecturerName;
            TempData["Student"] = model.StudentName;
            TempData["Date"] = model.SessionDate.ToShortDateString();
            TempData["Hours"] = model.HoursTutored.ToString("0.##");
            TempData["Rate"] = model.HourlyRate.ToString("0.00");
            TempData["Total"] = model.CalculateEarnings().ToString("0.00");

            return RedirectToAction("Summary");

        }

        // GET: /Session/Summary
        [HttpGet]
        public IActionResult Summary()
        {
            return View();
        }
    }
}
