using Application.Features.Users.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Features.Users.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserDTO?> GetUserByEmailAsync(string email);
        Task<UserDTO?> GetUserByUsernameAsync(string username);
        Task AddUserAsync(CreateUserDTO userDto);
        Task UpdateUserAsync(UpdateUserDTO userDto);
        Task DeleteUserAsync(int id);
        Task<string?> GetRoleNameByRoleIdAsync(int roleId);
        Task ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task AdminChangePasswordAsync(int userId, string newPassword);
        Task ChangeUserStatusAsync(int userId, string newStatus);
    }
}