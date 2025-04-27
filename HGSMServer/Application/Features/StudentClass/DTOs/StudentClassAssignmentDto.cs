using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.DTOs
{
    public class StudentClassAssignmentDto
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int? AcademicYearId { get; set; }
    }
}
