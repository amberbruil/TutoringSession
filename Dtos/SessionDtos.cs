using System.ComponentModel.DataAnnotations;

namespace TutoringSession.Dtos
{
    public class SessionCreateDto
    {
        [Required, StringLength(60)]
        public string LecturerName { get; set; } = string.Empty;

        [Required, StringLength(60)]
        public string StudentName { get; set; } = string.Empty;

        [Range(0.5, 12)]
        public double HoursTutored { get; set; }

        [Range(50, 200)]
        public decimal HourlyRate { get; set; }

        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; }
    }

    public class SessionReadDto
    {
        public int Id { get; set; }
        public string LecturerName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public double HoursTutored { get; set; }
        public decimal HourlyRate { get; set; }
        public DateTime SessionDate { get; set; }
        public decimal FeeAmount { get; set; }
    }
}
