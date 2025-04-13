using Application.Features.AcademicYears.DTOs;
using Application.Features.Classes.DTOs;
using Application.Features.Students.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.DTOs
{
    public class StudentClassFilterDataDto
    {
        public List<StudentFilterDto> Students { get; set; }
        public List<ClassDto> Classes { get; set; }
        public List<AcademicYearDto> AcademicYears { get; set; }
    }
}
