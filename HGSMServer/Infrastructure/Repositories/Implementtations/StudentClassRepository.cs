using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementtations
{
    public class StudentClassRepository : IStudentClassRepository
    {
        private readonly HgsdbContext _context;

        public StudentClassRepository(HgsdbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StudentClass>> GetByClassIdAsync(int classId)
        {
            return await _context.StudentClasses
                .Where(sc => sc.ClassId == classId)
                .Include(sc => sc.Student)
                .ToListAsync();
        }

        public async Task<StudentClass?> GetByIdAsync(int id)
        {
            return await _context.StudentClasses
                .Include(sc => sc.Student)
                .FirstOrDefaultAsync(sc => sc.Id == id);
        }

        public async Task<IEnumerable<StudentClass>> GetAllAsync()
        {
            return await _context.StudentClasses
                .Include(sc => sc.Student)
                .Include(sc => sc.Class)
                .ToListAsync();
        }

        public async Task<StudentClass> AddAsync(StudentClass entity)
        {
            _context.StudentClasses.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<StudentClass> UpdateAsync(StudentClass entity)
        {
            _context.StudentClasses.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.StudentClasses.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<StudentClass> GetStudentClassByStudentAndClassIdAsync(int studentId, int classId)
        {
            return await _context.StudentClasses
                                 .Where(sc => sc.StudentId == studentId && sc.ClassId == classId)
                                 .FirstOrDefaultAsync();
        }
        }
    }
