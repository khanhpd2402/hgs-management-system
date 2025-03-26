using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations // Sửa namespace
{
    public class AcademicYearRepository : IAcademicYearRepository
    {
        private readonly HgsdbContext _context;

        public AcademicYearRepository(HgsdbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<AcademicYear>> GetAllAsync()
        {
            return await _context.AcademicYears.AsNoTracking().ToListAsync();
        }

        public async Task<AcademicYear?> GetByIdAsync(int id)
        {
            return await _context.AcademicYears.AsNoTracking().FirstOrDefaultAsync(ay => ay.AcademicYearId == id);
        }

        public async Task AddAsync(AcademicYear academicYear)
        {
            await _context.AcademicYears.AddAsync(academicYear);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AcademicYear academicYear)
        {
            _context.AcademicYears.Update(academicYear);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear != null)
            {
                _context.AcademicYears.Remove(academicYear);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<AcademicYear?> GetCurrentAcademicYearAsync()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.AcademicYears
                .AsNoTracking()
                .Where(ay => ay.StartDate <= currentDate && ay.EndDate >= currentDate)
                .FirstOrDefaultAsync();
        }
    }
}