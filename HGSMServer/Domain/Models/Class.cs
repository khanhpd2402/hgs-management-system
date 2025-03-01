using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public int Grade { get; set; }

    public int? HomeroomTeacherId { get; set; }

    public virtual Teacher? HomeroomTeacher { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
