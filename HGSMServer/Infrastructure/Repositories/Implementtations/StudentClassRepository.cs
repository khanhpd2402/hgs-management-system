using Common.Constants;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories.Implementtations
{
    public class StudentClassRepository : IStudentClassRepository
    {
        private readonly HgsdbContext _context;

        public StudentClassRepository(HgsdbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StudentClass>> GetByClassIdAndAcademicYearAsync(int classId, int academicYearId)
        {
            return await _context.StudentClasses
                .Include(sc => sc.Student)
                .Where(sc => sc.ClassId == classId
                             && sc.AcademicYearId == academicYearId)
                .ToListAsync();
        }
        public async Task<StudentClass?> GetWithClassAndStudentAsync(int studentClassId)
        {
            return await _context.StudentClasses
                .Include(sc => sc.Student).ThenInclude(s => s.Parent)
                .Include(sc => sc.Class)
                .FirstOrDefaultAsync(sc => sc.Id == studentClassId);
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
        public async Task<StudentClass> GetByStudentAndAcademicYearAsync(int studentId, int academicYearId)
        {
            return await _context.StudentClasses
                .Include(sc => sc.Student)
                .Include(sc => sc.Class)
                .Include(sc => sc.AcademicYear)
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.AcademicYearId == academicYearId);
        }

        public async Task<IEnumerable<StudentClass>> SearchAsync(int? studentId, int? classId, int? academicYearId, string studentName)
        {
            var query = _context.StudentClasses
                .Include(sc => sc.Student)
                .Include(sc => sc.Class)
                .Include(sc => sc.AcademicYear)
                .AsQueryable();

            if (studentId.HasValue)
                query = query.Where(sc => sc.StudentId == studentId.Value);
            if (classId.HasValue)
                query = query.Where(sc => sc.ClassId == classId.Value);
            if (academicYearId.HasValue)
                query = query.Where(sc => sc.AcademicYearId == academicYearId.Value);
            if (!string.IsNullOrEmpty(studentName))
                query = query.Where(sc => sc.Student.FullName.Contains(studentName));

            return await query.ToListAsync();
        }
        public async Task AddRangeAsync(IEnumerable<StudentClass> entities)
        {
            await _context.StudentClasses.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateRangeAsync(List<StudentClass> assignments)
        {
            foreach (var assignment in assignments)
            {
                _context.StudentClasses.Update(assignment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<int> ids)
        {
            var assignments = await _context.StudentClasses
                .Where(sc => ids.Contains(sc.Id))
                .ToListAsync();
            _context.StudentClasses.RemoveRange(assignments);
            await _context.SaveChangesAsync();
        }
        public async Task<List<StudentClass>> GetByGradeLevelAndAcademicYearAsync(int gradeLevelId, int academicYearId)
        {
            return await _context.StudentClasses
                .Include(sc => sc.Class)
                .Include(sc => sc.Student)
                .Include(sc => sc.AcademicYear)
                .Where(sc => sc.Class.GradeLevelId == gradeLevelId && sc.AcademicYearId == academicYearId)
                .ToListAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<List<StudentClass>> GetByStudentIdAsync(int studentId)
        {
            return await _context.StudentClasses
                .Include(sc => sc.Class)
                .Include(sc => sc.AcademicYear)
                .Where(sc => sc.StudentId == studentId)
                .ToListAsync();
        }

    }
    }
