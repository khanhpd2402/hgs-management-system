using Application.Features.Attendances.DTOs;
using Application.Features.Attendances.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Attendances.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _repository;
        private readonly IMapper _mapper;

        public AttendanceService(IAttendanceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<AttendanceDto>> GetWeeklyAttendanceAsync(int classId, DateOnly weekStart)
        {
            var attendances = await _repository.GetByWeekAsync(classId, weekStart);
            return _mapper.Map<List<AttendanceDto>>(attendances);
        }

        public async Task UpsertAttendancesAsync(List<AttendanceDto> dtos)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = DateTime.Now;
            var entitiesToAdd = new List<Attendance>();
            var entitiesToUpdate = new List<Attendance>();

            foreach (var dto in dtos)
            {
                var attendanceDate = dto.Date;

                if (attendanceDate > today)
                    throw new InvalidOperationException("Không thể điểm danh trước ngày hiện tại.");

                if (attendanceDate == today)
                    ValidateSessionTime(dto.Session, now);

                var existing = await _repository.GetAsync(dto.StudentClassId, dto.Date, dto.Session);
                if (existing != null)
                {
                    existing.Status = dto.Status;
                    existing.Note = dto.Note;
                    entitiesToUpdate.Add(existing);
                }
                else
                {
                    var entity = _mapper.Map<Attendance>(dto);
                    entity.CreatedAt = now;
                    entitiesToAdd.Add(entity);
                }
            }

            if (entitiesToAdd.Any())
                await _repository.AddRangeAsync(entitiesToAdd);

            if (entitiesToUpdate.Any())
                await _repository.UpdateRangeAsync(entitiesToUpdate);
        }

        private void ValidateSessionTime(string session, DateTime now)
        {
            switch (session)
            {
                case "Sáng":
                    if (now.Hour < 7)
                        throw new InvalidOperationException("Chưa đến giờ điểm danh buổi sáng.");
                    break;
                case "Chiều":
                    if (now.Hour < 13 || (now.Hour == 13 && now.Minute < 30))
                        throw new InvalidOperationException("Chưa đến giờ điểm danh buổi chiều.");
                    break;
                default:
                    throw new InvalidOperationException("Buổi học không hợp lệ.");
            }
        }
    }

}
