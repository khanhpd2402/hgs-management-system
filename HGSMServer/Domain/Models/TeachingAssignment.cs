using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TeachingAssignment
{
    public int AssignmentId { get; set; }

    public int TeacherId { get; set; }

    public int SubjectId { get; set; }

    public int ClassId { get; set; }

    public byte Semester { get; set; }

    public string AcademicYear { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
