using Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Timetables.DTOs
{
    public class CreateTimetableDto
    {
        [Required(ErrorMessage = "SemesterId is required.")]
        public int SemesterId { get; set; }

        [Required(ErrorMessage = "EffectiveDate is required.")]
        public DateOnly EffectiveDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public string Status { get; set; } = AppConstants.Status.PENDING;

        public List<TimetableDetailCreateDto> Details { get; set; }
    }

    public class TimetableDetailCreateDto
    {
        public int ClassId { get; set; }

        public int SubjectId { get; set; }

        public int TeacherId { get; set; }

        public string DayOfWeek { get; set; }

        public int PeriodId { get; set; }
    }
}
