using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.DTOs
{
    public class StudentListResponseDto
    {
        public List<StudentDto> Students { get; set; } = new List<StudentDto>();
        public int TotalCount { get; set; }
    }
}
