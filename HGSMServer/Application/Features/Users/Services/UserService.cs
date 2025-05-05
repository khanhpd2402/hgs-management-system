using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Common.Utils;
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
        private readonly EmailService _emailService;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ITeacherRepository teacherRepository,
            IParentRepository parentRepository,
            IStudentRepository studentRepository,
            EmailService emailService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
            _parentRepository = parentRepository ?? throw new ArgumentNullException(nameof(parentRepository));
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersWithTeacherAndParentInfoAsync();
            var userDTOs = new List<UserDTO>();
            foreach (var u in users)
            {
                var role = await _roleRepository.GetRoleByIdAsync(u.RoleId);
                userDTOs.Add(new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    RoleId = u.RoleId,
                    RoleName = role?.RoleName,
                    Status = u.Status,
                    PasswordHash = u.PasswordHash,
                    FullName = u.Teacher?.FullName ?? u.Parent?.FullNameFather ?? u.Parent?.FullNameMother ?? u.Parent?.FullNameGuardian
                });
            }
            return userDTOs;
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserWithTeacherAndParentInfoAsync(id);
            if (user == null) return null;

            var role = await _roleRepository.GetRoleByIdAsync(user.RoleId);
            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                RoleName = role?.RoleName,
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
            var role = await _roleRepository.GetRoleByIdAsync(userWithDetails.RoleId);
            return new UserDTO
            {
                UserId = userWithDetails.UserId,
                Username = userWithDetails.Username,
                Email = userWithDetails.Email,
                PhoneNumber = userWithDetails.PhoneNumber,
                RoleId = userWithDetails.RoleId,
                RoleName = role?.RoleName,
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
            var role = await _roleRepository.GetRoleByIdAsync(userWithDetails.RoleId);
            return new UserDTO
            {
                UserId = userWithDetails.UserId,
                Username = userWithDetails.Username,
                Email = userWithDetails.Email,
                PhoneNumber = userWithDetails.PhoneNumber,
                RoleId = userWithDetails.RoleId,
                RoleName = role?.RoleName,
                Status = userWithDetails.Status,
                PasswordHash = userWithDetails.PasswordHash,
                FullName = userWithDetails.Teacher?.FullName ?? userWithDetails.Parent?.FullNameFather ?? userWithDetails.Parent?.FullNameMother ?? userWithDetails.Parent?.FullNameGuardian
            };
        }

        public async Task<UserDTO> AddUserAsync(CreateUserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            string passwordHash = BCrypt.Net.BCrypt.HashPassword("12345678");

            string? roleName = await GetRoleNameByRoleIdAsync(userDto.RoleId);
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentException("Vai trò không tồn tại.");

            if (!roleName.Equals("Phụ huynh", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(userDto.FullName))
                    throw new ArgumentException("Họ và tên là bắt buộc cho các vai trò không phải phụ huynh.");
                if (!userDto.DOB.HasValue)
                    throw new ArgumentException("Ngày sinh là bắt buộc cho các vai trò không phải phụ huynh.");
                if (string.IsNullOrEmpty(userDto.Gender))
                    throw new ArgumentException("Giới tính là bắt buộc cho các vai trò không phải phụ huynh.");
                if (!userDto.SchoolJoinDate.HasValue)
                    throw new ArgumentException("Ngày vào trường là bắt buộc cho các vai trò không phải phụ huynh.");
            }

            var user = new User
            {
                Username = "tempuser",
                PasswordHash = passwordHash,
                Email = roleName.Equals("Phụ huynh", StringComparison.OrdinalIgnoreCase)
                    ? (userDto.Email ?? userDto.EmailFather ?? userDto.EmailMother ?? userDto.EmailGuardian)
                    : userDto.Email,
                PhoneNumber = roleName.Equals("Phụ huynh", StringComparison.OrdinalIgnoreCase)
                    ? (userDto.PhoneNumber ?? userDto.PhoneNumberFather ?? userDto.PhoneNumberMother ?? userDto.PhoneNumberGuardian)
                    : userDto.PhoneNumber,
                RoleId = userDto.RoleId,
                Status = "Hoạt động"
            };

            if (!string.IsNullOrEmpty(user.Email))
            {
                var existingUserByEmail = await _userRepository.GetByEmailAsync(user.Email);
                if (existingUserByEmail != null)
                {
                    throw new Exception("Email đã được sử dụng.");
                }
            }

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(user.PhoneNumber);
                if (existingUserByPhone != null)
                {
                    throw new Exception("Số điện thoại đã được sử dụng.");
                }
            }

            await _userRepository.AddAsync(user);
            Console.WriteLine("Created new user successfully.");

            string fullNameForUsername = roleName.Equals("Phụ huynh", StringComparison.OrdinalIgnoreCase)
                ? (userDto.FullNameFather ?? userDto.FullNameMother ?? userDto.FullNameGuardian ?? "user")
                : userDto.FullName;
            string finalUsername = FormatUserName.GenerateUsername(fullNameForUsername, user.UserId);
            var existingUserByUsername = await _userRepository.GetByUsernameAsync(finalUsername);
            if (existingUserByUsername != null)
            {
                throw new Exception("Tên người dùng đã tồn tại.");
            }
            user.Username = finalUsername;
            await _userRepository.UpdateAsync(user);
            Console.WriteLine("Updated user with new username successfully.");

            if (!roleName.Equals("Phụ huynh", StringComparison.OrdinalIgnoreCase))
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
            else
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

            return await GetUserByIdAsync(user.UserId);
        }

        public async Task UpdateUserAsync(UpdateUserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            var user = await _userRepository.GetByIdForUpdateAsync(userDto.UserId);
            if (user == null)
                throw new ArgumentException("Người dùng không tồn tại.");

            var newRoleName = await GetRoleNameByRoleIdAsync(userDto.RoleId);
            if (string.IsNullOrEmpty(newRoleName))
                throw new ArgumentException("Vai trò không tồn tại.");

            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.RoleId = userDto.RoleId;

            if (!newRoleName.Equals("Phụ huynh", StringComparison.OrdinalIgnoreCase))
            {
                var teacher = await _teacherRepository.GetByUserIdAsync(userDto.UserId);
                if (teacher != null)
                {
                    teacher.IsHeadOfDepartment = newRoleName.Equals("Trưởng bộ môn", StringComparison.OrdinalIgnoreCase);
                    await _teacherRepository.UpdateAsync(teacher);
                }
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException("Người dùng không tồn tại.");

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
                throw new ArgumentException("Mật khẩu cũ và mới là bắt buộc.");

            if (changePasswordDto.NewPassword.Length < 8)
                throw new ArgumentException("Mật khẩu mới phải có ít nhất 8 ký tự.");

            var user = await _userRepository.GetByIdForUpdateAsync(userId);
            if (user == null)
                throw new ArgumentException("Người dùng không tồn tại.");

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Mật khẩu cũ không đúng.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateAsync(user);
        }

        public async Task AdminChangePasswordAsync(int userId, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentException("Mật khẩu mới là bắt buộc.");

            if (newPassword.Length < 8)
                throw new ArgumentException("Mật khẩu mới phải có ít nhất 8 ký tự.");

            var user = await _userRepository.GetByIdForUpdateAsync(userId);
            if (user == null)
                throw new ArgumentException("Người dùng không tồn tại.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            // Send email notification using EmailService
            if (!string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendAdminPasswordChangeNotificationAsync(user.Email, user.Username, newPassword);
            }
        }

        public async Task ChangeUserStatusAsync(int userId, string newStatus)
        {
            var user = await _userRepository.GetByIdForUpdateAsync(userId);
            if (user == null)
                throw new ArgumentException("Người dùng không tồn tại.");

            user.Status = newStatus;
            await _userRepository.UpdateAsync(user);

            // Send email notification to teacher if applicable
            if (!string.IsNullOrEmpty(user.Email))
            {
                var roleName = await GetRoleNameByRoleIdAsync(user.RoleId);
                if (!string.IsNullOrEmpty(roleName) && !roleName.Equals("Phụ huynh", StringComparison.OrdinalIgnoreCase))
                {
                    var teacher = await _teacherRepository.GetByUserIdAsync(userId);
                    if (teacher != null)
                    {
                        await _emailService.SendUserStatusChangeNotificationAsync(user.Email, teacher.FullName, newStatus);
                    }
                }
            }
        }
        public async Task ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email là bắt buộc.");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new ArgumentException("Email không tồn tại.");

            string newPassword = GenerateRandomPassword();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            if (!string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendForgotPasswordNotificationAsync(user.Email, user.Username, newPassword);
            }
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*-_+=";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}