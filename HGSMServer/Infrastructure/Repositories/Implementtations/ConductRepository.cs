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
    public class ConductRepository : IConductRepository
    {
        private readonly HgsdbContext _context;

        public ConductRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Conduct>> GetAllAsync()
        {
            return await _context.Conducts
                                 .Include(c => c.Student)   // Join với bảng Students nếu cần
                                 .Include(c => c.Semester)  // Join với bảng Semesters nếu cần
                                 .ToListAsync();
        }
        public async Task<Conduct> GetByIdAsync(int id)
        {
            return await _context.Conducts
                                 .Include(c => c.Student)
                                 .Include(c => c.Semester)
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Conduct> CreateAsync(Conduct conduct)
        {
            await _context.Conducts.AddAsync(conduct);
            await _context.SaveChangesAsync();
            return conduct;
        }
        public async Task<bool> UpdateAsync(int id, Conduct conduct)
        {
            var existingConduct = await GetByIdAsync(id);
            if (existingConduct == null)
            {
                return false;
            }

            existingConduct.ConductType = conduct.ConductType;
            existingConduct.Note = conduct.Note;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var conduct = await GetByIdAsync(id);
            if (conduct == null)
            {
                return false;
            }

            _context.Conducts.Remove(conduct);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
