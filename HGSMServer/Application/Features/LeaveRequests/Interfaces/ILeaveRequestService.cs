using Application.Features.LeaveRequests.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LeaveRequests.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequestDetailDto?> GetByIdAsync(int id);
        Task<IEnumerable<LeaveRequestListDto>> GetAllAsync(int? teacherId = null, string? status = null);
        Task<LeaveRequestDetailDto> CreateAsync(CreateLeaveRequestDto dto);
        Task<bool> UpdateAsync(UpdateLeaveRequest dto);
        Task<bool> DeleteAsync(int id);
    }

}
