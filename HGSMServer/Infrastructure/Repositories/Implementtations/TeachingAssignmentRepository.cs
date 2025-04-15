using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementtations
{
    public class TeachingAssignmentRepository : ITeachingAssignmentRepository
    {
        private readonly HgsdbContext _context;

        public TeachingAssignmentRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<TeachingAssignment> GetAssignmentByClassSubjectTeacherAsync(int classId, int subjectId, int semesterId)
        {
            return await _context.TeachingAssignments
                                 .Where(ta => ta.ClassId == classId && ta.SubjectId == subjectId && ta.SemesterId == semesterId)
                                 .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<TeachingAssignment>> GetBySemesterIdAsync(int semesterId)
        {
            return await _context.TeachingAssignments
        .Where(t => t.SemesterId == semesterId)
        .Include(t => t.Class)
        .Include(t => t.Teacher) 
        .ToListAsync();
        }
        
    }
}
