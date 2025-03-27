using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LessonPlans.DTOs
{
    public class LessonPlanResponseDto
    {
        public int PlanId { get; set; }
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public string PlanContent { get; set; }
        public string Status { get; set; }
        public int SemesterId { get; set; }
        public string Feedback { get; set; } // Thêm nếu bảng được cập nhật
        public DateTime? SubmittedDate { get; set; } // Thêm nếu bảng được cập nhật
        public DateTime? ReviewedDate { get; set; } // Thêm nếu bảng được cập nhật
        public int? ReviewerId { get; set; } // Thêm nếu bảng được cập nhật
    }
}
