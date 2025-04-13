using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.DTOs
{
    public class StudentClassFilterDto
    {
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? AcademicYearId { get; set; }
        public string StudentName { get; set; }
    }
}
