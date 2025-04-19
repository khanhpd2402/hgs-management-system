using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TeacherSubjectRepository : ITeacherSubjectRepository
    {
        private readonly HgsdbContext _context;

        public TeacherSubjectRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<List<TeacherSubject>> GetAllAsync()
        {
            return await _context.TeacherSubjects
                .Include(ts => ts.Teacher)
                .Include(ts => ts.Subject)
                .ToListAsync();
        }

        public async Task<TeacherSubject> GetByIdAsync(int id)
        {
            return await _context.TeacherSubjects
                .Include(ts => ts.Teacher)
                .Include(ts => ts.Subject)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }

        public async Task AddAsync(TeacherSubject teacherSubject)
        {
            await _context.TeacherSubjects.AddAsync(teacherSubject);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TeacherSubject teacherSubject)
        {
            _context.TeacherSubjects.Update(teacherSubject);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var teacherSubject = await _context.TeacherSubjects.FindAsync(id);
            if (teacherSubject != null)
            {
                _context.TeacherSubjects.Remove(teacherSubject);
                await _context.SaveChangesAsync();
            }
        }
    }
}