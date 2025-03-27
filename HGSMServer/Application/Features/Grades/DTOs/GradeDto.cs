namespace Application.Features.Grades.DTOs
{
    public class GradeDto
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int BatchId { get; set; }
        public string AssessmentsTypeName { get; set; }
        public string? Score { get; set; }
        public string? TeacherComment { get; set; }
    }
}
