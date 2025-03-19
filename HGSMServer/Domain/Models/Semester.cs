using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Semester
{
    public int SemesterId { get; set; }

    public int AcademicYearId { get; set; }

    public string SemesterName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual AcademicYear AcademicYear { get; set; } = null!;

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<GradeBatch> GradeBatches { get; set; } = new List<GradeBatch>();

    public virtual ICollection<LessonPlan> LessonPlans { get; set; } = new List<LessonPlan>();

    public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();

    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
