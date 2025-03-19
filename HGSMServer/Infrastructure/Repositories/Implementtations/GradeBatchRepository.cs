using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Repositories.Interfaces;

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
            return await _context.GradeBatches.ToListAsync();
        }

        public async Task<GradeBatch?> GetByIdAsync(int id)
        {
            return await _context.GradeBatches.FindAsync(id);
        }

        public async Task<GradeBatch> AddAsync(GradeBatch gradeBatch)
        {
            _context.GradeBatches.Add(gradeBatch);
            await _context.SaveChangesAsync();
            return gradeBatch;
        }

        public async Task<bool> UpdateAsync(GradeBatch gradeBatch)
        {
            var existingBatch = await _context.GradeBatches.FindAsync(gradeBatch.BatchId);
            if (existingBatch == null) return false;

            _context.Entry(existingBatch).CurrentValues.SetValues(gradeBatch);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var batch = await _context.GradeBatches.FindAsync(id);
            if (batch == null) return false;

            _context.GradeBatches.Remove(batch);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
