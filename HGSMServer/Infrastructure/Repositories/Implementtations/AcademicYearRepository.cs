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
    public class AcademicYearRepository : IAcademicYearRepository
    {
        private readonly HgsdbContext _context;

        public AcademicYearRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<List<AcademicYear>> GetAllAsync()
        {
            return await _context.AcademicYears.ToListAsync();
        }

        public async Task<AcademicYear?> GetByIdAsync(int id)
        {
            return await _context.AcademicYears.FindAsync(id);
        }

        public async Task AddAsync(AcademicYear academicYear)
        {
            _context.AcademicYears.Add(academicYear);
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
    }

}
