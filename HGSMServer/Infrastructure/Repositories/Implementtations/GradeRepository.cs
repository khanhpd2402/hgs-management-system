using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementtations
{
    public class GradeRepository : IGradeRepository
    {
        private readonly HgsdbContext _context;

        public GradeRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Grade> grades)
        {
            await _context.Grades.AddRangeAsync(grades);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Grade>> GetGradesAsync(int classId, int subjectId, int semesterId)
        {
            return await (from g in _context.Grades
                          join gb in _context.GradeBatches on g.BatchId equals gb.BatchId
                          join sem in _context.Semesters on gb.SemesterId equals sem.SemesterId
                          where g.ClassId == classId
                                && g.SubjectId == subjectId
                                && sem.SemesterId == semesterId
                          select g).Include(s => s.Student).ToListAsync();
        }
        public async Task<bool> UpdateMultipleGradesAsync(List<Grade> grades)
        {
            if (grades == null || grades.Count == 0) return false;

            _context.Grades.UpdateRange(grades);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Grade>> GetGradesByIdsAsync(List<int> gradeIds)
        {
            return await _context.Grades
                .Where(g => gradeIds.Contains(g.GradeId))
                .ToListAsync();
        }
        public async Task<IEnumerable<Grade>> GetByBatchIdAsync(int batchId)
        {
            return await _context.Grades
                .Where(g => g.BatchId == batchId)
                .ToListAsync();
        }
    }
}
