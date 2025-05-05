using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<List<Attendance>> GetByWeekAsync(int studentClassId, DateOnly weekStart);
        Task<Attendance?> GetAsync(int studentClassId, DateOnly date, string session);
        Task<List<Attendance>> GetByStudentAndWeekAsync(int studentId, DateOnly weekStart);
        Task AddRangeAsync(IEnumerable<Attendance> attendances);
        Task UpdateRangeAsync(IEnumerable<Attendance> attendances);
    }
}