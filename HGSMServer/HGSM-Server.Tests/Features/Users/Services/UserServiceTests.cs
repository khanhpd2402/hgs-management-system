using Application.Features.Users.DTOs;
using Application.Features.Users.Services;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Moq;
using Xunit;

namespace HGSM_Server.Tests.Features.Users.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
        private readonly Mock<IParentRepository> _parentRepositoryMock;
        private readonly Mock<IStudentRepository> _studentRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _teacherRepositoryMock = new Mock<ITeacherRepository>();
            _parentRepositoryMock = new Mock<IParentRepository>();
            _studentRepositoryMock = new Mock<IStudentRepository>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _teacherRepositoryMock.Object,
                _parentRepositoryMock.Object,
                _studentRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnUserList_WhenUsersExist()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, Username = "user1", Email = "user1@example.com", RoleId = 1, Teacher = new Teacher { FullName = "Teacher 1" } },
                new User { UserId = 2, Username = "user2", Email = "user2@example.com", RoleId = 6, Parent = new Parent { FullNameFather = "Father 2" } }
            };

            _userRepositoryMock.Setup(repo => repo.GetAllUsersWithTeacherAndParentInfoAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().FullName.Should().Be("Teacher 1");
            result.Last().FullName.Should().Be("Father 2");
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUser_WhenDataIsValid()
        {
            // Arrange
            var createUserDto = new CreateUserDTO
            {
                RoleId = 1,
                FullName = "Teacher 1",
                DOB = new DateTime(1980, 1, 1),
                Gender = "Nam",
                SchoolJoinDate = new DateTime(2020, 1, 1),
                Email = "teacher1@example.com",
                PhoneNumber = "0123456789"
            };
            var user = new User { UserId = 1, Username = "tempuser", RoleId = 1 };
            var role = new Role { RoleId = 1, RoleName = "Teacher" };

            _roleRepositoryMock.Setup(repo => repo.GetByIdAsync(createUserDto.RoleId))
                .ReturnsAsync(role);
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(createUserDto.Email))
                .ReturnsAsync((User)null);
            _userRepositoryMock.Setup(repo => repo.GetByPhoneNumberAsync(createUserDto.PhoneNumber))
                .ReturnsAsync((User)null);
            _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => u.UserId = 1);
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _userRepositoryMock.Setup(repo => repo.GetUserWithTeacherAndParentInfoAsync(1))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.AddUserAsync(createUserDto);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(1);
            _teacherRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Teacher>()), Times.Once());
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldChangePassword_WhenDataIsValid()
        {
            // Arrange
            int userId = 1;
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "12345678",
                NewPassword = "newpassword123"
            };
            var user = new User { UserId = userId, PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678") };

            _userRepositoryMock.Setup(repo => repo.GetByIdForUpdateAsync(userId))
                .ReturnsAsync(user);

            // Act
            await _userService.ChangePasswordAsync(userId, changePasswordDto);

            // Assert
            BCrypt.Net.BCrypt.Verify("newpassword123", user.PasswordHash).Should().BeTrue();
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(user), Times.Once());
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrowException_WhenOldPasswordIsIncorrect()
        {
            // Arrange
            int userId = 1;
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "wrongpassword",
                NewPassword = "newpassword123"
            };
            var user = new User { UserId = userId, PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678") };

            _userRepositoryMock.Setup(repo => repo.GetByIdForUpdateAsync(userId))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.ChangePasswordAsync(userId, changePasswordDto));
        }
    }
}