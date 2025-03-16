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
        private readonly ISmsService _smsService;
        private readonly HgsdbContext _context;

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            IHttpContextAccessor httpContextAccessor,
            ISmsService smsService,
            HgsdbContext context)
        {
            _attendanceRepository = attendanceRepository;
            _httpContextAccessor = httpContextAccessor;
            _smsService = smsService;
            _context = context;
        }

        public async Task<Attendance> CreateAttendance(AttendanceRequest request)
        {
            var attendance = new Attendance
            {
                StudentId = request.StudentID,
                Date = request.Date,
                Status = request.Status,
                Note = request.Note,
                CreatedAt = DateTime.Now,
                CreatedBy = 1,
                //GetCurrentTeacherId()
                Shift = request.Shift
            };

            var result = await _attendanceRepository.AddAttendance(attendance);

            if (request.Status == "P" || request.Status == "K")
            {
                await SendSmsIfAbsent(request.StudentID, request.Date, request.Status, request.Shift, request.Note);
            }

            return result;
        }

        public async Task<Attendance> UpdateAttendance(int attendanceId, AttendanceRequest request)
        {
            var attendance = await _attendanceRepository.GetAttendanceById(attendanceId);
            if (attendance == null)
                throw new Exception("Attendance not found");

            var oldStatus = attendance.Status;
            attendance.Status = request.Status;
            attendance.Note = request.Note;

            var result = await _attendanceRepository.UpdateAttendance(attendance);

            if ((oldStatus != "P" && oldStatus != "K") && (request.Status == "P" || request.Status == "K"))
            {
                await SendSmsIfAbsent(request.StudentID, request.Date, request.Status, request.Shift, request.Note);
            }

            return result;
        }

        public async Task<Attendance> GetAttendanceById(int attendanceId)
        {
            return await _attendanceRepository.GetAttendanceById(attendanceId);
        }

        public async Task<List<Attendance>> GetAttendancesByClass(int classId, DateOnly date, string shift)
        {
            return await _attendanceRepository.GetAttendancesByClass(classId, date, shift);
        }

        private async Task SendSmsIfAbsent(int studentId, DateOnly date, string status, string shift, string note)
        {
            //var phoneNumber = await _attendanceRepository.GetParentPhoneNumbers(studentId);
            //if (!string.IsNullOrEmpty(phoneNumber))
            //{
            //    var student = await _context.Students.FindAsync(studentId);
            //    var message = $"Học sinh {student.FullName} vắng mặt ngày {date:yyyy-MM-dd} (Ca: {shift}). Trạng thái: {(status == "P" ? "Nghỉ có phép" : "Nghỉ không phép")}. Ghi chú: {note ?? "Không có"}";
            //    await _smsService.SendSmsAsync(phoneNumber, message);
            //}
        }

        private int GetCurrentTeacherId()
        {
            var teacherIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("TeacherID")?.Value;
            return int.Parse(teacherIdClaim ?? throw new Exception("TeacherID not found in token"));
        }
    }
}
