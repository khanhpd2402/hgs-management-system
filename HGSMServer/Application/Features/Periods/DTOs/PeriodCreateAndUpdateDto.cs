using System.ComponentModel.DataAnnotations;

namespace Application.Features.Periods.DTOs
{
    public class PeriodCreateAndUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string PeriodName { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Shift must be 1 (Morning) or 2 (Afternoon)")]
        public byte Shift { get; set; }
    }
}
