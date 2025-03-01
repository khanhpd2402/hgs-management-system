using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Exam
{
    public int ExamId { get; set; }

    public int SubjectId { get; set; }

    public int CreatedBy { get; set; }

    public string ExamContent { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual Teacher CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual Subject Subject { get; set; } = null!;
}
