using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LeaveRequests.DTOs
{
    public class CreateLeaveRequestDto
    {
        public int TeacherId { get; set; }
        public DateOnly LeaveFromDate { get; set; }
        public DateOnly LeaveToDate { get; set; }
        public string Reason { get; set; } = null!;
    }

}
