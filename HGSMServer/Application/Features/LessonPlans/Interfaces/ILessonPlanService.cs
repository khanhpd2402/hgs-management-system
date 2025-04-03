using Application.Features.LessonPlans.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Features.LessonPlans.Interfaces
{
    public interface ILessonPlanService
    {
        Task UploadLessonPlanAsync(LessonPlanUploadDto lessonPlanDto);
        Task ReviewLessonPlanAsync(LessonPlanReviewDto reviewDto);
        Task<List<LessonPlanResponseDto>> GetAllLessonPlansAsync(); 
        Task<LessonPlanResponseDto> GetLessonPlanByIdAsync(int planId); 
        Task<List<LessonPlanResponseDto>> GetLessonPlansByStatusAsync(string status);
        Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetAllLessonPlansAsync(int pageNumber, int pageSize);
        Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetLessonPlansByStatusAsync(string status, int pageNumber, int pageSize);
    }
}