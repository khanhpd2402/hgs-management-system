using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.DTOs
{
    public class StudentClassByLastAcademicYearResponseDto
    {
        public IEnumerable<StudentClassResponseDto> Students { get; set; } = new List<StudentClassResponseDto>();
        public int EligibleForPromotionCount { get; set; }
    }
}
