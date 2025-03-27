using Application.Features.Parents.DTOs;
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
        public DateOnly Dob { get; set; }
        public string Gender { get; set; } = null!;
        public string ClassName { get; set; } = null!;
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

        public List<ParentDto> Parents { get; set; } = new();
    }

}