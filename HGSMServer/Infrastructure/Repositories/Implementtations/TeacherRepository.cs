using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementtations
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly HgsdbContext _context;

        public TeacherRepository(HgsdbContext context)
        {
            _context = context;
        }

        public IQueryable<Teacher> GetAll()
        {
            return _context.Teachers.Include(t => t.User).AsQueryable();
        }

        public async Task<Teacher?> GetByIdAsync(int id)
        {

            return await _context.Teachers.Include(t => t.User)
                                          .FirstOrDefaultAsync(t => t.TeacherId == id);
        }

        public async Task AddAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(string idCard, string insuranceNumber)
        {
            return await _context.Teachers.AnyAsync(t => t.IdcardNumber == idCard || t.InsuranceNumber == insuranceNumber);
        }

        public async Task AddRangeAsync(IEnumerable<Teacher> teachers)
        {
            await _context.Teachers.AddRangeAsync(teachers);
            await _context.SaveChangesAsync();
        }
        public async Task<Teacher> GetByUserIdAsync(int userId)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }
        public async Task<IEnumerable<TeacherSubject>> GetTeacherSubjectsAsync(int teacherId)
        {
            return await _context.TeacherSubjects
                .Where(ts => ts.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task DeleteTeacherSubjectsAsync(int teacherId)
        {
            var subjects = await _context.TeacherSubjects
                .Where(ts => ts.TeacherId == teacherId)
                .ToListAsync();

            _context.TeacherSubjects.RemoveRange(subjects);
            await _context.SaveChangesAsync();
        }
    }
}
