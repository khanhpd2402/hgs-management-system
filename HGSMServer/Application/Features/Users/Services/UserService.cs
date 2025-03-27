using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.Implementations;
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
        private readonly ITeacherRepository _teacherRepository;
        private readonly IParentRepository _parentRepository;
        private readonly IStudentRepository _studentRepository;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ITeacherRepository teacherRepository,
            IParentRepository parentRepository,
            IStudentRepository studentRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
            _parentRepository = parentRepository ?? throw new ArgumentNullException(nameof(parentRepository));
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersWithTeacherAndParentInfoAsync();
            return users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                Status = u.Status,
                PasswordHash = u.PasswordHash,
                FullName = u.Teacher?.FullName ?? u.Parent?.FullName
            }).ToList();
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserWithTeacherAndParentInfoAsync(id);
            if (user == null) return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                Status = user.Status,
                PasswordHash = user.PasswordHash,
                FullName = user.Teacher?.FullName ?? user.Parent?.FullName
            };
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            var userWithDetails = await _userRepository.GetUserWithTeacherAndParentInfoAsync(user.UserId);
            return new UserDTO
            {
                UserId = userWithDetails.UserId,
                Username = userWithDetails.Username,
                Email = userWithDetails.Email,
                PhoneNumber = userWithDetails.PhoneNumber,
                RoleId = userWithDetails.RoleId,
                Status = userWithDetails.Status,
                PasswordHash = userWithDetails.PasswordHash,
                FullName = userWithDetails.Teacher?.FullName ?? userWithDetails.Parent?.FullName
            };
        }

        public async Task<UserDTO?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            var userWithDetails = await _userRepository.GetUserWithTeacherAndParentInfoAsync(user.UserId);
            return new UserDTO
            {
                UserId = userWithDetails.UserId,
                Username = userWithDetails.Username,
                Email = userWithDetails.Email,
                PhoneNumber = userWithDetails.PhoneNumber,
                RoleId = userWithDetails.RoleId,
                Status = userWithDetails.Status,
                PasswordHash = userWithDetails.PasswordHash,
                FullName = userWithDetails.Teacher?.FullName ?? userWithDetails.Parent?.FullName
            };
        }

        public async Task AddUserAsync(CreateUserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            if (string.IsNullOrEmpty(userDto.PasswordHash))
                throw new ArgumentException("Password is required.");

            if (userDto.PasswordHash.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.");

            // Tạo User
            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash),
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                RoleId = userDto.RoleId,
                Status = "Active"
            };

            await _userRepository.AddAsync(user);

            // Kiểm tra RoleId để quyết định tạo bản ghi trong Teachers hoặc Parents
            if (userDto.RoleId != 6) // 6 là RoleID của Parent
            {
                var teacher = new Teacher
                {
                    UserId = user.UserId,
                    FullName = userDto.FullName,
                    Dob = DateOnly.FromDateTime(userDto.DOB),
                    Gender = userDto.Gender,
                    SchoolJoinDate = DateOnly.FromDateTime(userDto.SchoolJoinDate),
                    Ethnicity = null,
                    Religion = null,
                    MaritalStatus = null,
                    IdcardNumber = null,
                    InsuranceNumber = null,
                    EmploymentType = null,
                    Position = null,
                    Department = null,
                    AdditionalDuties = null,
                    IsHeadOfDepartment = false,
                    EmploymentStatus = null,
                    RecruitmentAgency = null,
                    HiringDate = null,
                    PermanentEmploymentDate = null,
                    PermanentAddress = null,
                    Hometown = null
                };

                await _teacherRepository.AddAsync(teacher);
            }
            else // RoleId = 6 (Parent)
            {
                var parent = new Parent
                {
                    UserId = user.UserId,
                    FullName = userDto.FullName,
                    Dob = userDto.DOB != default ? DateOnly.FromDateTime(userDto.DOB) : null,
                    Occupation = null
                };

                await _parentRepository.AddAsync(parent);
            }
        }

        public async Task UpdateUserAsync(UpdateUserDTO userDto)
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

        public async Task ChangeUserStatusAsync(int userId, string newStatus)
        {
            var user = await _userRepository.GetByIdForUpdateAsync(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found.");

            user.Status = newStatus;
            await _userRepository.UpdateAsync(user);
        }

        
    }
}