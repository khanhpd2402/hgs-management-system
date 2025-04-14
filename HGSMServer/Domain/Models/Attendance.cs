using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int StudentId { get; set; }

    public int TimetableDetailId { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public DateOnly Date { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual TimetableDetail TimetableDetail { get; set; } = null!;
}
