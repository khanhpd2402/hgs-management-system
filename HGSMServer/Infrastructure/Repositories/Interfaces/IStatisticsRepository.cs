using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IStatisticsRepository
    {
        Task<int> GetTotalStudentsAsync();
        Task<int> GetTotalTeachersAsync();
        Task<int> GetMaleStudentsCountAsync();
        Task<int> GetFemaleStudentsCountAsync();
        Task<int> GetMaleTeachersCountAsync();
        Task<int> GetFemaleTeachersCountAsync();
        Task<int> GetTotalAbsentStudentsTodayAsync();
        Task<int> GetPermissionAbsentStudentsTodayAsync();
        Task<int> GetAbsentWithoutPermissionStudentsTodayAsync();
        Task<int> GetTotalActiveClassesAsync();
        Task<int> GetUnknownAbsentStudentsTodayAsync();
    }

}
