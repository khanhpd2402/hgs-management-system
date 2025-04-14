﻿using Common.Constants;
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

        public async Task<IEnumerable<Timetable>> GetTimetablesForPrincipalAsync(int semesterId, string? status = null)
        {
            var query = _context.Timetables
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Period)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Subject)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Teacher)
                    .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Class)
                .Where(t => t.SemesterId == semesterId);

            var statusFilter = string.IsNullOrEmpty(status) ? AppConstants.Status.ACTIVE : status;
            query = query.Where(t => t.Status == statusFilter);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Timetable>> GetByStudentIdAsync(int studentId, int semesterId)
        {
            // Lấy ClassId của học sinh trong học kỳ
            var classId = await _context.StudentClasses
                .Join(_context.Semesters,
                      sc => sc.AcademicYearId,
                      s => s.AcademicYearId,
                      (sc, s) => new { sc.ClassId, sc.StudentId, s.SemesterId })
                .Where(x => x.StudentId == studentId && x.SemesterId == semesterId)
                .Select(x => x.ClassId)
                .FirstOrDefaultAsync();

            if (classId == 0)
                return Enumerable.Empty<Timetable>(); // Không tìm thấy lớp cho học sinh

            // Lấy Timetables dựa trên ClassId
            var query = _context.Timetables
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Period)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Subject)
                .Include(t => t.TimetableDetails)
                    .ThenInclude(td => td.Teacher)
                .Where(t => t.SemesterId == semesterId
                         && t.Status == AppConstants.Status.ACTIVE
                         && t.TimetableDetails.Any(td => td.ClassId == classId));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Timetable>> GetByTeacherIdAsync(int teacherId)
        {
            var query = _context.Timetables
                 .Include(t => t.TimetableDetails)
                     .ThenInclude(td => td.Period)
                 .Include(t => t.TimetableDetails)
                     .ThenInclude(td => td.Subject)
                 .Include(t => t.TimetableDetails)
                     .ThenInclude(td => td.Teacher)
                     .Include(t => t.TimetableDetails)
                     .ThenInclude(td => td.Class)
                 .Where(t => t.TimetableDetails.Any(td => td.TeacherId == teacherId))
                .Where(t => t.Status == AppConstants.Status.ACTIVE);

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Timetable>> GetTimetablesBySemesterAsync(int semesterId)
        {
            return await _context.Timetables
                .Where(t => t.SemesterId == semesterId)
                .ToListAsync();
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
                x.DayOfWeek == detail.DayOfWeek &&
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
