using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Teachers.DTOs
{
    public class TeacherListDto
    {
        public int TeacherId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? EmploymentStatus { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

}
