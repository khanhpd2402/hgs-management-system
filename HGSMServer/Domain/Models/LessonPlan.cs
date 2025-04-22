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

    public string? Title { get; set; }

    public string? AttachmentUrl { get; set; }

    public string? Feedback { get; set; }

    public DateTime? SubmittedDate { get; set; }

    public DateTime? ReviewedDate { get; set; }

    public int? ReviewerId { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? Startdate { get; set; }

    public virtual Teacher? Reviewer { get; set; }

    public virtual Semester Semester { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
