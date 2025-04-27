using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public int GradeLevelId { get; set; }

    public string? Status { get; set; }

    public virtual GradeLevel GradeLevel { get; set; } = null!;

    public virtual ICollection<HomeroomAssignment> HomeroomAssignments { get; set; } = new List<HomeroomAssignment>();

    public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();

    public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();

    public virtual ICollection<TimetableDetail> TimetableDetails { get; set; } = new List<TimetableDetail>();
}
