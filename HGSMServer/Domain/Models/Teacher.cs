using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public int? UserId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Gender { get; set; } = null!;

    public string? Ethnicity { get; set; }

    public string? Religion { get; set; }

    public string? MaritalStatus { get; set; }

    public string? IdcardNumber { get; set; }

    public string? InsuranceNumber { get; set; }

    public string? EmploymentType { get; set; }

    public string? Position { get; set; }

    public string? Department { get; set; }

    public string? AdditionalDuties { get; set; }

    public bool? IsHeadOfDepartment { get; set; }

    public string? EmploymentStatus { get; set; }

    public string? RecruitmentAgency { get; set; }

    public DateOnly? HiringDate { get; set; }

    public DateOnly? PermanentEmploymentDate { get; set; }

    public DateOnly SchoolJoinDate { get; set; }

    public string? PermanentAddress { get; set; }

    public string? Hometown { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<LessonPlan> LessonPlans { get; set; } = new List<LessonPlan>();

    public virtual ICollection<TeacherClass> TeacherClasses { get; set; } = new List<TeacherClass>();

    public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();

    public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();

    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();

    public virtual User? User { get; set; }
}
