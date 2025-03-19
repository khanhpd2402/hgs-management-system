using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.DTOs
{
    public class UpdateStudentDto
    {
        public int StudentId { get; set; } // Bắt buộc cần ID khi update
        public string FullName { get; set; } = null!;
        public DateOnly Dob { get; set; }
        public string Gender { get; set; } = null!;
        public int ClassId { get; set; }
        public int Grade { get; set; }
        public DateOnly AdmissionDate { get; set; }
        public string? EnrollmentType { get; set; }
        public string? Ethnicity { get; set; }
        public string? PermanentAddress { get; set; }
        public string? BirthPlace { get; set; }
        public string? Religion { get; set; }
        public bool? RepeatingYear { get; set; }
        public string? IdcardNumber { get; set; }
        public string? Status { get; set; }
    }
}
