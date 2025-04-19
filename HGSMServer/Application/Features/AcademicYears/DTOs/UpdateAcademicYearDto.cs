using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AcademicYears.DTOs
{
    public class UpdateAcademicYearDto
    {
        public int AcademicYearId { get; set; } 
        public string YearName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateOnly Semester1StartDate { get; set; }
        public DateOnly Semester1EndDate { get; set; }
        public DateOnly Semester2StartDate { get; set; }
        public DateOnly Semester2EndDate { get; set; }
    }
}
