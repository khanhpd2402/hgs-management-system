using Application.Features.TeacherSubjects.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Features.TeacherSubjects.Interfaces
{
    public interface ITeacherSubjectService
    {
        Task<List<TeacherSubjectDto>> GetAllAsync();
        Task<TeacherSubjectDto> GetByIdAsync(int id);
        Task<List<TeacherSubjectDto>> GetByTeacherIdAsync(int teacherId);
        Task<List<TeacherSubjectDto>> GetBySubjectIdAsync(int subjectId);
        Task CreateAsync(CreateTeacherSubjectDto dto);
        Task UpdateAsync(UpdateTeacherSubjectDto dto);
        Task DeleteAsync(int id);
    }
}