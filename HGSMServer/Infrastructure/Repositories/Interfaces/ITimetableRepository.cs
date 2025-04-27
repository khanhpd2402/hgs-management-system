using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITimetableRepository
    {
        Task<IEnumerable<Timetable>> GetTimetablesForPrincipalAsync(int timetableId, string? status = null);
        Task<IEnumerable<Timetable>> GetByStudentIdAsync(int studentId, int semesterId);
        Task<IEnumerable<Timetable>> GetByTeacherIdAsync(int teacherId);
        Task<IEnumerable<Timetable>> GetTimetablesBySemesterAsync(int semesterId);

        Task<Timetable> GetByIdAsync(int timetableId);
        Task UpdateTimetableAsync(Timetable timetable);
        Task<bool> UpdateMultipleDetailsAsync(List<TimetableDetail> details);
        Task<bool> DeleteDetailAsync(int detailId);
        Task<bool> IsConflictAsync(TimetableDetail detail);
        Task<Timetable> CreateTimetableAsync(Timetable timetable);
        Task<TimetableDetail> AddTimetableDetailAsync(TimetableDetail detail);

        Task<IEnumerable<Class>> GetAllClassesAsync(); // Lấy tất cả lớp
        Task<IEnumerable<Subject>> GetAllSubjectsAsync(); // Lấy tất cả môn học
        Task<IEnumerable<Period>> GetAllPeriodsAsync(); // Lấy tất cả tiết học
        Task<Timetable?> FindBySemesterAndEffectiveDateAsync(int semesterId, DateOnly effectiveDate); // Tìm TKB theo học kỳ và ngày hiệu lực
    }
}
