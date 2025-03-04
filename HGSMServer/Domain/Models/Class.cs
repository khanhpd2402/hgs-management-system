using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public int Grade { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<TeacherClass> TeacherClasses { get; set; } = new List<TeacherClass>();

    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
