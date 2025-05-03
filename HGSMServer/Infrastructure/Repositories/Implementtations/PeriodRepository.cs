using Common.Constants;
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
    public class PeriodRepository : IPeriodRepository
    {
        private readonly HgsdbContext _context;

        public PeriodRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Period>> GetAllAsync()
        {
            return await _context.Periods.ToListAsync();
        }

        public async Task<Period> GetByIdAsync(int id)
        {
            return await _context.Periods.FindAsync(id);
        }

        public async Task<Period> CreateAsync(Period entity)
        {
            _context.Periods.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Period entity)
        {        
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Periods.FindAsync(id);
            if (entity != null)
            {
                _context.Periods.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Period?> GetByPeriodNameAndShiftAsync(string periodName, byte shift)
        {
            if (string.IsNullOrWhiteSpace(periodName) || !AppConstants.Shift.All.Contains(shift))
            {
                return null;
            }

            string normalizedPeriodName = periodName.Trim();
            return await _context.Periods
                .FirstOrDefaultAsync(p => p.PeriodName == normalizedPeriodName && p.Shift == shift);
        }
    }
}
