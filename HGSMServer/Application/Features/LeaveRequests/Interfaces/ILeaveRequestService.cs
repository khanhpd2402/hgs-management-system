using Application.Features.LeaveRequests.DTOs;
using Application.Features.LeaveRequests.DTOs.Application.Features.LeaveRequests.DTOs;

namespace Application.Features.LeaveRequests.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequestDetailDto?> GetByIdAsync(int id);
        Task<IEnumerable<LeaveRequestListDto>> GetAllAsync(int? teacherId = null, string? status = null);
        Task<LeaveRequestDetailDto> CreateAsync(CreateLeaveRequestDto dto);
        Task<bool> UpdateAsync(UpdateLeaveRequest dto);
        Task<bool> DeleteAsync(int id);
        Task<List<AvailableSubstituteTeacherDto>> FindAvailableSubstituteTeachersAsync(FindSubstituteTeacherRequestDto request);
        Task<List<AvailableSubstituteTeacherDto>> CheckAvailableTeachersAsync(FindSubstituteTeacherRequestDto request);
    }

}
