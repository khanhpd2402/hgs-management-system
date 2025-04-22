using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int StudentClassId { get; set; }

    public DateOnly Date { get; set; }

    public string Session { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual StudentClass StudentClass { get; set; } = null!;
}
