using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class HomeroomAssignment
{
    public int HomeroomAssignmentId { get; set; }

    public int TeacherId { get; set; }

    public int ClassId { get; set; }

    public int SemesterId { get; set; }

    public string Status { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
