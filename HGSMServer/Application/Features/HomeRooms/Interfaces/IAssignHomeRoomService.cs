using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.HomeRooms.DTOs;
using Application.Features.TeachingAssignments.DTOs;

namespace Application.Features.HomeRooms.Interfaces
{
    public interface IAssignHomeRoomService
    {
        Task AssignHomeroomAsync(AssignHomeroomDto dto);
        Task UpdateHomeroomAssignmentsAsync(List<UpdateHomeroomDto> dtos);
        Task<List<HomeroomAssignmentResponseDto>> GetAllHomeroomAssignmentsAsync();
    }
}