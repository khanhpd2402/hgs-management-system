using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IGradeRepository
    {
        Task AddRangeAsync(IEnumerable<Grade> grades);
        Task<List<Grade>> GetGradesAsync(int classId, int subjectId, int semesterId);
        Task<bool> UpdateMultipleGradesAsync(List<Grade> grades);
        Task<List<Grade>> GetGradesByIdsAsync(List<int> gradeIds);
    }
}
