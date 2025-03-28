using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersWithTeacherAndParentInfoAsync();
        Task<User> GetUserWithTeacherAndParentInfoAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByIdAsync(int? id);
        Task<User> GetByIdForUpdateAsync(int id);
        Task<int> GetUserCountAsync();
        Task<int> GetMaxUserIdAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}