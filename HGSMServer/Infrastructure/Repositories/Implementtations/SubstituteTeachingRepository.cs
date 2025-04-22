using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementtations
{
    public class SubstituteTeachingRepository : ISubstituteTeachingRepository
    {
        private readonly HgsdbContext _context;

        public SubstituteTeachingRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<SubstituteTeaching> CreateAsync(SubstituteTeaching entity)
        {
            await _context.SubstituteTeachings.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<SubstituteTeaching?> GetByTimetableDetailAndDateAsync(int timetableDetailId, DateOnly date)
        {
            return await _context.SubstituteTeachings
                .FirstOrDefaultAsync(x => x.TimetableDetailId == timetableDetailId && x.Date == date);
        }

        public async Task<SubstituteTeaching> GetByIdAsync(int substituteId)
        {
            return await _context.SubstituteTeachings
                .Include(st => st.TimetableDetail)
                    .ThenInclude(td => td.Class)
                .Include(st => st.TimetableDetail)
                    .ThenInclude(td => td.Subject)
                .Include(st => st.TimetableDetail)
                    .ThenInclude(td => td.Period)
                .Include(st => st.OriginalTeacher)
                .Include(st => st.SubstituteTeacher)
                .FirstOrDefaultAsync(st => st.SubstituteId == substituteId);
        }

        public async Task<IEnumerable<SubstituteTeaching>> GetAllAsync(int? timetableDetailId = null, int? OriginalTeacherId = null, int? SubstituteTeacherId = null, DateOnly? date = null)
        {
            var query = _context.SubstituteTeachings
                .Include(st => st.TimetableDetail)
                    .ThenInclude(td => td.Class)
                .Include(st => st.TimetableDetail)
                    .ThenInclude(td => td.Subject)
                .Include(st => st.TimetableDetail)
                    .ThenInclude(td => td.Period)
                .Include(st => st.OriginalTeacher)
                .Include(st => st.SubstituteTeacher)
                .AsQueryable();

            if (timetableDetailId.HasValue)
                query = query.Where(st => st.TimetableDetailId == timetableDetailId.Value);

            if (OriginalTeacherId.HasValue)
                query = query.Where(st => st.OriginalTeacherId == OriginalTeacherId.Value);
            if (SubstituteTeacherId.HasValue)
                query = query.Where(st => st.OriginalTeacherId == SubstituteTeacherId.Value);

            if (date.HasValue)
                query = query.Where(st => st.Date == date.Value);

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(SubstituteTeaching entity)
        {
            _context.SubstituteTeachings.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int substituteId)
        {
            var entity = await _context.SubstituteTeachings.FindAsync(substituteId);
            if (entity != null)
            {
                _context.SubstituteTeachings.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
