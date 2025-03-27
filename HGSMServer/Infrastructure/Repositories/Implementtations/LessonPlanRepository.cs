using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class LessonPlanRepository : ILessonPlanRepository
    {
        private readonly HgsdbContext _context;

        public LessonPlanRepository(HgsdbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddLessonPlanAsync(LessonPlan lessonPlan)
        {
            await _context.LessonPlans.AddAsync(lessonPlan);
            await _context.SaveChangesAsync();
        }

        public async Task<LessonPlan> GetLessonPlanByIdAsync(int planId)
        {
            return await _context.LessonPlans.FindAsync(planId);
        }

        public async Task UpdateLessonPlanAsync(LessonPlan lessonPlan)
        {
            _context.LessonPlans.Update(lessonPlan);
            await _context.SaveChangesAsync();
        }
    }
}