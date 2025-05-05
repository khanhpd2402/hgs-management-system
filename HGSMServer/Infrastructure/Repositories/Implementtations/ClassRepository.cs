using Common.Constants;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Common.Constants.AppConstants;

namespace Infrastructure.Repositories.Implementtations
{
    public class ClassRepository : IClassRepository
    {
        private readonly HgsdbContext _context;

        public ClassRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Class>> GetAllAsync()
        {
            return await _context.Classes
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.Student)
                .Include(c => c.TeachingAssignments)
                //.Include(c => c.TimetableDetails)
                    //.ThenInclude(td => td.Timetable)
                .ToListAsync();
        }
        public async Task<IEnumerable<Class>> GetAllActiveAsync(string? status = null)
        {
            var statusFilter = string.IsNullOrEmpty(status) ? Status.ACTIVE : status;

            var query = _context.Classes
                .Where(c => c.Status == statusFilter)
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.Student)
                .Include(c => c.TeachingAssignments)
                .Include(c => c.TimetableDetails)
                    .ThenInclude(td => td.Timetable);

            return await query.ToListAsync();
        }


        public async Task<Class> GetByIdAsync(int id)
        {
            return await _context.Classes
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.Student)
                .Include(c => c.TeachingAssignments)
                .Include(c => c.TimetableDetails)
                    .ThenInclude(td => td.Timetable)
                .FirstOrDefaultAsync(c => c.ClassId == id)
                ?? throw new Exception("Class not found");
        }

        public async Task<Class> AddAsync(Class classEntity)
        {
            _context.Classes.Add(classEntity);
            await _context.SaveChangesAsync();
            return classEntity;
        }

        public async Task<Class> UpdateAsync(Class classEntity)
        {
            _context.Classes.Update(classEntity);
            await _context.SaveChangesAsync();
            return classEntity;
        }

        public async Task DeleteAsync(int id)
        {
            var classEntity = await GetByIdAsync(id);
            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(int classId)
        {
            return await _context.Classes.AnyAsync(c => c.ClassId == classId);
        }
        public async Task<Class> GetClassByNameAsync(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return null;
            }
            string normalizedClassName = className.Trim();
            return await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassName.ToLower() == normalizedClassName.ToLower());
        }
        public async Task<Class> GetByIdWithoutTimetableAsync(int id)
        {
            return await _context.Classes
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.Student)
                .Include(c => c.TeachingAssignments)
                .FirstOrDefaultAsync(c => c.ClassId == id)
                ?? throw new Exception("Class not found");
        }
    }
}
