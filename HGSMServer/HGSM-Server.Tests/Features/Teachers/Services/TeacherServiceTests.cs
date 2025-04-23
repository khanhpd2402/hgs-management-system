using Application.Features.Subjects.DTOs;
using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;
using AutoMapper;
using Common.Utils;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HGSM_Server.Tests.Features.Teachers.Services
{
    public class TeacherServiceTests
    {
        private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock; // Add IUserRepository
        private readonly Mock<ISubjectRepository> _subjectRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<HgsdbContext> _dbContextMock;
        private readonly Mock<IDbContextTransaction> _dbTransactionMock;
        private readonly TeacherService _teacherService;

        public TeacherServiceTests()
        {
            _teacherRepositoryMock = new Mock<ITeacherRepository>();
            _userRepositoryMock = new Mock<IUserRepository>(); 
            _subjectRepositoryMock = new Mock<ISubjectRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapperMock = new Mock<IMapper>();
            _dbContextMock = new Mock<HgsdbContext>(new DbContextOptions<HgsdbContext>());
            _dbTransactionMock = new Mock<IDbContextTransaction>();

            var databaseFacadeMock = new Mock<DatabaseFacade>(_dbContextMock.Object);
            databaseFacadeMock.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                              .ReturnsAsync(_dbTransactionMock.Object);
            databaseFacadeMock.Setup(d => d.BeginTransaction())
                              .Returns(_dbTransactionMock.Object);

            _dbContextMock.Setup(c => c.Database).Returns(databaseFacadeMock.Object);

            _teacherService = new TeacherService(
                _teacherRepositoryMock.Object,
                _userRepositoryMock.Object, 
                _mapperMock.Object,
                _subjectRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _dbContextMock.Object);
        }

        [Fact]
        public async Task GetAllTeachersAsync_ShouldReturnTeacherList_WhenTeachersExist()
        {
            var teachers = new List<Teacher>
            {
                new Teacher
                {
                    TeacherId = 1,
                    FullName = "Teacher 1",
                    User = new User { UserId= 101, Email = "teacher1@example.com", PhoneNumber = "0123456789" }
                },
                 new Teacher
                {
                    TeacherId = 2,
                    FullName = "Teacher 2",
                    User = new User { UserId= 102, Email = "teacher2@example.com", PhoneNumber = "0987654321" }
                }
            };
            var teacher1Subjects = new List<TeacherSubject>
            {
                new TeacherSubject { TeacherId = 1, Subject = new Subject { SubjectId = 1, SubjectName = "Math" }, IsMainSubject = true }
            };
            var teacher2Subjects = new List<TeacherSubject>
            {
                new TeacherSubject { TeacherId = 2, Subject = new Subject { SubjectId = 2, SubjectName = "Physics" }, IsMainSubject = false }
            };

            var teacherDtos = new List<TeacherListDto>
            {
                new TeacherListDto
                {
                    TeacherId = 1,
                    FullName = "Teacher 1",
                    Email = "teacher1@example.com",
                    PhoneNumber = "0123456789",
                    Subjects = new List<SubjectTeacherDto> { new SubjectTeacherDto { SubjectName = "Math", IsMainSubject = true } }
                },
                 new TeacherListDto
                {
                    TeacherId = 2,
                    FullName = "Teacher 2",
                    Email = "teacher2@example.com",
                    PhoneNumber = "0987654321",
                    Subjects = new List<SubjectTeacherDto> { new SubjectTeacherDto { SubjectName = "Physics", IsMainSubject = false } }
                }
            };

            _teacherRepositoryMock.Setup(repo => repo.GetAllWithUserAsync())
                .ReturnsAsync(teachers);
            _teacherRepositoryMock.Setup(repo => repo.GetTeacherSubjectsAsync(1))
                .ReturnsAsync(teacher1Subjects);
            _teacherRepositoryMock.Setup(repo => repo.GetTeacherSubjectsAsync(2))
               .ReturnsAsync(teacher2Subjects);

            _mapperMock.Setup(m => m.Map<TeacherListDto>(teachers[0]))
                       .Returns(teacherDtos[0]);
            _mapperMock.Setup(m => m.Map<TeacherListDto>(teachers[1]))
                      .Returns(teacherDtos[1]);

            var result = await _teacherService.GetAllTeachersAsync();

            result.Should().NotBeNull();
            result.Teachers.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.Teachers.Should().BeEquivalentTo(teacherDtos, options => options.ComparingByMembers<TeacherListDto>());
            result.Teachers.First(t => t.TeacherId == 1).Subjects.Should().ContainSingle(s => s.SubjectName == "Math" && s.IsMainSubject);
            result.Teachers.First(t => t.TeacherId == 2).Subjects.Should().ContainSingle(s => s.SubjectName == "Physics" && !s.IsMainSubject);
        }

        [Fact]
        public async Task AddTeacherAsync_ShouldAddTeacherAndCommitTransaction_WhenDataIsValid()
        {
            var teacherDto = new TeacherListDto
            {
                FullName = "New Teacher",
                Dob = new DateOnly(1985, 5, 15),
                Gender = "Nữ",
                IdcardNumber = "987654321012",
                InsuranceNumber = "INS987",
                Department = "Physics",
                SchoolJoinDate = new DateOnly(2021, 8, 20),
                Email = "newteacher@example.com",
                PhoneNumber = "0112233445",
                IsHeadOfDepartment = false,
                Subjects = new List<SubjectTeacherDto> { new SubjectTeacherDto { SubjectName = "Physics", IsMainSubject = true } }
            };
            var teacher = new Teacher { TeacherId = 0, };
            var user = new User();
            var role = new Role { RoleId = 2, RoleName = "Giáo viên" };
            var subject = new Subject { SubjectId = 2, SubjectName = "Physics" };

            _teacherRepositoryMock.Setup(repo => repo.ExistsAsync(teacherDto.IdcardNumber))
                .ReturnsAsync(false);
            _teacherRepositoryMock.Setup(repo => repo.IsEmailOrPhoneExistsAsync(teacherDto.Email, teacherDto.PhoneNumber))
                .ReturnsAsync(false);
            _roleRepositoryMock.Setup(repo => repo.GetRoleByNameAsync("Giáo viên"))
                .ReturnsAsync(role);
            _mapperMock.Setup(m => m.Map<Teacher>(teacherDto))
                .Returns(teacher);
            _teacherRepositoryMock.Setup(repo => repo.IsUsernameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _subjectRepositoryMock.Setup(repo => repo.GetByNameAsync("Physics"))
               .ReturnsAsync(subject);

            _teacherRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Teacher>()))
               .Callback<Teacher>(t => {
                   t.TeacherId = 5;
                   t.User.UserId = 105;
                   t.UserId = t.User.UserId;
               })
               .Returns(Task.CompletedTask);

            await _teacherService.AddTeacherAsync(teacherDto);

            _dbContextMock.Verify(c => c.Database.BeginTransaction(), Times.Once());
            _dbTransactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
            _dbTransactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never());

            _teacherRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Teacher>(t =>
                t.FullName == teacherDto.FullName &&
                t.User != null &&
                t.User.Email == teacherDto.Email &&
                t.User.PhoneNumber == teacherDto.PhoneNumber &&
                t.User.RoleId == role.RoleId &&
                t.User.PasswordHash != null
           )), Times.Once());

            _teacherRepositoryMock.Verify(repo => repo.IsUsernameExistsAsync(It.IsAny<string>()), Times.AtLeastOnce());
            _teacherRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.Is<User>(u => u.Username != "tempuser" && u.UserId == 105)), Times.Once());

            _teacherRepositoryMock.Verify(repo => repo.AddTeacherSubjectsRangeAsync(
                It.Is<IEnumerable<TeacherSubject>>(list =>
                    list.Count() == 1 &&
                    list.First().SubjectId == subject.SubjectId &&
                    list.First().IsMainSubject == true &&
                    list.First().TeacherId == 5
                )), Times.Once());
        }

        [Fact]
        public async Task AddTeacherAsync_ShouldRollbackTransaction_WhenTeacherExists()
        {
            var teacherDto = new TeacherListDto { FullName = "Any", Dob = DateOnly.MinValue, Gender = "Any", IdcardNumber = "123456789", InsuranceNumber = "Any", Department = "Any", SchoolJoinDate = DateOnly.MinValue, Email = "t@e.com", PhoneNumber = "111" };
            _teacherRepositoryMock.Setup(repo => repo.ExistsAsync(teacherDto.IdcardNumber)).ReturnsAsync(true);
            _teacherRepositoryMock.Setup(repo => repo.IsEmailOrPhoneExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            Func<Task> act = async () => await _teacherService.AddTeacherAsync(teacherDto);

            await act.Should().ThrowAsync<Exception>().WithMessage($"*Giáo viên với CMND/CCCD {teacherDto.IdcardNumber} đã tồn tại.*");

            _dbContextMock.Verify(c => c.Database.BeginTransaction(), Times.Once());
            _dbTransactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once());
            _dbTransactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());

            _teacherRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Teacher>()), Times.Never());
            _teacherRepositoryMock.Verify(repo => repo.AddTeacherSubjectsRangeAsync(It.IsAny<List<TeacherSubject>>()), Times.Never());

        }


        [Fact]
        public async Task DeleteTeacherAsync_ShouldReturnTrueAndCommit_WhenTeacherExists()
        {
            int teacherId = 1;
            int userId = 101;
            var teacher = new Teacher { TeacherId = teacherId, UserId = userId, User = new User { UserId = userId } };
            var teacherSubjects = new List<TeacherSubject> { new TeacherSubject() };

            _teacherRepositoryMock.Setup(repo => repo.GetByIdWithUserAsync(teacherId))
                .ReturnsAsync(teacher);
            _teacherRepositoryMock.Setup(repo => repo.GetTeacherSubjectsAsync(teacherId))
                .ReturnsAsync(teacherSubjects);

            var result = await _teacherService.DeleteTeacherAsync(teacherId);

            result.Should().BeTrue();

            _dbContextMock.Verify(c => c.Database.BeginTransaction(), Times.Once());
            _dbTransactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
            _dbTransactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never());

            _teacherRepositoryMock.Verify(repo => repo.DeleteTeacherSubjectsRangeAsync(teacherSubjects), Times.Once());
            _teacherRepositoryMock.Verify(repo => repo.DeleteAsync(teacherId), Times.Once());
            _teacherRepositoryMock.Verify(repo => repo.DeleteUserAsync(userId), Times.Once());
        }

        [Fact]
        public async Task DeleteTeacherAsync_ShouldReturnFalseAndRollback_WhenExceptionOccurs()
        {
            int teacherId = 1;
            int userId = 101;
            var teacher = new Teacher { TeacherId = teacherId, UserId = userId, User = new User { UserId = userId } };
            var teacherSubjects = new List<TeacherSubject> { new TeacherSubject() };

            _teacherRepositoryMock.Setup(repo => repo.GetByIdWithUserAsync(teacherId))
               .ReturnsAsync(teacher);
            _teacherRepositoryMock.Setup(repo => repo.GetTeacherSubjectsAsync(teacherId))
               .ReturnsAsync(teacherSubjects);
            _teacherRepositoryMock.Setup(repo => repo.DeleteAsync(teacherId))
               .ThrowsAsync(new InvalidOperationException("Simulated DB error"));

            var result = await _teacherService.DeleteTeacherAsync(teacherId);

            result.Should().BeFalse();

            _dbContextMock.Verify(c => c.Database.BeginTransaction(), Times.Once());
            _dbTransactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once());
            _dbTransactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());

            _teacherRepositoryMock.Verify(repo => repo.DeleteTeacherSubjectsRangeAsync(teacherSubjects), Times.Once());
            _teacherRepositoryMock.Verify(repo => repo.DeleteAsync(teacherId), Times.Once());
            _teacherRepositoryMock.Verify(repo => repo.DeleteUserAsync(userId), Times.Never());
        }

        [Fact]
        public async Task DeleteTeacherAsync_ShouldReturnFalse_WhenTeacherNotFound()
        {
            int teacherId = 99;
            _teacherRepositoryMock.Setup(repo => repo.GetByIdWithUserAsync(teacherId))
                .ReturnsAsync((Teacher?)null);

            var result = await _teacherService.DeleteTeacherAsync(teacherId);

            result.Should().BeFalse();

            _dbContextMock.Verify(c => c.Database.BeginTransaction(), Times.Once());
            _dbTransactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
            _dbTransactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never());

            _teacherRepositoryMock.Verify(repo => repo.DeleteTeacherSubjectsRangeAsync(It.IsAny<List<TeacherSubject>>()), Times.Never());
            _teacherRepositoryMock.Verify(repo => repo.DeleteAsync(teacherId), Times.Never());
            _teacherRepositoryMock.Verify(repo => repo.DeleteUserAsync(It.IsAny<int>()), Times.Never());
        }
    }
}