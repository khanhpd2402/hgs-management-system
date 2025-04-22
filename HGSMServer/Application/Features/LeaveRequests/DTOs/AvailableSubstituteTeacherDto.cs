using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LeaveRequests.DTOs
{
    namespace Application.Features.LeaveRequests.DTOs
    {
        public class AvailableSubstituteTeacherDto
        {
            public int TeacherId { get; set; }
            public string FullName { get; set; } = string.Empty;
        }

        public class FindSubstituteTeacherRequestDto
        {
            public int OriginalTeacherId { get; set; }
            public int TimetableDetailId { get; set; }  
        }
    }
}
