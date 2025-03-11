using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class LessonPlan
{
    public int PlanId { get; set; }

    public int TeacherId { get; set; }

    public int SubjectId { get; set; }

    public string PlanContent { get; set; } = null!;

    public string? Status { get; set; }

    public int SemesterId { get; set; }

    public virtual Semester Semester { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
