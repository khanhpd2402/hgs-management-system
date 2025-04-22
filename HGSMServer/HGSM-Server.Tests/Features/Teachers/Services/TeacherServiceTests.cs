using Application.Features.Subjects.DTOs;
using Application.Features.Teachers.DTOs;
using AutoMapper;
using Common.Utils;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HGSM_Server.Tests.Features.Teachers.Services
{
    public class TeacherServiceTests
    {
        private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
        private readonly Mock<ISubjectRepository> _subjectRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TeacherService _teacherService;

        //public TeacherServiceTests()
        //{
        //    _teacherRepositoryMock = new Mock<ITeacherRepository>();
        //    _subjectRepositoryMock = new Mock<ISubjectRepository>();
        //    _roleRepositoryMock = new Mock<IRoleRepository>();
        //    _mapperMock = new Mock<IMapper>();

        //    _teacherService = new TeacherService(
        //        _teacherRepositoryMock.Object,
        //        _mapperMock.Object,
        //        _subjectRepositoryMock.Object,
        //        _roleRepositoryMock.Object);
        //}

        [Fact]
        public async Task GetAllTeachersAsync_ShouldReturnTeacherList_WhenTeachersExist()
        {
            // Arrange
            var teachers = new List<Teacher>
            {
                new Teacher
                {
                    TeacherId = 1,
                    FullName = "Teacher 1",
                    User = new User { Email = "teacher1@example.com", PhoneNumber = "0123456789" }
                }
            };
            var teacherSubjects = new List<TeacherSubject>
            {
                new TeacherSubject { Subject = new Subject { SubjectName = "Math" }, IsMainSubject = true }
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
                }
            };

            _teacherRepositoryMock.Setup(repo => repo.GetAllWithUserAsync())
                .ReturnsAsync(teachers);
            _teacherRepositoryMock.Setup(repo => repo.GetTeacherSubjectsAsync(1))
                .ReturnsAsync(teacherSubjects);
            _mapperMock.Setup(m => m.Map<TeacherListDto>(teachers[0]))
                .Returns(teacherDtos[0]);

            // Act
            var result = await _teacherService.GetAllTeachersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Teachers.Should().HaveCount(1);
            result.TotalCount.Should().Be(1);
            result.Teachers.First().Subjects.Should().ContainSingle(s => s.SubjectName == "Math" && s.IsMainSubject); // Fixed: Use First() instead of [0]
        }

        [Fact]
        public async Task AddTeacherAsync_ShouldAddTeacher_WhenDataIsValid()
        {
            // Arrange
            var teacherDto = new TeacherListDto
            {
                FullName = "Teacher 1",
                Dob = DateOnly.FromDateTime(new DateTime(1980, 1, 1)),
                Gender = "Nam",
                IdcardNumber = "123456789012",
                InsuranceNumber = "INS123",
                Department = "Math",
                SchoolJoinDate = DateOnly.FromDateTime(new DateTime(2020, 1, 1)),
                Email = "teacher1@example.com",
                PhoneNumber = "0123456789",
                Subjects = new List<SubjectTeacherDto> { new SubjectTeacherDto { SubjectName = "Math", IsMainSubject = true } }
            };
            var teacher = new Teacher { TeacherId = 1 };
            var role = new Role { RoleId = 2, RoleName = "Giáo viên" };
            var subject = new Subject { SubjectId = 1, SubjectName = "Math" };

            _teacherRepositoryMock.Setup(repo => repo.ExistsAsync(teacherDto.IdcardNumber))
                .ReturnsAsync(false);
            _teacherRepositoryMock.Setup(repo => repo.IsEmailOrPhoneExistsAsync(teacherDto.Email, teacherDto.PhoneNumber))
                .ReturnsAsync(false);
            _roleRepositoryMock.Setup(repo => repo.GetRoleByNameAsync("Giáo viên"))
                .ReturnsAsync(role);
            _mapperMock.Setup(m => m.Map<Teacher>(teacherDto))
                .Returns(teacher);
            _teacherRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Teacher>()))
                .Callback<Teacher>(t => t.TeacherId = 1);
            _teacherRepositoryMock.Setup(repo => repo.IsUsernameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _subjectRepositoryMock.Setup(repo => repo.GetByNameAsync("Math"))
                .ReturnsAsync(subject);

            // Act
            await _teacherService.AddTeacherAsync(teacherDto);

            // Assert
            _teacherRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Teacher>()), Times.Once());
            _teacherRepositoryMock.Verify(repo => repo.AddTeacherSubjectsRangeAsync(It.IsAny<List<TeacherSubject>>()), Times.Once());
        }

        [Fact]
        public async Task DeleteTeacherAsync_ShouldReturnTrue_WhenTeacherExists()
        {
            // Arrange
            int teacherId = 1;
            var teacher = new Teacher { TeacherId = 1, UserId = 1 };
            var teacherSubjects = new List<TeacherSubject> { new TeacherSubject() };

            _teacherRepositoryMock.Setup(repo => repo.GetByIdWithUserAsync(teacherId))
                .ReturnsAsync(teacher);
            _teacherRepositoryMock.Setup(repo => repo.GetTeacherSubjectsAsync(teacherId))
                .ReturnsAsync(teacherSubjects);

            // Act
            var result = await _teacherService.DeleteTeacherAsync(teacherId);

            // Assert
            result.Should().BeTrue();
            _teacherRepositoryMock.Verify(repo => repo.DeleteAsync(teacherId), Times.Once());
            _teacherRepositoryMock.Verify(repo => repo.DeleteUserAsync(1), Times.Once());
        }


    }
}