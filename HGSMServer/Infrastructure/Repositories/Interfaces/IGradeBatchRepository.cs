using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IGradeBatchRepository
    {
        Task<IEnumerable<GradeBatch>> GetAllAsync();
        Task<GradeBatch?> GetByIdAsync(int id);
        Task<GradeBatch> AddAsync(GradeBatch gradeBatch);
        Task<bool> UpdateAsync(GradeBatch gradeBatch);
        Task<bool> DeleteAsync(int id);
    }
}
