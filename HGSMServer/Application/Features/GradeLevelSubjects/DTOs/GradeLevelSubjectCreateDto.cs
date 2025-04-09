
namespace Application.Features.GradeLevelSubjects.DTOs
{
    public class GradeLevelSubjectCreateAndUpdateDto
    {
        public int GradeLevelId { get; set; }
        public int SubjectId { get; set; }
        public int PeriodsPerWeekHKI { get; set; }
        public int PeriodsPerWeekHKII { get; set; }
        public int ContinuousAssessmentsHKI { get; set; }
        public int ContinuousAssessmentsHKII { get; set; }
        public int MidtermAssessments { get; set; }
        public int FinalAssessments { get; set; }
    }
}
