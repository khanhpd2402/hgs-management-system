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

        public async Task<List<SubstituteTeaching>> GetAllAsync()
        {
            return await _context.SubstituteTeachings
                .Include(s => s.OriginalTeacher)
                .Include(s => s.SubstituteTeacher)
                .Include(s => s.TimetableDetail)
                .ToListAsync();
        }

        public async Task<SubstituteTeaching?> GetByIdAsync(int id)
        {
            return await _context.SubstituteTeachings
                .Include(s => s.OriginalTeacher)
                .Include(s => s.SubstituteTeacher)
                .Include(s => s.TimetableDetail)
                .FirstOrDefaultAsync(s => s.SubstituteId == id);
        }

        public async Task<SubstituteTeaching> CreateAsync(SubstituteTeaching model)
        {
            _context.SubstituteTeachings.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<bool> UpdateAsync(int id, SubstituteTeaching model)
        {
            var existing = await _context.SubstituteTeachings.FindAsync(id);
            if (existing == null) return false;

            existing.TimetableDetailId = model.TimetableDetailId;
            existing.OriginalTeacherId = model.OriginalTeacherId;
            existing.SubstituteTeacherId = model.SubstituteTeacherId;
            existing.Date = model.Date;
            existing.Note = model.Note;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var model = await _context.SubstituteTeachings.FindAsync(id);
            if (model == null) return false;

            _context.SubstituteTeachings.Remove(model);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
