namespace Application.Features.Grades.DTOs
{
    public class GradeDto
    {
        public int? GradeId { get; set; } 
        public int BatchId { get; set; } 
        public int StudentId { get; set; } 
        public int ClassId { get; set; } 
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public string AssessmentsTypeName { get; set; }

        public string? Score { get; set; }
        public string? TeacherComment { get; set; }
    }
}
