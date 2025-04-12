using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IPeriodRepository
    {
        Task<IEnumerable<Period>> GetAllAsync();
        Task<Period> GetByIdAsync(int id);
        Task<Period> CreateAsync(Period entity);
        Task UpdateAsync(Period entity);
        Task DeleteAsync(int id);
    }
}
