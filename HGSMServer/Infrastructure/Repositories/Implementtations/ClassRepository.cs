using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                .Include(c => c.TeacherClasses)
                .Include(c => c.TeachingAssignments)
                .Include(c => c.TimetableDetails) // ← đúng
                    .ThenInclude(td => td.Timetable)
                .ToListAsync();
        }

        public async Task<Class> GetByIdAsync(int id)
        {
            return await _context.Classes
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.Student)
                .Include(c => c.TeacherClasses)
                .Include(c => c.TeachingAssignments)
                .Include(c => c.TimetableDetails) // ← đúng
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
    }
}
