using Domain.Models;
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

        // --- Implement các hàm mới ---
        public async Task<LessonPlan> GetLessonPlanByIdIncludingDetailsAsync(int planId)
        {
            return await _context.LessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Subject)
                .Include(lp => lp.Reviewer) 
                .FirstOrDefaultAsync(lp => lp.PlanId == planId);
        }

        private IQueryable<LessonPlan> GetQueryWithIncludes()
        {
            return _context.LessonPlans
               .Include(lp => lp.Teacher)
               .Include(lp => lp.Subject)
               .Include(lp => lp.Reviewer) 
               .AsNoTracking() 
               .AsQueryable();
        }

        public async Task<(List<LessonPlan> LessonPlans, int TotalCount)> GetAllLessonPlansIncludingDetailsAsync(int pageNumber, int pageSize)
        {
            var query = GetQueryWithIncludes();
            var totalCount = await query.CountAsync();
            var lessonPlans = await query
                .OrderByDescending(lp => lp.PlanId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (lessonPlans, totalCount);
        }

        public async Task<(List<LessonPlan> LessonPlans, int TotalCount)> GetLessonPlansByTeacherIncludingDetailsAsync(int teacherId, int pageNumber, int pageSize)
        {
            var query = GetQueryWithIncludes().Where(lp => lp.TeacherId == teacherId);
            var totalCount = await query.CountAsync();
            var lessonPlans = await query
               .OrderByDescending(lp => lp.PlanId)
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
            return (lessonPlans, totalCount);
        }

        public async Task<(List<LessonPlan> LessonPlans, int TotalCount)> GetLessonPlansByStatusIncludingDetailsAsync(string status, int pageNumber, int pageSize)
        {
            var query = GetQueryWithIncludes().Where(lp => lp.Status == status);
            var totalCount = await query.CountAsync();
            var lessonPlans = await query
               .OrderByDescending(lp => lp.PlanId)
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
            return (lessonPlans, totalCount);
        }

        public async Task<IEnumerable<LessonPlan>> GetAll()
        {
            return await GetQueryWithIncludes()
                .OrderByDescending(lp => lp.PlanId)
                .ToListAsync();
        }
        public async Task<List<LessonPlan>> GetLessonPlansByTeacherIdsAsync(List<int> teacherIds)
        {
            return await _context.LessonPlans
                .Where(lp => teacherIds.Contains(lp.TeacherId))
                .ToListAsync();
        }
    }
}