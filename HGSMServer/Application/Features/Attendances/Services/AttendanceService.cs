using Application.Features.Attendances.DTOs;
using Application.Features.Attendances.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Attendances.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HgsdbContext _context;

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            IHttpContextAccessor httpContextAccessor,
            HgsdbContext context)
        {
            _attendanceRepository = attendanceRepository;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

       
    }
}
