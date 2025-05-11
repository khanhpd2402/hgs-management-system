using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories.Interfaces;
using Common.Constants;

namespace Infrastructure.Repositories
{
    public class GradeBatchRepository : IGradeBatchRepository
    {
        private readonly HgsdbContext _context;

        public GradeBatchRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GradeBatch>> GetAllAsync()
        {
            return await _context.GradeBatches.Include(gb => gb.Semester).ToListAsync();
        }

        public async Task<GradeBatch?> GetByIdAsync(int id)
        {
            return await _context.GradeBatches.FindAsync(id);
        }
        public async Task<GradeBatch?> GetActiveAsync() 
        {
            var activeBatch = await _context.GradeBatches
                .Include(gb => gb.Semester) 
                .FirstOrDefaultAsync(g => g.Status == AppConstants.Status.ACTIVE);
            return activeBatch;
        }

        public async Task<GradeBatch> AddAsync(GradeBatch entity)
        {
            _context.GradeBatches.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<GradeBatch> UpdateAsync(GradeBatch entity)
        {
            _context.GradeBatches.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
       
    }
}
