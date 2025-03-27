using Application.Features.GradeBatchs.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeBatchs.Interfaces
{
    public interface IGradeBatchService
    {
        Task<IEnumerable<GradeBatchDto>> GetAllAsync();
        Task<GradeBatchDto?> GetByIdAsync(int id);
        Task<GradeBatchDto> CreateAsync(GradeBatchDto gradeBatchDto, List<int> subjectIds, List<string> assessmentTypes);
        Task<bool> UpdateAsync(GradeBatchDto gradeBatchDto);
        Task<bool> DeleteAsync(int id);
    }
}
