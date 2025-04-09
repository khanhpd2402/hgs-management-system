using Application.Features.GradeLevelSubjects.DTOs;

namespace Application.Features.GradeLevelSubjects.Interfaces
{

    public interface IGradeLevelSubjectService
    {
        Task<IEnumerable<GradeLevelSubjectDto>> GetAllAsync();
        Task<GradeLevelSubjectDto> GetByIdAsync(int id);
        Task<GradeLevelSubjectCreateAndUpdateDto> CreateAsync(GradeLevelSubjectCreateAndUpdateDto dto);
        Task<GradeLevelSubjectCreateAndUpdateDto> UpdateAsync(int id, GradeLevelSubjectCreateAndUpdateDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<GradeLevelSubjectDto>> GetBySubjectIdAsync(int subjectId);

    }
}

