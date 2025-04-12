using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Periods.DTOs
{
    public class PeriodDto
    {
        public int PeriodId { get; set; }
        public string PeriodName { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public byte Shift { get; set; }
    }
}
