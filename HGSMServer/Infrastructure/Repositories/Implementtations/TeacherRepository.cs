using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<bool> ExistsAsync(string idCard)
        {
            return await _context.Teachers.AnyAsync(t => t.IdcardNumber == idCard);
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

        public async Task<List<TeacherSubject>?> GetTeacherSubjectsAsync(int teacherId)
        {
            return await _context.TeacherSubjects
                .Include(ts => ts.Subject)
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

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(t => t.Username == username);
        }

        public async Task<bool> IsEmailOrPhoneExistsAsync(string email, string phoneNumber)
        {
            return await _context.Users.AnyAsync(u => u.Email == email || u.PhoneNumber == phoneNumber);
        }
        public async Task AddTeacherSubjectAsync(TeacherSubject teacherSubject)
        {
            await _context.TeacherSubjects.AddAsync(teacherSubject);
            await _context.SaveChangesAsync();
        }
        public async Task AddTeacherSubjectsRangeAsync(IEnumerable<TeacherSubject> teacherSubjects)
        {
            await _context.TeacherSubjects.AddRangeAsync(teacherSubjects);
            await _context.SaveChangesAsync();
        }
        public async Task<Teacher?> GetByIdWithUserAsync(int id)
        {
            return await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TeacherId == id);
        }
        public async Task<IEnumerable<Teacher>> GetAllWithUserAsync()
        {
            return await _context.Teachers
                .Include(t => t.User)
                .ToListAsync();
        }
        public async Task UpdateTeacherSubjectAsync(TeacherSubject teacherSubject)
        {
            _context.TeacherSubjects.Update(teacherSubject);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeacherSubjectsRangeAsync(IEnumerable<TeacherSubject> teacherSubjects)
        {
            _context.TeacherSubjects.RemoveRange(teacherSubjects);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}