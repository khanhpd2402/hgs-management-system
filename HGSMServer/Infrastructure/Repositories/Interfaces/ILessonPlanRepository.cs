using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ILessonPlanRepository
    {
        Task AddLessonPlanAsync(LessonPlan lessonPlan);
        Task<LessonPlan> GetLessonPlanByIdAsync(int planId);
        Task UpdateLessonPlanAsync(LessonPlan lessonPlan);
    }
}
