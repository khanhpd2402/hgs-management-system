using Application.Features.TeachingAssignment.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TeachingAssignment.Interfaces
{
    public interface ITeachingAssignmentService
    {
        Task CreateTeachingAssignmentAsync(TeachingAssignmentCreateDto dto);
        Task<TeachingAssignmentFilterDataDto> GetFilterDataAsync();
        Task<List<TeachingAssignmentResponseDto>> GetAssignmentsForCreationAsync(TeachingAssignmentCreateDto dto);
        Task<List<TeachingAssignmentResponseDto>> SearchTeachingAssignmentsAsync(TeachingAssignmentFilterDto filter);
    }
}
