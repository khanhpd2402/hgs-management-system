using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int? BatchId { get; set; }

    public int? StudentClassId { get; set; }

    public int? AssignmentId { get; set; }

    public string? Score { get; set; }

    public string AssessmentsTypeName { get; set; } = null!;

    public string? TeacherComment { get; set; }

    public virtual TeachingAssignment? Assignment { get; set; }

    public virtual GradeBatch? Batch { get; set; }

    public virtual StudentClass? StudentClass { get; set; }
}
