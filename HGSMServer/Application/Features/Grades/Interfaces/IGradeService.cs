using Application.Features.Grades.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Grades.Interfaces
{
    public interface IGradeService
    {
        Task AddGradesAsync(IEnumerable<GradeDto> gradeDtos);
        Task<List<GradeRespondDto>> GetGradesForStudentAsync(int studentId, int semesterId);
        Task<List<GradeRespondDto>> GetGradesForTeacherAsync(int teacherId, int classId, int subjectId, int semesterId);
        Task<List<GradeRespondDto>> GetGradesForPrincipalAsync(int classId, int subjectId, int semesterId);
        Task<bool> UpdateMultipleGradesAsync(UpdateMultipleGradesDto dto);
        Task<GradeSummaryDto> GetTotalGradeSummaryByStudentAsync(int studentId, int semesterId);
        Task<List<GradeSummaryEachSubjectNameDto>> GetGradeSummaryEachSubjectByStudentAsync(int studentId, int semesterId);
        Task<ImportGradesResultDto> ImportGradesFromExcelAsync(int classId, int subjectId, int semesterId, IFormFile file);
    }
}
