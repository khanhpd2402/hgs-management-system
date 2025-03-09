using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Timetables.DTOs
{
    public class TimetableCreateDto
    {
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public string DayOfWeek { get; set; } = null!;
        public string Shift { get; set; } = null!;
        public int Period { get; set; }
        public DateOnly EffectiveDate { get; set; }
    }

    public class TimetableUpdateDto : TimetableCreateDto
    {
        public int TimetableId { get; set; }
    }


}
