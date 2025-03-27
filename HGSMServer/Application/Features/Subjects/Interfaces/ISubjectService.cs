using Application.Features.Subjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Subjects.Interfaces
{
    public interface ISubjectService
    {
        Task<List<SubjectDto>> GetAllSubjectsAsync();
        Task<SubjectDto?> GetSubjectByIdAsync(int id);
        Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createDto);
        Task<bool> UpdateSubjectAsync(int id, UpdateSubjectDto updateDto);
        Task<bool> DeleteSubjectAsync(int id);
    }
    
}
