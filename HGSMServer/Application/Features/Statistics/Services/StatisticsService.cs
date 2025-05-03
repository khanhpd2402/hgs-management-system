using Application.Features.Statistics.Interfaces;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Statistics.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _repository;

        public StatisticsService(IStatisticsRepository repository)
        {
            _repository = repository;
        }

        public async Task<object> GetSchoolStatisticsAsync()
        {
            var totalStudents = await _repository.GetTotalStudentsAsync();
            var totalTeachers = await _repository.GetTotalTeachersAsync();
            var maleStudents = await _repository.GetMaleStudentsCountAsync();
            var femaleStudents = await _repository.GetFemaleStudentsCountAsync();
            var maleTeachers = await _repository.GetMaleTeachersCountAsync();
            var femaleTeachers = await _repository.GetFemaleTeachersCountAsync();
            var totalClasses = await _repository.GetTotalActiveClassesAsync();

            var totalAbsent = await _repository.GetTotalAbsentStudentsTodayAsync();
            var permissionAbsent = await _repository.GetPermissionAbsentStudentsTodayAsync();
            var absentWithoutPermission = await _repository.GetAbsentWithoutPermissionStudentsTodayAsync();
            var unknownAbsent = await _repository.GetUnknownAbsentStudentsTodayAsync();

            return new
            {
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                MaleStudents = maleStudents,
                FemaleStudents = femaleStudents,
                MaleTeachers = maleTeachers,
                FemaleTeachers = femaleTeachers,
                ActiveClasses = totalClasses,
                AttendanceSummary = new
                {
                    TotalAbsent = totalAbsent,
                    PermissionAbsent = permissionAbsent,
                    AbsentWithoutPermission = absentWithoutPermission,
                    UnknownAbsent = unknownAbsent
                }
            };
        }

    }
}
