using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementtations
{
    public class TimetableRepository : ITimetableRepository
    {
        private readonly HgsdbContext _context;

        public TimetableRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TimetableDetail>> GetByStudentIdAsync(int studentId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var query = _context.TimetableDetails
                .Include(x => x.Timetable)
                .Where(x => _context.StudentClasses.Any(sc => sc.ClassId == x.ClassId && sc.StudentId == studentId));

            if (semesterId.HasValue)
                query = query.Where(x => x.Timetable.SemesterId == semesterId);

            if (effectiveDate.HasValue)
                query = query.Where(x => x.Timetable.EffectiveDate == effectiveDate);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TimetableDetail>> GetByTeacherIdAsync(int teacherId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var query = _context.TimetableDetails
                .Include(x => x.Timetable)
                .Where(x => x.TeacherId == teacherId);

            if (semesterId.HasValue)
                query = query.Where(x => x.Timetable.SemesterId == semesterId);

            if (effectiveDate.HasValue)
                query = query.Where(x => x.Timetable.EffectiveDate == effectiveDate);

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateDetailAsync(TimetableDetail detail)
        {
            _context.TimetableDetails.Update(detail);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteDetailAsync(int detailId)
        {
            var detail = await _context.TimetableDetails.FindAsync(detailId);
            if (detail == null) return false;
            _context.TimetableDetails.Remove(detail);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsConflictAsync(TimetableDetail detail)
        {
            return await _context.TimetableDetails.AnyAsync(x =>
                x.ClassId == detail.ClassId &&
                x.DayOfWeek == detail.DayOfWeek &&
                x.Shift == detail.Shift &&
                x.Period == detail.Period &&
                x.Timetable.EffectiveDate == detail.Timetable.EffectiveDate &&
                x.Timetable.SemesterId == detail.Timetable.SemesterId &&
                x.TimetableId == detail.TimetableId);
        }

        public async Task<Timetable> AddAsync(Timetable timetable)
        {
            _context.Timetables.Add(timetable);
            await _context.SaveChangesAsync();
            return timetable;
        }

    }
}
