using Application.Features.Parents.DTOs;

namespace Application.Features.Students.DTOs
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly Dob { get; set; }
        public string Gender { get; set; } = null!;
        public string ClassName { get; set; } = null!;
        public int GradeId { get; set; }
        public string GradeName { get; set; } = null!;
        public DateOnly AdmissionDate { get; set; }
        public string? EnrollmentType { get; set; }
        public string? Ethnicity { get; set; }
        public string? PermanentAddress { get; set; }
        public string? BirthPlace { get; set; }
        public string? Religion { get; set; }
        public string? IdcardNumber { get; set; }
        public string? Status { get; set; }

        
        public ParentDto? Parent { get; set; }
    }
}