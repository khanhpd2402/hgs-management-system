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
        
        public async Task<StudentClass> GetStudentClassByStudentAndClassIdAsync(int studentId, int classId)
        {
            return await _context.StudentClasses
                                 .Where(sc => sc.StudentId == studentId && sc.ClassId == classId)
                                 .FirstOrDefaultAsync();
        }
        }
    }
