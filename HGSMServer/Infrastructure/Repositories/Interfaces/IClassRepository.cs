using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IClassRepository
    {
        Task<IEnumerable<Class>> GetAllAsync();
        Task<IEnumerable<Class>> GetAllActiveAsync(string? status = null);
        Task<Class> GetByIdAsync(int id);
        Task<Class> AddAsync(Class classEntity);
        Task<Class> UpdateAsync(Class classEntity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int classId);
        Task<Class> GetClassByNameAsync(string className);
    }
}
