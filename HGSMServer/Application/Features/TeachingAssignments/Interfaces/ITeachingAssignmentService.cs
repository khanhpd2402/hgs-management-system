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
        Task UpdateTeachingAssignmentsAsync(List<TeachingAssignmentUpdateDto> dtos);
        Task<List<TeachingAssignmentResponseDto>> GetAssignmentsForCreationAsync(TeachingAssignmentCreateDto dto);
        Task<List<TeachingAssignmentResponseDto>> GetTeachingAssignmentsByTeacherIdAsync(int teacherId, int semesterId);
        Task DeleteTeachingAssignmentsByTeacherIdAndSemesterIdAsync(int teacherId, int semesterId);
    }
}