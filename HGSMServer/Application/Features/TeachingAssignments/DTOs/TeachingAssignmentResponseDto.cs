namespace Application.Features.TeachingAssignments.DTOs
{
    public class TeachingAssignmentResponseDto
    {
        public int AssignmentId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int SemesterId { get; set; }
        public string SemesterName { get; set; }
    }
}