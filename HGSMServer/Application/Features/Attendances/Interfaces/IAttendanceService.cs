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
        Task<List<AttendanceDto>> GetWeeklyAttendanceAsync(int teacherId, int classId, int semesterId, DateOnly weekStart);
        Task UpsertAttendancesAsync(int teacherId, int classId, int semesterId, List<AttendanceDto> dtos);
        Task<List<AttendanceDto>> GetHomeroomAttendanceAsync(int teacherId, int semesterId, DateOnly weekStart);
    }
}
