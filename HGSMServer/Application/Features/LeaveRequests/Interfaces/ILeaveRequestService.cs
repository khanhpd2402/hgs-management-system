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
        Task<List<LeaveRequestDto>> GetAllAsync();
        Task<LeaveRequestDto?> GetByIdAsync(int id);
        Task<List<LeaveRequestDto>> GetByTeacherIdAsync(int teacherId);
        Task AddAsync(CreateLeaveRequestDto leaveRequestDto);
        Task UpdateAsync(LeaveRequestDto leaveRequestDto);
        Task DeleteAsync(int id);
    }

}
