using Common.Constants;

namespace Application.Features.Timetables.DTOs
{
    public class CreateTimetableDto
    {
        public int SemesterId { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public string Status { get; set; } = AppConstants.Status.PENDING;
        public List<TimetableDetailCreateDto> Details { get; set; }
    }

    public class TimetableDetailCreateDto
    {
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public DateOnly Date { get; set; }
        public int PeriodId { get; set; }
    }

}
