using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TimetableDetail
{
    public int TimetableDetailId { get; set; }

    public int TimetableId { get; set; }

    public int ClassId { get; set; }

    public int SubjectId { get; set; }

    public int TeacherId { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public int PeriodId { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Class Class { get; set; } = null!;

    public virtual Period Period { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;

    public virtual Timetable Timetable { get; set; } = null!;
}
