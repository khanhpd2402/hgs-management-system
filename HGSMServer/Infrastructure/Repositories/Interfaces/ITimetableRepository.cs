using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITimetableRepository
    {
        Task<IEnumerable<Timetable>> GetByStudentIdAsync(int studentId, int? semesterId = null, DateOnly? effectiveDate = null);
        Task<IEnumerable<Timetable>> GetByTeacherIdAsync(int teacherId, int? semesterId = null, DateOnly? effectiveDate = null);
        Task<IEnumerable<Timetable>> GetByClassIdAsync(int classId, int? semesterId = null, DateOnly? effectiveDate = null);
        Task<bool> UpdateDetailAsync(TimetableDetail detail);
        Task<bool> DeleteDetailAsync(int detailId);
        Task<bool> IsConflictAsync(TimetableDetail detail);
        Task<Timetable> CreateTimetableAsync(Timetable timetable);
        Task<TimetableDetail> AddTimetableDetailAsync(TimetableDetail detail);
    }
}
