using System.ComponentModel.DataAnnotations;

namespace TutoringSession.Models
{
    public class Session
    {
        public int Id { get; set; }

        /* Data annotations: provides declarative validation for user input.
         * MVC uses these attributes during model binding to build ModelState.
         * If invalid, errors appear next to fields using Razor Tag Helpers.
         */
        [Required, StringLength(60)]
        public string LecturerName { get; set; } = string.Empty;

        [Required, StringLength(60)]
        public string StudentName { get; set; } = string.Empty;

        [Range(0.5, 12, ErrorMessage = "Hours must be between 0.5 and 12.")]    
        public double HoursTutored { get; set; }

        [Range(50, 200, ErrorMessage = "Hourly rate must be between 50 and 200.")]
        public decimal HourlyRate { get; set; }

        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; } = DateTime.Today;

        public decimal CalculateEarnings() => (decimal)HoursTutored * HourlyRate;

        // Calculated by the API, not from the client
        public decimal FeeAmount { get; set; }
    }
}