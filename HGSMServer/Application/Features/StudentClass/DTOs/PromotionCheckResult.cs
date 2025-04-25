using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.DTOs
{
    public class PromotionCheckResult
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public int AcademicYearId { get; set; }
        public bool MustRepeat { get; set; }
        public List<string> Reasons { get; set; }
    }
}
