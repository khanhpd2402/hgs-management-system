using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementtations
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly HgsdbContext _context;

        public AttendanceRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<Attendance> AddAttendance(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task<Attendance> UpdateAttendance(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task<Attendance> GetAttendanceById(int attendanceId)
        {
            return await _context.Attendances.FindAsync(attendanceId);
        }

        public async Task<List<Attendance>> GetAttendancesByClass(int classId, DateOnly date, string shift)
        {
            return await _context.Attendances
                .Where(a => a.Student.StudentClasses.Any(sc => sc.ClassId == classId)
                         && a.Date == date
                         && a.Shift == shift)
                .ToListAsync();
        }

        public async Task<List<string>> GetParentPhoneNumbers(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Parent)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null || student.Parent == null)
            {
                throw new Exception($"Không tìm thấy phụ huynh cho học sinh với ID {studentId}.");
            }

            var phoneNumbers = new List<string>();

            // Lấy số điện thoại từ Parent (Father, Mother, Guardian)
            if (!string.IsNullOrEmpty(student.Parent.PhoneNumberFather))
                phoneNumbers.Add(student.Parent.PhoneNumberFather);
            if (!string.IsNullOrEmpty(student.Parent.PhoneNumberMother))
                phoneNumbers.Add(student.Parent.PhoneNumberMother);
            if (!string.IsNullOrEmpty(student.Parent.PhoneNumberGuardian))
                phoneNumbers.Add(student.Parent.PhoneNumberGuardian);

            // Nếu không có số điện thoại nào, lấy từ User nếu có
            if (!phoneNumbers.Any() && !string.IsNullOrEmpty(student.Parent.User?.PhoneNumber))
                phoneNumbers.Add(student.Parent.User.PhoneNumber);

            if (!phoneNumbers.Any())
            {
                throw new Exception($"Không tìm thấy số điện thoại phụ huynh cho học sinh với ID {studentId}.");
            }

            return phoneNumbers;
        }
    }
}