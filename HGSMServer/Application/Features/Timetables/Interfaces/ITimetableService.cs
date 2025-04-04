using Application.Features.Timetables.DTOs;
using Domain.Models;

namespace Application.Features.Timetables.Interfaces
{
    public interface ITimetableService
    {
        Task<IEnumerable<TimetableDetailDto>> GetTimetableByStudentAsync(int studentId, int? semesterId = null, DateOnly? effectiveDate = null);
        Task<IEnumerable<TimetableDetailDto>> GetTimetableByTeacherAsync(int teacherId, int? semesterId = null, DateOnly? effectiveDate = null);
        Task<Timetable> CreateTimetableAsync(CreateTimetableDto dto);
        Task<bool> UpdateDetailAsync(TimetableDetailDto detailDto);
        Task<bool> DeleteDetailAsync(int detailId);
        Task<bool> IsConflictAsync(TimetableDetailDto detailDto);
    }

}
