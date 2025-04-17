using Application.Features.Classes.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Classes.Interfaces
{
    public interface IClassService
    {
        Task<IEnumerable<ClassDto>> GetAllClassesAsync();
        Task<IEnumerable<ClassDto>> GetAllClassesActiveAsync(string? status = null);
        Task<ClassDto> GetClassByIdAsync(int id);
        Task<ClassDto> CreateClassAsync(ClassDto classDto);
        Task<ClassDto> UpdateClassAsync(int id, ClassDto classDto);
        Task DeleteClassAsync(int id);
    }
}
