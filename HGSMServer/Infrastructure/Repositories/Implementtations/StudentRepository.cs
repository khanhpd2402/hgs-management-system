using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Student>> GetAllWithParentsAsync(int academicYearId)
        {
            return await _context.Students
                .Include(s => s.Parent) // Lấy thông tin Parent qua ParentId
                    .ThenInclude(p => p.User) // Lấy thông tin User của Parent
                .Include(s => s.StudentClasses)
                    .ThenInclude(sc => sc.Class)
                .Where(s => s.StudentClasses.Any(sc => sc.AcademicYearId == academicYearId)) // Lọc theo năm học
                .ToListAsync();
        }

        public async Task<int> GetAcademicYearIdAsync(int semesterId)
        {
            return await _context.Semesters
                .Where(s => s.SemesterId == semesterId)
                .Select(s => s.AcademicYearId)
                .FirstOrDefaultAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.StudentClasses)
                    .ThenInclude(sc => sc.Class)
                .Include(s => s.StudentClasses)
                    .ThenInclude(sc => sc.AcademicYear)
                .Include(s => s.Parent) // Lấy thông tin Parent qua ParentId
                    .ThenInclude(p => p.User) // Lấy thông tin User của Parent
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<Student?> GetByIdWithParentsAsync(int id, int academicYearId)
        {
            return await _context.Students
                .Include(s => s.Parent) // Lấy thông tin Parent qua ParentId
                    .ThenInclude(p => p.User) // Lấy thông tin User của Parent
                .Include(s => s.StudentClasses)
                    .ThenInclude(sc => sc.Class)
                .Where(s => s.StudentId == id && s.StudentClasses.Any(sc => sc.AcademicYearId == academicYearId)) // Lọc theo năm học
                .FirstOrDefaultAsync();
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

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<AcademicYear> GetCurrentAcademicYearAsync(DateOnly currentDate)
        {
            return await _context.Set<AcademicYear>()
                .FirstOrDefaultAsync(ay => ay.StartDate <= currentDate && ay.EndDate >= currentDate);
        }
    }
}