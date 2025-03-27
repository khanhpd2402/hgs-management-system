using Application.Features.Grades.DTOs;

namespace Application.Features.Grades.Interfaces
{
    public interface IGradeService
    {
        Task AddGradesAsync(IEnumerable<GradeDto> gradeDtos);
        Task<List<GradeRespondDto>> GetGradesByClassSubjectSemesterAsync(int classId, int subjectId, int semesterId);
        Task<bool> UpdateMultipleGradesAsync(UpdateMultipleGradesDto dto);
    }
}
