using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Timetables.DTOs
{
    public class TimetableDto
    {
        public int TimetableId { get; set; }
        public int SemesterId { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public string Status { get; set; } // Trạng thái của thời khóa biểu
        public List<TimetableDetailDto> Details { get; set; } = new();
    }

    public class TimetableDetailDto
    {
        public int TimetableDetailId { get; set; }
        public int TimetableId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public byte DayOfWeek { get; set; }
        public int Shift { get; set; }
        public int Period { get; set; }
    }

}
