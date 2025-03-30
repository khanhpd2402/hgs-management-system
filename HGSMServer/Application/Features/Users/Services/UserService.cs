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
                FullName = u.Teacher?.FullName ?? u.Parent?.FullNameFather ?? u.Parent?.FullNameMother ?? u.Parent?.FullNameGuardian
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
                FullName = user.Teacher?.FullName ?? user.Parent?.FullNameFather ?? user.Parent?.FullNameMother ?? user.Parent?.FullNameGuardian
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
                FullName = userWithDetails.Teacher?.FullName ?? userWithDetails.Parent?.FullNameFather ?? userWithDetails.Parent?.FullNameMother ?? userWithDetails.Parent?.FullNameGuardian
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
                FullName = userWithDetails.Teacher?.FullName ?? userWithDetails.Parent?.FullNameFather ?? userWithDetails.Parent?.FullNameMother ?? userWithDetails.Parent?.FullNameGuardian
            };
        }

        public async Task<UserDTO> AddUserAsync(CreateUserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            // Mật khẩu mặc định "12345678"
            string passwordHash = BCrypt.Net.BCrypt.HashPassword("12345678");

            // Lấy RoleName từ RoleId
            string? roleName = await GetRoleNameByRoleIdAsync(userDto.RoleId);
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentException($"Role with ID {userDto.RoleId} not found.");

            // Kiểm tra các trường bắt buộc cho Teacher và các role khác
            if (userDto.RoleId != 6)
            {
                if (string.IsNullOrEmpty(userDto.FullName))
                    throw new ArgumentException("FullName is required for non-Parent roles.");
                if (!userDto.DOB.HasValue)
                    throw new ArgumentException("DOB is required for non-Parent roles.");
                if (string.IsNullOrEmpty(userDto.Gender))
                    throw new ArgumentException("Gender is required for non-Parent roles.");
                if (!userDto.SchoolJoinDate.HasValue)
                    throw new ArgumentException("SchoolJoinDate is required for non-Parent roles.");
            }

            // Tạo User với Username tạm thời
            var user = new User
            {
                Username = $"{roleName.ToLower()}temp", // Gán tạm để tránh lỗi NOT NULL
                PasswordHash = passwordHash,
                Email = userDto.RoleId == 6
                    ? (userDto.Email ?? userDto.EmailFather ?? userDto.EmailMother ?? userDto.EmailGuardian)
                    : userDto.Email,
                PhoneNumber = userDto.RoleId == 6
                    ? (userDto.PhoneNumber ?? userDto.PhoneNumberFather ?? userDto.PhoneNumberMother ?? userDto.PhoneNumberGuardian)
                    : userDto.PhoneNumber,
                RoleId = userDto.RoleId,
                Status = "Active"
            };

            // Kiểm tra trùng lặp Email nếu có
            if (!string.IsNullOrEmpty(user.Email))
            {
                var existingUserByEmail = await _userRepository.GetByEmailAsync(user.Email);
                if (existingUserByEmail != null)
                {
                    throw new Exception($"Email {user.Email} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByEmail.UserId}).");
                }
            }

            // Kiểm tra trùng lặp PhoneNumber nếu có
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(user.PhoneNumber);
                if (existingUserByPhone != null)
                {
                    throw new Exception($"Số điện thoại {user.PhoneNumber} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByPhone.UserId}).");
                }
            }

            // Thêm User để lấy UserId
            await _userRepository.AddAsync(user);
            Console.WriteLine($"Created new user with UserID: {user.UserId}");

            // Sinh Username theo RoleName + UserId
            string finalUsername = $"{roleName.ToLower()}{user.UserId}";
            var existingUserByUsername = await _userRepository.GetByUsernameAsync(finalUsername);
            if (existingUserByUsername != null)
            {
                throw new Exception($"Username {finalUsername} đã tồn tại (UserID: {existingUserByUsername.UserId}).");
            }
            user.Username = finalUsername;
            await _userRepository.UpdateAsync(user);
            Console.WriteLine($"Updated user with Username: {user.Username}");

            // Tạo bản ghi Teacher hoặc Parent
            if (userDto.RoleId != 6) // Tất cả role ngoài Parent đều vào bảng Teacher
            {
                var teacher = new Teacher
                {
                    UserId = user.UserId,
                    FullName = userDto.FullName,
                    Dob = DateOnly.FromDateTime(userDto.DOB.Value),
                    Gender = userDto.Gender,
                    SchoolJoinDate = DateOnly.FromDateTime(userDto.SchoolJoinDate.Value),
                    Ethnicity = userDto.Ethnicity,
                    Religion = userDto.Religion,
                    MaritalStatus = userDto.MaritalStatus,
                    IdcardNumber = userDto.IdcardNumber,
                    InsuranceNumber = userDto.InsuranceNumber,
                    EmploymentType = userDto.EmploymentType,
                    Position = userDto.Position,
                    Department = userDto.Department,
                    AdditionalDuties = userDto.AdditionalDuties,
                    IsHeadOfDepartment = userDto.IsHeadOfDepartment ?? false,
                    EmploymentStatus = userDto.EmploymentStatus,
                    RecruitmentAgency = userDto.RecruitmentAgency,
                    HiringDate = userDto.HiringDate.HasValue ? DateOnly.FromDateTime(userDto.HiringDate.Value) : null,
                    PermanentEmploymentDate = userDto.PermanentEmploymentDate.HasValue ? DateOnly.FromDateTime(userDto.PermanentEmploymentDate.Value) : null,
                    PermanentAddress = userDto.PermanentAddress,
                    Hometown = userDto.Hometown
                };
                await _teacherRepository.AddAsync(teacher);
            }
            else // Parent
            {
                var parent = new Parent
                {
                    UserId = user.UserId,
                    FullNameFather = userDto.FullNameFather,
                    YearOfBirthFather = userDto.YearOfBirthFather.HasValue ? DateOnly.FromDateTime(userDto.YearOfBirthFather.Value) : null,
                    OccupationFather = userDto.OccupationFather,
                    PhoneNumberFather = userDto.PhoneNumberFather,
                    EmailFather = userDto.EmailFather,
                    IdcardNumberFather = userDto.IdcardNumberFather,
                    FullNameMother = userDto.FullNameMother,
                    YearOfBirthMother = userDto.YearOfBirthMother.HasValue ? DateOnly.FromDateTime(userDto.YearOfBirthMother.Value) : null,
                    OccupationMother = userDto.OccupationMother,
                    PhoneNumberMother = userDto.PhoneNumberMother,
                    EmailMother = userDto.EmailMother,
                    IdcardNumberMother = userDto.IdcardNumberMother,
                    FullNameGuardian = userDto.FullNameGuardian,
                    YearOfBirthGuardian = userDto.YearOfBirthGuardian.HasValue ? DateOnly.FromDateTime(userDto.YearOfBirthGuardian.Value) : null,
                    OccupationGuardian = userDto.OccupationGuardian,
                    PhoneNumberGuardian = userDto.PhoneNumberGuardian,
                    EmailGuardian = userDto.EmailGuardian,
                    IdcardNumberGuardian = userDto.IdcardNumberGuardian
                };
                await _parentRepository.AddAsync(parent);
            }

            // Trả về thông tin người dùng vừa tạo
            return await GetUserByIdAsync(user.UserId);
        }

        public async Task UpdateUserAsync(UpdateUserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            var user = await _userRepository.GetByIdForUpdateAsync(userDto.UserId);
            if (user == null)
                throw new ArgumentException($"User with ID {userDto.UserId} not found.");

            // Không cho phép cập nhật Username vì nó được sinh tự động
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