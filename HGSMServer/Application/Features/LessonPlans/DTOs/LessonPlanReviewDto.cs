using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LessonPlans.DTOs
{
    public class LessonPlanReviewDto
    {
        public int PlanId { get; set; }
        public string Status { get; set; } // "Approved" or "Rejected"
        public string Feedback { get; set; }
    }
}
