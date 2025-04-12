using Application.Features.Periods.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Periods.Interfaces
{
    public interface IPeriodService
    {
        Task<IEnumerable<PeriodDto>> GetAllAsync();
        Task<PeriodDto> GetByIdAsync(int id);
        Task<PeriodCreateAndUpdateDto> CreateAsync(PeriodCreateAndUpdateDto dto);
        Task<PeriodCreateAndUpdateDto> UpdateAsync(int id, PeriodCreateAndUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
