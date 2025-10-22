using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutoringSession.Data;
using TutoringSession.Dtos;
using TutoringSession.Models;

namespace TutoringSession.Controllers.Api
{
    [ApiController]
    [Route("api/sessions")]
    public class SessionsApiController : ControllerBase
    {
        private readonly TutoringDbContext _db;
        public SessionsApiController(TutoringDbContext db) => _db = db;

        /// <summary>
        /// Get all tutoring session records with calculated FeeAmount.
        /// </summary>
        /// <returns></returns>
        // GET: /api/sessions  (view results; not strictly required, but useful)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SessionReadDto>>> GetAll()
        {
            var data = await _db.Sessions
                .AsNoTracking()
                .OrderByDescending(s => s.SessionDate)
                .Select(s => new SessionReadDto
                {
                    Id = s.Id,
                    LecturerName = s.LecturerName,
                    StudentName = s.StudentName,
                    HoursTutored = s.HoursTutored,
                    HourlyRate = s.HourlyRate,
                    SessionDate = s.SessionDate,
                    FeeAmount = s.FeeAmount
                })
                .ToListAsync();

            return Ok(data);
        }

        /// <summary>
        /// Create a new tutoring session record with server-calculated FeeAmount
        /// and return the created record
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        // POST: /api/sessions  (server auto-calculates FeeAmount)
        [HttpPost]
        public async Task<ActionResult<SessionReadDto>> Create([FromBody] SessionCreateDto dto)
        {
            if (!ModelState.IsValid) 
                return ValidationProblem(ModelState);

            try
            {
                /*
                 * Create a new session record, calculating the FeeAmount on the server,
                 * and save record to the database
                 */
                var entity = new Session
                {
                    LecturerName = dto.LecturerName,
                    StudentName = dto.StudentName,
                    HoursTutored = dto.HoursTutored,
                    HourlyRate = dto.HourlyRate,
                    SessionDate = dto.SessionDate,
                    // Client never sets fee; server calculates it
                    FeeAmount = 0m
                };

                // Calculate the fee based on hours and rate
                entity.FeeAmount = entity.CalculateEarnings();

                // Save to database
                _db.Sessions.Add(entity);
                await _db.SaveChangesAsync();

                /*
                 * This block prepares the data to return to the client
                 */
                var read = new SessionReadDto
                {
                    Id = entity.Id,
                    LecturerName = entity.LecturerName,
                    StudentName = entity.StudentName,
                    HoursTutored = entity.HoursTutored,
                    HourlyRate = entity.HourlyRate,
                    SessionDate = entity.SessionDate,
                    FeeAmount = entity.FeeAmount
                };

                return Ok(read); // Return 200 OK with created record
            }
            catch (DbUpdateException ex)
            {
                return Problem(
                    title: "Database update failed",
                    detail: ex.InnerException?.Message ?? ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
