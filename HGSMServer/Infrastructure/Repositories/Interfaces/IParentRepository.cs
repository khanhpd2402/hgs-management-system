using Domain.Models;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IParentRepository
    {
        Task AddAsync(Parent parent);
        Task<Parent> GetByIdAsync(int parentId);
        Task<Parent> GetParentByUserIdAsync(int userId);
        Task<Parent> GetParentByDetailsAsync(string fullName, DateOnly? dob, string phoneNumber, string email, string idcardNumber);
        Task UpdateAsync(Parent parent);
    }
}