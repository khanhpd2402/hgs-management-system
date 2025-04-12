using Common.Constants;
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

        public async Task<IEnumerable<Timetable>> GetByClassIdAsync(int classId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var query = _context.Timetables
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Period)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Subject)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Teacher)
                .Where(t => t.TimetableDetails.Any(td => td.ClassId == classId))
                .Where(t => t.Status == AppConstants.Status.ACTIVE);

            if (semesterId.HasValue)
                query = query.Where(t => t.SemesterId == semesterId.Value);

            if (effectiveDate.HasValue)
                query = query.Where(t => t.EffectiveDate == effectiveDate.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Timetable>> GetByStudentIdAsync(int studentId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var query = from t in _context.Timetables
                        join td in _context.TimetableDetails on t.TimetableId equals td.TimetableId
                        join sc in _context.StudentClasses on td.ClassId equals sc.ClassId
                        where sc.StudentId == studentId  && t.Status == AppConstants.Status.ACTIVE 
                        select t;

            query = query
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Period)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Subject)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Teacher)
                .Distinct();

            if (semesterId.HasValue)
                query = query.Where(t => t.SemesterId == semesterId.Value);

            if (effectiveDate.HasValue)
                query = query.Where(t => t.EffectiveDate == effectiveDate.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Timetable>> GetByTeacherIdAsync(int teacherId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var query = _context.Timetables
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Period)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Subject)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Teacher)
                .Where(t => t.TimetableDetails.Any(td => td.TeacherId == teacherId))
                .Where(t => t.Status == AppConstants.Status.ACTIVE);

            if (semesterId.HasValue)
                query = query.Where(t => t.SemesterId == semesterId.Value);

            if (effectiveDate.HasValue)
                query = query.Where(t => t.EffectiveDate == effectiveDate.Value);

            return await query.ToListAsync();
        }

        public async Task<Timetable> GetByIdAsync(int timetableId)
        {
            return await _context.Timetables
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Period)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Subject)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Teacher)
                .FirstOrDefaultAsync(t => t.TimetableId == timetableId);
        }
        public async Task UpdateTimetableAsync(Timetable timetable)
        {
            _context.Timetables.Update(timetable);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateMultipleDetailsAsync(List<TimetableDetail> details)
        {
            if (details == null || !details.Any())
                return false;

            _context.TimetableDetails.UpdateRange(details);
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
                x.PeriodId == detail.PeriodId && 
                x.Date == detail.Date &&
                x.TimetableId == detail.TimetableId &&
                x.Timetable.EffectiveDate == detail.Timetable.EffectiveDate &&
                x.Timetable.SemesterId == detail.Timetable.SemesterId);
        }

        public async Task<Timetable> CreateTimetableAsync(Timetable timetable)
        {
            _context.Timetables.Add(timetable);
            await _context.SaveChangesAsync();
            return timetable;
        }

        public async Task<TimetableDetail> AddTimetableDetailAsync(TimetableDetail detail)
        {
            _context.TimetableDetails.Add(detail);
            await _context.SaveChangesAsync();
            return detail;
        }
    }
}
