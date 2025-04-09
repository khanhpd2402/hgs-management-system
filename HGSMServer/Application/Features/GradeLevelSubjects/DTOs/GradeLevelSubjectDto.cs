using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeLevelSubjects.DTOs
{
    public class GradeLevelSubjectDto
    {
        public int GradeLevelSubjectId { get; set; }
        public int GradeLevelId { get; set; }
        public string GradeLevelName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int PeriodsPerWeekHKI { get; set; }
        public int PeriodsPerWeekHKII { get; set; }
        public int ContinuousAssessmentsHKI { get; set; }
        public int ContinuousAssessmentsHKII { get; set; }
        public int MidtermAssessments { get; set; }
        public int FinalAssessments { get; set; }
    }
}
