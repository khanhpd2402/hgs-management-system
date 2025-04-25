using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.DTOs
{
    public class StudentDtoWithGradeLevel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public DateOnly DOB { get; set; }
        public string Gender { get; set; }
        public DateOnly AdmissionDate { get; set; }
        public string? EnrollmentType { get; set; }
        public string? Ethnicity { get; set; }
        public string? PermanentAddress { get; set; }
        public string? BirthPlace { get; set; }
        public string? Religion { get; set; }
        public bool? RepeatingYear { get; set; }
        public string? IDCardNumber { get; set; }
        public string? Status { get; set; }
        public int? ParentID { get; set; }
        public string? ParentFullName { get; set; } 
        public int GradeLevelId { get; set; } 
        public string ClassName { get; set; }
    }
}
