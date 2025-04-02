using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public string SubjectCategory { get; set; } = null!;

    public string TypeOfGrade { get; set; } = null!;

    public virtual ICollection<ExamProposal> ExamProposals { get; set; } = new List<ExamProposal>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<LessonPlan> LessonPlans { get; set; } = new List<LessonPlan>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();

    public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();

    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
