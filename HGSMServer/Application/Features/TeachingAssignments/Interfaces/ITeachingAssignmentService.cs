using Application.Features.Teachers.DTOs;
using Application.Features.TeachingAssignments.DTOs;

namespace Application.Features.TeachingAssignments.Interfaces
{
    public interface ITeachingAssignmentService
    {
        Task CreateTeachingAssignmentsAsync(List<TeachingAssignmentCreateDto> dtos);
        Task<TeachingAssignmentFilterDataDto> GetFilterDataAsync();
        Task<List<TeachingAssignmentResponseDto>> GetAssignmentsForCreationAsync(TeachingAssignmentCreateDto dto);
        Task AssignHomeroomAsync(AssignHomeroomDto dto);
        Task<List<TeachingAssignmentResponseDto>> GetAllTeachingAssignmentsAsync();
        Task UpdateTeachingAssignmentsAsync(List<TeachingAssignmentUpdateDto> dtos);
        Task UpdateHomeroomAssignmentsAsync(List<UpdateHomeroomDto> dtos);
    }
}
