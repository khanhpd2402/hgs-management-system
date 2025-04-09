using Domain.Models;
//using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class LessonPlanRepository : ILessonPlanRepository
    {
        private readonly HgsdbContext _context;

        public LessonPlanRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task AddLessonPlanAsync(LessonPlan lessonPlan)
        {
            await _context.LessonPlans.AddAsync(lessonPlan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLessonPlanAsync(LessonPlan lessonPlan)
        {
            _context.LessonPlans.Update(lessonPlan);
            await _context.SaveChangesAsync();
        }

        public async Task<LessonPlan> GetLessonPlanByIdAsync(int planId)
        {
            return await _context.LessonPlans
                .Include(lp => lp.Teacher) 
                .Include(lp => lp.Subject) 
                .FirstOrDefaultAsync(lp => lp.PlanId == planId);
        }

        public async Task<List<LessonPlan>> GetAllLessonPlansAsync()
        {
            return await _context.LessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Subject)
                .ToListAsync();
        }

        public async Task<List<LessonPlan>> GetLessonPlansByStatusAsync(string status)
        {
            return await _context.LessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Subject)
                .Where(lp => lp.Status == status)
                .ToListAsync();
        }
        public async Task<(List<LessonPlan> LessonPlans, int TotalCount)> GetAllLessonPlansAsync(int pageNumber, int pageSize)
        {
            var query = _context.LessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Subject)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var lessonPlans = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (lessonPlans, totalCount);
        }

        public async Task<(List<LessonPlan> LessonPlans, int TotalCount)> GetLessonPlansByStatusAsync(string status, int pageNumber, int pageSize)
        {
            var query = _context.LessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Subject)
                .Where(lp => lp.Status == status)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var lessonPlans = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (lessonPlans, totalCount);
        }
    }
}