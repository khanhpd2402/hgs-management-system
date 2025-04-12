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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly HgsdbContext _context;

        public SubjectRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<Subject> GetByIdAsync(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task<Subject> CreateAsync(Subject entity)
        {
            // Kiểm tra UNIQUE constraint trước khi thêm
            if (await _context.Subjects.AnyAsync(s => s.SubjectName == entity.SubjectName))
            {
                throw new InvalidOperationException($"Subject with name '{entity.SubjectName}' already exists.");
            }

            _context.Subjects.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Subject entity)
        {
            // Kiểm tra UNIQUE constraint cho SubjectName
            if (await _context.Subjects.AnyAsync(s => s.SubjectName == entity.SubjectName && s.SubjectId != entity.SubjectId))
            {
                throw new InvalidOperationException($"Subject with name '{entity.SubjectName}' already exists.");
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Subjects.FindAsync(id);
            if (entity != null)
            {
                _context.Subjects.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Subject> GetByNameAsync(string subjectName)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectName == subjectName);
        }
    }

}
