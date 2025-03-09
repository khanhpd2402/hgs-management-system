using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementtations
{
    public class TimetableRepository : ITimetableRepository
    {
        private readonly HgsdbContext _context;

        public TimetableRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<List<Timetable>> GetTimetableByClassAsync(int classId, DateOnly effectiveDate)
        {
            return await _context.Timetables
                .Include(t => t.Class)
                .Include(t => t.Subject)
                .Include(t => t.Teacher)
                .Where(t => t.ClassId == classId && t.EffectiveDate == effectiveDate)
                .ToListAsync();
        }

        public async Task<Timetable?> GetTimetableByIdAsync(int timetableId)
        {
            return await _context.Timetables
                .Include(t => t.Class)
                .Include(t => t.Subject)
                .Include(t => t.Teacher)
                .FirstOrDefaultAsync(t => t.TimetableId == timetableId);
        }

        public async Task<Timetable> AddTimetableAsync(Timetable timetable)
        {
            _context.Timetables.Add(timetable);
            await _context.SaveChangesAsync();
            return timetable;
        }

        public async Task<bool> UpdateTimetableAsync(Timetable timetable)
        {
            _context.Timetables.Update(timetable);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTimetableAsync(int timetableId)
        {
            var timetable = await _context.Timetables.FindAsync(timetableId);
            if (timetable == null) return false;

            _context.Timetables.Remove(timetable);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
