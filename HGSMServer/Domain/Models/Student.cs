using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string FullName { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string Gender { get; set; } = null!;

    public int ClassId { get; set; }

    public DateTime AdmissionDate { get; set; }

    public string? EnrollmentType { get; set; }

    public string? Ethnicity { get; set; }

    public string? PermanentAddress { get; set; }

    public string? BirthPlace { get; set; }

    public string? Religion { get; set; }

    public bool? RepeatingYear { get; set; }

    public string? IdcardNumber { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Parent> Parents { get; set; } = new List<Parent>();
}
