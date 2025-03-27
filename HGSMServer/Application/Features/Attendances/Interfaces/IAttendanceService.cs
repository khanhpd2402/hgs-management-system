using Application.Features.Attendances.DTOs;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Attendances.Interfaces
{
    public interface IAttendanceService
    {
        Task<Attendance> CreateAttendance(AttendanceRequest request);
        Task<Attendance> UpdateAttendance(int attendanceId, AttendanceRequest request);
        Task<Attendance> GetAttendanceById(int attendanceId);
        Task<List<Attendance>> GetAttendancesByClass(int classId, DateOnly date, string shift);
    }
}
