using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IGradeLevelRepository
    {
        Task<IEnumerable<GradeLevel>> GetAllAsync();
        Task<GradeLevel> GetByIdAsync(int id);
        Task<GradeLevel> CreateAsync(GradeLevel entity);
        Task UpdateAsync(GradeLevel entity);
        Task DeleteAsync(int id);
    }
}
