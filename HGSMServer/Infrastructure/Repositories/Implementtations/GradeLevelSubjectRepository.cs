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
    }
}
