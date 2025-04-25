namespace Application.Features.Grades.DTOs
{
    public class GradeRespondDto
    {
        public int GradeId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string SubjectName { get; set; }
        public string Score { get; set; }
        public string AssessmentType { get; set; }
        public string TeacherComment { get; set; }
        public string TeacherName { get; set; }
    }
}
