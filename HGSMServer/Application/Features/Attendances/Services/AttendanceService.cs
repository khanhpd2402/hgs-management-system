using Application.Features.Attendances.DTOs;
using Application.Features.Attendances.Interfaces;
using AutoMapper;
using Common.Constants;
using Common.Utils;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Common.Constants.AppConstants;

namespace Application.Features.Attendances.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;

        public AttendanceService(IAttendanceUnitOfWork uow, IMapper mapper, EmailService emailService)
        {
            _uow = uow;
            _mapper = mapper;
            _emailService = emailService;
        }


        public async Task<List<AttendanceDto>> GetWeeklyAttendanceAsync(int teacherId, int classId, int semesterId, DateOnly weekStart)
        {
            var isAssigned = await _uow.TeachingAssignmentRepository.IsTeacherAssignedAsync(teacherId, classId, semesterId);
            if (!isAssigned)
                throw new UnauthorizedAccessException("Bạn không được phân công dạy lớp này trong học kỳ này.");

            var attendances = await _uow.AttendanceRepository.GetByWeekAsync(classId, weekStart);
            return _mapper.Map<List<AttendanceDto>>(attendances);
        }

        public async Task UpsertAttendancesAsync(int teacherId, int classId, int semesterId, List<AttendanceDto> dtos)
        {
            var isAssigned = await _uow.TeachingAssignmentRepository.IsTeacherAssignedAsync(teacherId, classId, semesterId);
            if (!isAssigned)
                throw new UnauthorizedAccessException("Bạn không được phân công dạy lớp này trong học kỳ này.");

            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = DateTime.Now;
            var entitiesToAdd = new List<Attendance>();
            var entitiesToUpdate = new List<Attendance>();

            var teacher = await _uow.TeacherRepository.GetByIdAsync(teacherId);

            foreach (var dto in dtos)
            {
                var attendanceDate = dto.Date;

                if (attendanceDate > today)
                    throw new InvalidOperationException("Không thể điểm danh trước ngày hiện tại.");

                if (attendanceDate == today)
                    ValidateSessionTime(dto.Session, now);

                var existing = await _uow.AttendanceRepository.GetAsync(dto.StudentClassId, dto.Date, dto.Session);
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

                // Gửi thông báo nếu không có mặt
                if (dto.Status != AttendanceStatus.PRESENT)
                {
                    var studentClass = await _uow.StudentClassRepository.GetWithClassAndStudentAsync(dto.StudentClassId);
                    if (studentClass?.Student?.Parent != null)
                    {
                        string reason = dto.Status switch
                        {
                            AttendanceStatus.ABSENT => "Nghỉ học không phép",
                            AttendanceStatus.PERMISSION => "Nghỉ học có phép",
                            AttendanceStatus.LATE => $"Trường hợp khác: {dto.Note}",
                            _ => "Không rõ lý do"
                        };

                        var parentEmails = new List<string?>
                {
                    studentClass.Student.Parent.EmailMother,
                    studentClass.Student.Parent.EmailFather,
                    studentClass.Student.Parent.EmailGuardian
                };

                        foreach (var parentEmail in parentEmails.Where(e => !string.IsNullOrWhiteSpace(e)).Distinct())
                        {
                            await _emailService.SendAbsenceNotificationAsync(
                                parentEmail: parentEmail!,
                                studentName: studentClass.Student.FullName,
                                className: studentClass.Class.ClassName,
                                absenceDate: dto.Date.ToDateTime(TimeOnly.MinValue),
                                reason: reason,
                                teacherName: teacher?.FullName,
                                teacherEmail: teacher?.User.Email,
                                teacherPhone: teacher?.User.PhoneNumber
                            );
                        }
                    }
                }
            }

            if (entitiesToAdd.Any())
                await _uow.AttendanceRepository.AddRangeAsync(entitiesToAdd);

            if (entitiesToUpdate.Any())
                await _uow.AttendanceRepository.UpdateRangeAsync(entitiesToUpdate);
        }

        private void ValidateSessionTime(string session, DateTime now)
        {
            switch (session)
            {
                //case "Sáng":
                //    if (now.Hour < 7)
                //        throw new InvalidOperationException("Chưa đến giờ điểm danh buổi sáng.");
                //    break;
                //case "Chiều":
                //    if (now.Hour < 13 || (now.Hour == 13 && now.Minute < 30))
                //        throw new InvalidOperationException("Chưa đến giờ điểm danh buổi chiều.");
                //    break;
                //default:
                //    throw new InvalidOperationException("Buổi học không hợp lệ.");
            }
        }
        public async Task<List<AttendanceDto>> GetHomeroomAttendanceAsync(int teacherId, int semesterId, DateOnly weekStart)
        {
            var homeroomAssignment = await _uow.HomeroomAssignmentRepository.GetByTeacherAndSemesterAsync(teacherId, semesterId);
            if (homeroomAssignment == null)
            {
                throw new InvalidOperationException("Giáo viên không được phân công làm chủ nhiệm lớp nào trong học kỳ này.");
            }

            var attendances = await _uow.AttendanceRepository.GetByWeekAsync(homeroomAssignment.ClassId, weekStart);
            return _mapper.Map<List<AttendanceDto>>(attendances);
        }
    }

}
