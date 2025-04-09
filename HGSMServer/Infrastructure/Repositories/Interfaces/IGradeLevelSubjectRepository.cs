using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IGradeLevelSubjectRepository
    {
        Task<GradeLevelSubject?> GetByGradeAndSubjectAsync(int gradeLevelId, int subjectId);
        Task<IEnumerable<GradeLevelSubject>> GetBySubjectIdAsync(int subjectId);
        Task<IEnumerable<GradeLevelSubject>> GetAllAsync();
        Task<GradeLevelSubject> GetByIdAsync(int id);
        Task<GradeLevelSubject> CreateAsync(GradeLevelSubject entity);
        Task UpdateAsync(GradeLevelSubject entity);
        Task DeleteAsync(int id);
    }
}

