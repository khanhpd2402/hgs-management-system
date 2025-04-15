using Application.Features.SubstituteTeachings.DTOs;

namespace Application.Features.SubstituteTeachings.Interfaces
{
    public interface ISubstituteTeachingService
    {
        Task<SubstituteTeachingDto> CreateAsync(SubstituteTeachingCreateDto dto);
        Task<SubstituteTeachingDto> GetByIdAsync(int substituteId);
        Task<IEnumerable<SubstituteTeachingDto>> GetAllAsync(int? timetableDetailId = null, int? teacherId = null, DateOnly? date = null);
        Task<SubstituteTeachingDto> UpdateAsync(SubstituteTeachingUpdateDto dto);
        Task DeleteAsync(int substituteId);
    }

}
