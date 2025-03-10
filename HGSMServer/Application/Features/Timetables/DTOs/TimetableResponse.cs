namespace Application.Features.Timetables.DTOs
{
    public class TimetableResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public DateOnly EffectiveDate { get; set; }

        public Dictionary<string, TimetableDayResponse> Timetable { get; set; } = new();
    }

    public class TimetableDayResponse
    {
        public List<TimetablePeriodResponse> Morning { get; set; } = new();
        public List<TimetablePeriodResponse> Afternoon { get; set; } = new();
    }

    public class TimetablePeriodResponse
    {
        public int Period { get; set; }
        public string Subject { get; set; } = null!;
        public string Teacher { get; set; } = null!;
    }

}
