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
        //private readonly HgsdbContext _context;
        //public AttendanceRepository(HgsdbContext context) => _context = context;

        //public async Task<List<Attendance>> GetByWeekAsync(int classId, DateOnly weekStart)
        //{
        //    var weekDates = Enumerable.Range(0, 6).Select(i => weekStart.AddDays(i)).ToList();

        //    return await _context.Attendances
        //        .Where(a => weekDates.Contains(a.Date) &&
        //                    _context.Students.Any(s => s.StudentId == a.StudentId && s.ClassID == classId))
        //        .Include(a => a.Student)
        //        .ToListAsync();
        //}

        //public async Task AddRangeAsync(List<Attendance> attendances)
        //{
        //    await _context.Attendances.AddRangeAsync(attendances);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task UpdateRangeAsync(List<Attendance> updates)
        //{
        //    foreach (var updated in updates)
        //    {
        //        var existing = await _context.Attendances.FindAsync(updated.AttendanceId);
        //        if (existing == null || existing.Date.Date != DateTime.Today) continue;

        //        existing.Status = updated.Status;
        //        existing.Note = updated.Note;
        //    }

        //    await _context.SaveChangesAsync();
        //}

        //public async Task<bool> ExistsForSessionAsync(int studentId, DateTime date, string session)
        //{
        //    return await _context.Attendances.AnyAsync(a =>
        //        a.StudentId == studentId &&
        //        a.Date == date &&
        //        a.Session == session);
        //}
    }
}