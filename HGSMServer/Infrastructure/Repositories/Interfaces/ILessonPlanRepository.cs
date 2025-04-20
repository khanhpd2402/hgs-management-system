using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ILessonPlanRepository
    {
        Task AddLessonPlanAsync(LessonPlan lessonPlan);
        Task UpdateLessonPlanAsync(LessonPlan lessonPlan);
        Task<LessonPlan> GetLessonPlanByIdAsync(int planId);
        Task<LessonPlan> GetLessonPlanByIdIncludingDetailsAsync(int planId);
        Task<(List<LessonPlan> LessonPlans, int TotalCount)> GetAllLessonPlansIncludingDetailsAsync(int pageNumber, int pageSize);
        Task<(List<LessonPlan> LessonPlans, int TotalCount)> GetLessonPlansByTeacherIncludingDetailsAsync(int teacherId, int pageNumber, int pageSize);
        Task<(List<LessonPlan> LessonPlans, int TotalCount)> GetLessonPlansByStatusIncludingDetailsAsync(string status, int pageNumber, int pageSize);

        // Task<List<LessonPlan>> GetAllLessonPlansAsync();
        // Task<List<LessonPlan>> GetLessonPlansByStatusAsync(string status);
        
    }
}