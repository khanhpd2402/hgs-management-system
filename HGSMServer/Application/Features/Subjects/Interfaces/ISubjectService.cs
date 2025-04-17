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
        Task<IEnumerable<SubjectDto>> GetAllAsync();
        Task<SubjectDto> GetByIdAsync(int id);
        Task<SubjectDto> CreateAsync(SubjectCreateAndUpdateDto dto);
        Task<SubjectDto> UpdateAsync(int id, SubjectCreateAndUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
