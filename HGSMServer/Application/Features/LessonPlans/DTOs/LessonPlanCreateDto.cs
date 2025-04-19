using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LessonPlans.DTOs
{
    public class LessonPlanCreateDto
    {
        [Required]
        public int TeacherId { get; set; } 

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public int SemesterId { get; set; } 

        public DateTime? StartDate { get; set; } 

        public DateTime? EndDate { get; set; } 

        [StringLength(255)]
        public string? Title { get; set; } 

        public string? PlanContent { get; set; }
    }
}
