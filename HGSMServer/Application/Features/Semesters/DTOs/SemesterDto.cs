using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Semesters.DTOs
{
    public class SemesterDto
    {
        public int SemesterID { get; set; }
        public int AcademicYearID { get; set; }
        public string SemesterName { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
