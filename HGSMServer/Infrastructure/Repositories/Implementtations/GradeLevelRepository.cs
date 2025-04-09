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
    public class GradeLevelRepository : IGradeLevelRepository
    {
        private readonly HgsdbContext _context;

        public GradeLevelRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GradeLevel>> GetAllAsync()
        {
            return await _context.GradeLevels.ToListAsync();
        }

        public async Task<GradeLevel> GetByIdAsync(int id)
        {
            return await _context.GradeLevels.FindAsync(id);
        }

        public async Task<GradeLevel> CreateAsync(GradeLevel entity)
        {
            _context.GradeLevels.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(GradeLevel entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.GradeLevels.FindAsync(id);
            if (entity != null)
            {
                _context.GradeLevels.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
