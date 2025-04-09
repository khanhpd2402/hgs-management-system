using Application.Features.GradeLevels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeLevels.Interfaces
{
    public interface IGradeLevelService
    {
        Task<IEnumerable<GradeLevelDto>> GetAllAsync();
        Task<GradeLevelDto> GetByIdAsync(int id);
        Task<GradeLevelCreateAndUpdateDto> CreateAsync(GradeLevelCreateAndUpdateDto dto);
        Task<GradeLevelCreateAndUpdateDto> UpdateAsync(int id, GradeLevelCreateAndUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
