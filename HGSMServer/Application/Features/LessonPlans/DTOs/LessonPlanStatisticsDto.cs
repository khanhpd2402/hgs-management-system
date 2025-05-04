using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LessonPlans.DTOs
{
    public class LessonPlanStatisticsDto
    {
        public int TotalLessonPlans { get; set; }
        public int SubmittedCount { get; set; }
        public int PendingCount { get; set; }
        public int WaitingForApprovalCount { get; set; }
        public int RejectedCount { get; set; }
    }
}
