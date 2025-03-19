using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Teachers.DTOs
{
    public class AssignHomeroomDto
    {
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public int AcademicYearId { get; set; }
        public int SemesterId { get; set; }
    }
}
