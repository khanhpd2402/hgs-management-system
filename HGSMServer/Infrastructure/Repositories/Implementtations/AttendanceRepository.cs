﻿using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<string> GetParentPhoneNumber(int studentId)
        {
            return await _context.Students
                .Where(s => s.StudentId == studentId)
                .SelectMany(s => s.Parents)
                .Select(p => p.User.PhoneNumber)
                .FirstOrDefaultAsync();
        }
    }
}
