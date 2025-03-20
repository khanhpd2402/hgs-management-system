using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LeaveRequests.DTOs
{
    public class LeaveRequestDto
    {
        public int RequestId { get; set; }
        public int TeacherId { get; set; }
        public DateOnly RequestDate { get; set; }
        public DateOnly LeaveFromDate { get; set; }
        public DateOnly LeaveToDate { get; set; }
        public string Reason { get; set; } = null!;
        public string? Status { get; set; }
    }

}
