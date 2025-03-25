using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                Status = u.Status,
                PasswordHash = u.PasswordHash // Ánh xạ PasswordHash
            }).ToList();
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                Status = user.Status,
                PasswordHash = user.PasswordHash // Ánh xạ PasswordHash
            };
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                Status = user.Status,
                PasswordHash = user.PasswordHash // Ánh xạ PasswordHash
            };
        }

        public async Task<UserDTO?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                Status = user.Status,
                PasswordHash = user.PasswordHash // Ánh xạ PasswordHash
            };
        }

        public async Task AddUserAsync(UserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            if (string.IsNullOrEmpty(userDto.PasswordHash))
                throw new ArgumentException("Password is required.");

            if (userDto.PasswordHash.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.");

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash),
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                RoleId = userDto.RoleId,
                Status = userDto.Status ?? "Active"
            };

            await _userRepository.AddAsync(user);
            userDto.UserId = user.UserId;
        }

        public async Task UpdateUserAsync(UserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            var user = await _userRepository.GetByIdForUpdateAsync(userDto.UserId);
            if (user == null)
                throw new ArgumentException($"User with ID {userDto.UserId} not found.");

            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.RoleId = userDto.RoleId;
            user.Status = userDto.Status;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException($"User with ID {id} not found.");

            await _userRepository.DeleteAsync(id);
        }

        public async Task<string?> GetRoleNameByRoleIdAsync(int roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            return role?.RoleName;
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto == null)
                throw new ArgumentNullException(nameof(changePasswordDto));

            if (string.IsNullOrEmpty(changePasswordDto.OldPassword) || string.IsNullOrEmpty(changePasswordDto.NewPassword))
                throw new ArgumentException("Old password and new password are required.");

            if (changePasswordDto.NewPassword.Length < 8)
                throw new ArgumentException("New password must be at least 8 characters long.");

            var user = await _userRepository.GetByIdForUpdateAsync(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found.");

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Old password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateAsync(user);
        }

        public async Task AdminChangePasswordAsync(int userId, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentException("New password is required.");

            if (newPassword.Length < 8)
                throw new ArgumentException("New password must be at least 8 characters long.");

            var user = await _userRepository.GetByIdForUpdateAsync(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
        }
    }
}