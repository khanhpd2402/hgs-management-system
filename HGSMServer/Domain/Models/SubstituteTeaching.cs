using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class SubstituteTeaching
{
    public int SubstituteId { get; set; }

    public int TimetableDetailId { get; set; }

    public int OriginalTeacherId { get; set; }

    public int SubstituteTeacherId { get; set; }

    public DateOnly Date { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Teacher OriginalTeacher { get; set; } = null!;

    public virtual Teacher SubstituteTeacher { get; set; } = null!;

    public virtual TimetableDetail TimetableDetail { get; set; } = null!;
}
