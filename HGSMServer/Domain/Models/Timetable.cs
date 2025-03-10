namespace Domain.Models;

public partial class Timetable
{
    public int TimetableId { get; set; }

    public int ClassId { get; set; }

    public int SubjectId { get; set; }

    public int TeacherId { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public string Shift { get; set; } = null!;

    public int Period { get; set; }

    public string SchoolYear { get; set; } = null!;

    public int Semester { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
