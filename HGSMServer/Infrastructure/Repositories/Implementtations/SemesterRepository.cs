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
    public class SemesterRepository : ISemesterRepository
    {
        private readonly HgsdbContext _context;

        public SemesterRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<List<Semester>> GetByAcademicYearIdAsync(int academicYearId)
        {
            return await _context.Semesters
                                 .Where(s => s.AcademicYearId == academicYearId)
                                 .ToListAsync();
        }

        public async Task<Semester?> GetByIdAsync(int id)
        {
            return await _context.Semesters.Include(s => s.AcademicYear)
                                           .FirstOrDefaultAsync(s => s.SemesterId == id);
        }

        public async Task AddAsync(Semester semester)
        {
            _context.Semesters.Add(semester);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);
            if (semester != null)
            {
                _context.Semesters.Remove(semester);
                await _context.SaveChangesAsync();
            }
        }
    }

}
