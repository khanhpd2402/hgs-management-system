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

        public async Task<List<Attendance>> GetByWeekAsync(int studentClassId, DateOnly weekStart)
        {
            var weekDates = Enumerable.Range(0, 6).Select(i => weekStart.AddDays(i)).ToList();
            return await _context.Attendances
                .Where(a => a.StudentClassId == studentClassId && weekDates.Contains(a.Date))
                .ToListAsync();
        }

        public async Task<Attendance?> GetAsync(int studentClassId, DateOnly date, string session)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.StudentClassId == studentClassId && a.Date == date && a.Session == session);
        }

        public async Task AddRangeAsync(IEnumerable<Attendance> attendances)
        {
            _context.Attendances.AddRangeAsync(attendances);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Attendance> attendances)
        {
            _context.Attendances.UpdateRange(attendances);
            await _context.SaveChangesAsync();
        }
    }

}