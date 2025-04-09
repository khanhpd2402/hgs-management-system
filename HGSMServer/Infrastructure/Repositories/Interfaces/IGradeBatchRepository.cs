using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IGradeBatchRepository
    {
        Task<GradeBatch?> GetByIdAsync(int id);
        Task<IEnumerable<GradeBatch>> GetAllAsync();
        Task<GradeBatch> AddAsync(GradeBatch entity);
        Task<GradeBatch> UpdateAsync(GradeBatch entity);
        Task DeleteAsync(int id);
    }
}

