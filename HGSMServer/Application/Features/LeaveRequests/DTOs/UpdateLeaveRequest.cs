using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LeaveRequests.DTOs
{
    public class UpdateLeaveRequest
    {
        public int RequestId { get; set; }
        public string? Comment { get; set; }
        public string? Status { get; set; }
    }
}
