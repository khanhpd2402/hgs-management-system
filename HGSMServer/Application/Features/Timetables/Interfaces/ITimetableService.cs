using Application.Features.Timetables.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Timetables.Interfaces
{
    public interface ITimetableService
    {
        Task<IEnumerable<TimetableDto>> GetTimetableByStudentAsync(int studentId, int semesterId);
        Task<IEnumerable<TimetableDto>> GetTimetableByTeacherAsync(int teacherId);
        Task<IEnumerable<TimetableDto>> GetTimetablesForPrincipalAsync(int timetableId, string? status = null);
        Task<IEnumerable<TimetableListDto>> GetTimetablesBySemesterAsync(int semesterId);
        Task<Timetable> CreateTimetableAsync(CreateTimetableDto dto);
        Task<bool> UpdateMultipleDetailsAsync(UpdateTimetableDetailsDto dto);
        Task<TimetableDto> UpdateTimetableInfoAsync(UpdateTimetableInfoDto dto);
        Task<bool> DeleteDetailAsync(int detailId);
        //Task<bool> IsConflictAsync(TimetableDetailDto detailDto);

        Task<Timetable> ImportTimetableAsync(IFormFile file, int semesterId, DateOnly effectiveDate);
        Task CreateDetailAsync(CreateTimetableDetailRequest request);
    }

}
