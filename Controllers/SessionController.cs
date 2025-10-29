using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using TutoringSession.Dtos; // make sure this matches your namespace

namespace TutoringSession.Controllers
{
    public class SessionController : Controller
    {
        private readonly IHttpClientFactory _http;

        public SessionController(IHttpClientFactory http)
        {
            _http = http;
        }

        // Display the input form
        [HttpGet]
        public IActionResult Index()
        {
            // Check if user is logged in and is a Tutor using session
            var role = HttpContext.Session.GetString("Role");
            if (role != "Tutor")
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(new SessionVm());
        }

        // Handle form submission and call Micro API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SessionVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var client = _http.CreateClient("SessionsApi");

            // Map SessionVm to SessionCreateDto
            var createDto = new SessionCreateDto
            {
                LecturerName = vm.LecturerName,
                StudentName = vm.StudentName,
                SessionDate = vm.SessionDate,
                HoursTutored = vm.HoursTutored,
                HourlyRate = vm.HourlyRate
            };

            // Send to Micro API (MapPost endpoint)
            var response = await client.PostAsJsonAsync("/api/sessions", createDto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create tutoring session.");
                return View(vm);
            }

            // Read the API response (SessionReadDto)
            var readDto = await response.Content.ReadFromJsonAsync<SessionReadDto>();
            if (readDto is null)
            {
                ModelState.AddModelError("", "Invalid response from API.");
                return View(vm);
            }

            // Map SessionReadDto to SummaryVm
            var summaryVm = new SummaryVm
            {
                LecturerName = readDto.LecturerName,
                StudentName = readDto.StudentName,
                SessionDate = readDto.SessionDate,
                HoursTutored = readDto.HoursTutored, 
                HourlyRate = readDto.HourlyRate,
                FeeAmount = readDto.FeeAmount
            };

            // strongly typed ViewModel to Summary view
            return View("Summary", summaryVm);
        }

        // Display Summary view directly if navigated with existing model
        [HttpGet]
        public IActionResult Summary(SummaryVm vm)
        {
            // Check if user is logged in and is a Tutor using session
            var role = HttpContext.Session.GetString("Role");
            if (!string.Equals(role, "Tutor", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Login", "Auth");

            if (string.IsNullOrWhiteSpace(vm.LecturerName))
                return RedirectToAction(nameof(Index));

            return View(vm);
        }
    }

    // -------------------- VIEW MODELS --------------------
    /// <summary>
    /// ViewModel for tutoring session input form
    /// </summary>
    public class SessionVm
    {
        [Required]
        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Session Date")]
        public DateTime SessionDate { get; set; } = DateTime.Today;

        [Required]
        [Range(0.5, 12, ErrorMessage = "Hours must be between 0.5 and 12.")]
        [Display(Name = "Hours Tutored")]
        public double HoursTutored { get; set; } = 0.5;

        [Required]
        [Range(50, 200, ErrorMessage = "Hourly rate must be between R50 and R200.")]
        [Display(Name = "Hourly Rate (R)")]
        public decimal HourlyRate { get; set; }

        // client-side live preview
        public decimal PreviewFee { get; set; }
    }

    /// <summary>
    /// ViewModel for tutoring session summary
    /// </summary>
    public class SummaryVm
    {
        public string LecturerName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public double HoursTutored { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal FeeAmount { get; set; }
    }
}
