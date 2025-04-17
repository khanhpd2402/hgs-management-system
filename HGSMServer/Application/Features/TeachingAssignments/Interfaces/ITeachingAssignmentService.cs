using Application.Features.TeachingAssignments.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Features.TeachingAssignments.Interfaces
{
    public interface ITeachingAssignmentService
    {
        Task CreateTeachingAssignmentsAsync(List<TeachingAssignmentCreateDto> dtos);
        Task<List<TeachingAssignmentResponseDto>> GetAllTeachingAssignmentsAsync(int semesterId);
        Task<TeachingAssignmentFilterDataDto> GetFilterDataAsync();
        Task UpdateTeachingAssignmentAsync(TeachingAssignmentUpdateDto dto);
        Task<List<TeachingAssignmentResponseDto>> GetAssignmentsForCreationAsync(TeachingAssignmentCreateDto dto);
        Task<List<TeachingAssignmentResponseDto>> GetTeachingAssignmentsByTeacherIdAsync(int teacherId);
    }
}