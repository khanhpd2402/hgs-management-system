using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int? BatchId { get; set; }

    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public int SubjectId { get; set; }

    public string? Score { get; set; }

    public string AssessmentsTypeName { get; set; } = null!;

    public string? TeacherComment { get; set; }

    public virtual GradeBatch? Batch { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
