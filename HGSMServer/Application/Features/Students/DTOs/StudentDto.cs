using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.DTOs
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly Dob { get; set; }  // Ngày sinh
        public string Gender { get; set; } = null!;
        public int ClassId { get; set; }
        public DateOnly AdmissionDate { get; set; }  // Ngày nhập học
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
