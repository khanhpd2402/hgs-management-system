using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<Attendance> AddAttendance(Attendance attendance);
        Task<Attendance> UpdateAttendance(Attendance attendance);
        Task<Attendance> GetAttendanceById(int attendanceId);
        Task<List<Attendance>> GetAttendancesByClass(int classId, DateOnly date, string shift);
        Task<List<string>> GetParentPhoneNumbers(int studentId);
    }
}
