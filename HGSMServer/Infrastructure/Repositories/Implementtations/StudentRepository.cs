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
    public class StudentRepository : IStudentRepository
    {
        private readonly HgsdbContext _context;

        public StudentRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.Class)
                .Include(s => s.Parents)
                .ToListAsync();
        }
        public IQueryable<Student> GetAll()
        {
            return _context.Students.AsQueryable();
        }
        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Class)
                .Include(s => s.Parents)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task AddAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddRangeAsync(IEnumerable<Student> students)
        {
            await _context.Students.AddRangeAsync(students);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(string idCard)
        {
            return await _context.Students.AnyAsync(s => s.IdcardNumber == idCard);
        }
    }
}