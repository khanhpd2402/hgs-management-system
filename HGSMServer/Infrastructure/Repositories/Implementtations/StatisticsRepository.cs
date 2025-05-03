using Common.Constants;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementtations
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly HgsdbContext _context;

        public StatisticsRepository(HgsdbContext context)
        {
            _context = context;
        }

        public Task<int> GetTotalStudentsAsync()
     => _context.Students.CountAsync(s => s.Status == AppConstants.StudentStatus.STUDYING);

        public Task<int> GetTotalTeachersAsync()
            => _context.Teachers.CountAsync(t => t.EmploymentStatus == AppConstants.TeacherStatus.WORKING);

        public Task<int> GetMaleStudentsCountAsync()
            => _context.Students.CountAsync(s => s.Status == AppConstants.StudentStatus.STUDYING && s.Gender.ToLower() == "nam");

        public Task<int> GetFemaleStudentsCountAsync()
            => _context.Students.CountAsync(s => s.Status == AppConstants.StudentStatus.STUDYING && s.Gender.ToLower() == "nữ");

        public Task<int> GetMaleTeachersCountAsync()
            => _context.Teachers.CountAsync(t => t.EmploymentStatus == AppConstants.TeacherStatus.WORKING && t.Gender.ToLower() == "nam");

        public Task<int> GetFemaleTeachersCountAsync()
            => _context.Teachers.CountAsync(t => t.EmploymentStatus == AppConstants.TeacherStatus.WORKING && t.Gender.ToLower() == "nữ");
        public async Task<int> GetTotalAbsentStudentsTodayAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Attendances.CountAsync(a =>
                a.Date == today &&
                (a.Status == AppConstants.AttendanceStatus.ABSENT ||
                 a.Status == AppConstants.AttendanceStatus.PERMISSION));
        }

        public async Task<int> GetPermissionAbsentStudentsTodayAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Attendances.CountAsync(a =>
                a.Date == today &&
                a.Status == AppConstants.AttendanceStatus.PERMISSION);
        }

        public async Task<int> GetAbsentWithoutPermissionStudentsTodayAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Attendances.CountAsync(a =>
                a.Date == today &&
                a.Status == AppConstants.AttendanceStatus.ABSENT);
        }
        public Task<int> GetUnknownAbsentStudentsTodayAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return _context.Attendances.CountAsync(a =>
                a.Date == today && a.Status == AppConstants.AttendanceStatus.LATE);
        }

        public Task<int> GetTotalActiveClassesAsync()
    => _context.Classes.CountAsync(c => c.Status == AppConstants.Status.ACTIVE);
    }

}
