using Application.Features.TeachingAssignments.DTOs;

namespace Application.Features.TeachingAssignments.Interfaces
{
    public interface ITeachingAssignmentService
    {
        Task CreateTeachingAssignmentAsync(TeachingAssignmentCreateDto dto);
        Task<TeachingAssignmentFilterDataDto> GetFilterDataAsync();
        Task<List<TeachingAssignmentResponseDto>> GetAssignmentsForCreationAsync(TeachingAssignmentCreateDto dto);
        Task<List<TeachingAssignmentResponseDto>> SearchTeachingAssignmentsAsync(TeachingAssignmentFilterDto filter);
        Task AssignHomeroomAsync(AssignHomeroomDto dto);
    }
}
