using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IGradeLevelSubjectRepository
    {
        Task<GradeLevelSubject?> GetByGradeAndSubjectAsync(int gradeLevelId, int subjectId);
    }
}
