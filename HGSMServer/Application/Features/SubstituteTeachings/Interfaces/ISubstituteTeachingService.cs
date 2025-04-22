using Application.Features.SubstituteTeachings.DTOs;

namespace Application.Features.SubstituteTeachings.Interfaces
{
    public interface ISubstituteTeachingService
    {
        Task<SubstituteTeachingDto> CreateOrUpdateAsync(SubstituteTeachingCreateDto dto);
        Task<SubstituteTeachingDto> GetByIdAsync(int substituteId);
        Task<IEnumerable<SubstituteTeachingDto>> GetAllAsync(int? timetableDetailId = null , int? OriginalTeacherId = null, int? SubstituteTeacherId = null, DateOnly? date = null);
        Task DeleteAsync(int substituteId);
    }

}
