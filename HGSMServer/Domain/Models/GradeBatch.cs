using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class GradeBatch
{
    public int BatchId { get; set; }

    public string BatchName { get; set; } = null!;

    public int SemesterId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual Semester Semester { get; set; } = null!;
}
