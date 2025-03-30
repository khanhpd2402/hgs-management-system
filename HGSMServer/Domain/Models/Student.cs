using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly AdmissionDate { get; set; }

    public string? EnrollmentType { get; set; }

    public string? Ethnicity { get; set; }

    public string? PermanentAddress { get; set; }

    public string? BirthPlace { get; set; }

    public string? Religion { get; set; }

    public bool? RepeatingYear { get; set; }

    public string? IdcardNumber { get; set; }

    public string? Status { get; set; }

    public int? ParentId { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual Parent? Parent { get; set; }

    public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
}
