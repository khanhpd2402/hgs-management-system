using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IGradeRepository
    {
        Task AddRangeAsync(IEnumerable<Grade> grades);
        Task<List<Grade>> GetGradesByStudentAsync(int studentId, int semesterId);
        Task<List<Grade>> GetGradesByClassAsync(int classId, int subjectId, int semesterId);
        Task<List<Grade>> GetGradesByTeacherAsync(int teacherId, int classId, int subjectId, int semesterId);
        Task<IEnumerable<Grade>> GetByBatchIdAsync(int batchId);
        Task<bool> UpdateMultipleGradesAsync(List<Grade> grades);
        Task<List<Grade>> GetGradesByIdsAsync(List<int> gradeIds);
        Task DeleteRangeAsync(IEnumerable<Grade> grades);
        Task<IEnumerable<Grade>> GetByStudentClassIdsAsync(IEnumerable<int> studentClassIds);
    }
}
