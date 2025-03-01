using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TeacherClass
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public int ClassId { get; set; }

    public bool? IsHomeroomTeacher { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
