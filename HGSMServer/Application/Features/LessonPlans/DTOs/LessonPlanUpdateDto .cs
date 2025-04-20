using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LessonPlans.DTOs
{
    public class LessonPlanUpdateDto
    {
        [Required]
        public string PlanContent { get; set; }

        [StringLength(255)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? AttachmentUrl { get; set; }
    }
}
