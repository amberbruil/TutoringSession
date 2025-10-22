using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using TutoringSession.Dtos;
using TutoringSession.Models;

namespace TutoringSession.Controllers
{
    public class SessionController : Controller
    {
        // Inject IHttpClientFactory to create HttpClient instances
        private readonly IHttpClientFactory _http;
        public SessionController(IHttpClientFactory http) => _http = http;

        [HttpGet]
        public IActionResult Index() => View(new Session());

        /// <summary>
        /// Process form submission to create a new tutoring session.
        /// SessionCreateDto is sent to the API. 
        /// SessionReadDto is returned from the API.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Session model)
        {
            if (!ModelState.IsValid) 
                return View(model);

            // --- Call the API to create the session record ---
            var create = new SessionCreateDto
            {
                LecturerName = model.LecturerName,
                StudentName = model.StudentName,
                HoursTutored = model.HoursTutored,
                HourlyRate = model.HourlyRate,
                SessionDate = model.SessionDate
            };

            var client = _http.CreateClient(); // create HttpClient object to send web request
            var baseUrl = $"{Request.Scheme}://{Request.Host}"; // build base URL for API
            var resp = await client.PostAsJsonAsync($"{baseUrl}/api/sessions", create); // send POST request
            // ---

            // Handle API errors
            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", $"API error: {(int)resp.StatusCode} {resp.ReasonPhrase}");
                return View(model);
            }

            var created = await resp.Content.ReadFromJsonAsync<SessionReadDto>();
            if (created == null)
            {
                ModelState.AddModelError("", "API returned empty response.");
                return View(model);
            }
            // ---

            // Redirect to Summary view to display created
            return View("Summary", created);
        }

        /// <summary>
        /// Display summary of created tutoring session.
        /// SessionReadDto is retrieved from the API.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Summary(SessionReadDto dto)
        {
            return View(dto);
        }
    }
}
