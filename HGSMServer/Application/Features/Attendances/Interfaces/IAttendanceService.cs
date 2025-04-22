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
        Task<List<AttendanceDto>> GetWeeklyAttendanceAsync(int classId, DateOnly weekStart);
        Task UpsertAttendancesAsync(List<AttendanceDto> dtos);
    }
}
