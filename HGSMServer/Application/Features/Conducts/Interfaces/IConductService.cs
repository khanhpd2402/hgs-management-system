using Application.Features.Conducts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Conducts.Interfaces
{
    public interface IConductService
    {
        Task<IEnumerable<ConductDto>> GetAllAsync();
        Task<ConductDto> GetByIdAsync(int id);
        Task<ConductDto> CreateAsync(CreateConductDto dto);
        Task<ConductDto> UpdateAsync(int id, UpdateConductDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
