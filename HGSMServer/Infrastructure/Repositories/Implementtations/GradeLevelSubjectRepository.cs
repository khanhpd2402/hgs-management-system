using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Implementtations
{
    public class GradeLevelSubjectRepository : IGradeLevelSubjectRepository
    {
        private readonly HgsdbContext _context;

        public GradeLevelSubjectRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<GradeLevelSubject?> GetByGradeAndSubjectAsync(int gradeLevelId, int subjectId)
        {
            return await _context.GradeLevelSubjects
                .FirstOrDefaultAsync(gls => gls.GradeLevelId == gradeLevelId && gls.SubjectId == subjectId);
        }
        public async Task<IEnumerable<GradeLevelSubject>> GetAllAsync()
        {
            return await _context.GradeLevelSubjects
                .Include(gls => gls.GradeLevel)
                .Include(gls => gls.Subject)
                .ToListAsync();
        }

        public async Task<GradeLevelSubject> GetByIdAsync(int id)
        {
            return await _context.GradeLevelSubjects
                .Include(gls => gls.GradeLevel)
                .Include(gls => gls.Subject)
                .FirstOrDefaultAsync(gls => gls.GradeLevelSubjectId == id);
        }

        public async Task<GradeLevelSubject> CreateAsync(GradeLevelSubject entity)
        {
            _context.GradeLevelSubjects.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(GradeLevelSubject entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.GradeLevelSubjects.FindAsync(id);
            if (entity != null)
            {
                _context.GradeLevelSubjects.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<GradeLevelSubject>> GetBySubjectIdAsync(int subjectId)
        {
            return await _context.GradeLevelSubjects
                .Include(gls => gls.GradeLevel)
                .Include(gls => gls.Subject)
                .Where(gls => gls.SubjectId == subjectId)
                .ToListAsync();
        }
    }
}
