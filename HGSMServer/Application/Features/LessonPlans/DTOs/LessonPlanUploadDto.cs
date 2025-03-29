using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LessonPlans.DTOs
{
    public class LessonPlanUploadDto
    {
        //public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public string PlanContent { get; set; }
        public int SemesterId { get; set; }
        public string? Title { get; set; } 
        public string? AttachmentUrl { get; set; }
    }
}
