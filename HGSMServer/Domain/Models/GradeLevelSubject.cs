using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class GradeLevelSubject
{
    public int GradeLevelSubjectId { get; set; }

    public int GradeLevelId { get; set; }

    public int SubjectId { get; set; }

    public int PeriodsPerWeekHki { get; set; }

    public int PeriodsPerWeekHkii { get; set; }

    public int ContinuousAssessmentsHki { get; set; }

    public int ContinuousAssessmentsHkii { get; set; }

    public int MidtermAssessments { get; set; }

    public int FinalAssessments { get; set; }

    public virtual GradeLevel GradeLevel { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
