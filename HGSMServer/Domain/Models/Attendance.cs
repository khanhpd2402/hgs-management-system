using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int StudentId { get; set; }

    public DateOnly Date { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public string Shift { get; set; } = null!;

    public int SemesterId { get; set; }

    public virtual Teacher? CreatedByNavigation { get; set; }

    public virtual Semester Semester { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
