using System;

namespace Application.Features.LessonPlans.DTOs
{
    public class LessonPlanResponseDto
    {
        public int PlanId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } 
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } 
        public string PlanContent { get; set; }
        public string? AttachmentUrl { get; set; }
        public string Status { get; set; }
        public int SemesterId { get; set; }
        public string Feedback { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public int? ReviewerId { get; set; }
        public string ReviewerName { get; set; } 
    }
}