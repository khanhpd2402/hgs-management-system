using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.DTOs
{
    public class StudentDTO
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly Dob { get; set; }
        public string Gender { get; set; } = null!;
        public int GradeLevel { get; set; }
        public int ClassId { get; set; }
        public string? Status { get; set; }
    }
}
