using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Attendances.DTOs
{
    public class AttendanceRequest
    {
        public int StudentID { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; } // 'C', 'P', 'K', 'X'
        public string Note { get; set; }
        public string Shift { get; set; }
    }
}
