// Application/Features/LessonPlans/Interfaces/ILessonPlanService.cs
using Application.Features.LessonPlans.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Features.LessonPlans.Interfaces
{
    public interface ILessonPlanService
    {
        Task<LessonPlanResponseDto> CreateLessonPlanAsync(LessonPlanCreateDto createDto);
        Task UpdateMyLessonPlanAsync(int planId, LessonPlanUpdateDto updateDto);
        Task ReviewLessonPlanAsync(LessonPlanReviewDto reviewDto);
        Task<LessonPlanResponseDto> GetLessonPlanByIdAsync(int planId);
        Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetAllLessonPlansAsync(int pageNumber, int pageSize);
        Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetLessonPlansByTeacherAsync(int teacherId, int pageNumber, int pageSize);
        Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetLessonPlansByStatusAsync(string status, int pageNumber, int pageSize);
        Task<LessonPlanStatisticsDto> GetDepartmentHeadLessonPlanStatisticsAsync();
    }
}