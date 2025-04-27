using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class GradeLevel
{
    public int GradeLevelId { get; set; }

    public string GradeName { get; set; } = null!;

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<GradeLevelSubject> GradeLevelSubjects { get; set; } = new List<GradeLevelSubject>();
}
