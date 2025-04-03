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

    public byte DayOfWeek { get; set; }

    public byte Shift { get; set; }

    public byte Period { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;

    public virtual Timetable Timetable { get; set; } = null!;
}
