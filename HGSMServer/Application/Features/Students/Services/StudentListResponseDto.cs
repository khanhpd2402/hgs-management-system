using Application.Features.Students.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.Services
{
    public class StudentListResponseDto
    {
        public List<StudentDto> Students { get; set; } = new List<StudentDto>();
        public int TotalCount { get; set; }
    }
}
