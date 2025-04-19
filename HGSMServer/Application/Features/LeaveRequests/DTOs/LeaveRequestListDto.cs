using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LeaveRequests.DTOs
{
    public class LeaveRequestListDto
    {
        public int RequestId { get; set; }
        public int TeacherId { get; set; }
        public DateOnly RequestDate { get; set; }
        public string? Status { get; set; }
    }
}
