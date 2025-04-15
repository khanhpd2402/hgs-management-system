using Application.Features.HomeRooms.DTOs;
using Application.Features.TeachingAssignments.DTOs;

namespace Application.Features.TeachingAssignments.Interfaces
{
    public interface ITeachingAssignmentService
    {
        Task CreateTeachingAssignmentsAsync(List<TeachingAssignmentCreateDto> dtos);
        Task<TeachingAssignmentFilterDataDto> GetFilterDataAsync();
        Task<List<TeachingAssignmentResponseDto>> GetAssignmentsForCreationAsync(TeachingAssignmentCreateDto dto);
        Task<List<TeachingAssignmentResponseDto>> GetAllTeachingAssignmentsAsync();
        Task UpdateTeachingAssignmentsAsync(List<TeachingAssignmentUpdateDto> dtos);
    }
}
