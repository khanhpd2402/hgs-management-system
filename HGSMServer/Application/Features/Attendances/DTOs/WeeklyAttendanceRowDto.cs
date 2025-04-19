using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Attendances.DTOs
{
    public class WeeklyAttendanceRowDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public List<DayAttendanceDto> DailySessions { get; set; }
    }

    public class DayAttendanceDto
    {
        public DateTime Date { get; set; }
        public string MorningStatus { get; set; }
        public string AfternoonStatus { get; set; }
    }
}
