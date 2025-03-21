﻿using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.Implementtations;
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
        private readonly IRoleRepository _roleRepository; // Thêm repository cho Role

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
                PasswordHash = u.PasswordHash,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                Status = u.Status
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
                PasswordHash = user.PasswordHash,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                Status = user.Status
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
                PasswordHash = user.PasswordHash,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                Status = user.Status
            };
        }

        public async Task AddUserAsync(UserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = userDto.PasswordHash,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                RoleId = userDto.RoleId,
                Status = userDto.Status
            };

            await _userRepository.AddAsync(user);
            userDto.UserId = user.UserId; // Cập nhật UserId cho DTO sau khi thêm
        }

        public async Task UpdateUserAsync(UserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            var user = await _userRepository.GetByIdAsync(userDto.UserId);
            if (user == null)
                throw new ArgumentException($"User with ID {userDto.UserId} not found.");

            user.Username = userDto.Username;
            user.PasswordHash = userDto.PasswordHash;
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
            return role?.RoleName; // Trả về tên role (ví dụ: "Principal")
        }
    }
}