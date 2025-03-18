using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly HgsdbContext _context;

        public StudentRepository(HgsdbContext context)
        {
            _context = context;
        }

        public IQueryable<Student> GetAll()
        {
            return _context.Students.AsQueryable();
        }
        public async Task<List<Student>> GetStudentsByAcademicYearAsync(int academicYearId)
        {
            if (academicYearId == 0)
                return new List<Student>();

            // Lọc học sinh theo AcademicYearId
            return await _context.Students.Include(s => s.StudentClasses)
                .Where(s => s.StudentClasses.Any(sc => sc.AcademicYearId == academicYearId))
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
                .Include(s => s.StudentParents) // Sử dụng bảng trung gian
                    .ThenInclude(sp => sp.Parent) // Lấy thông tin phụ huynh
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }
        //public async Task<List<Student>> GetStudentsByIdsAsync(List<int> studentIds)
        //{
        //    return await _context.Students
        //        .Where(s => studentIds.Contains(s.StudentId))
        //        .ToListAsync();
        //}
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