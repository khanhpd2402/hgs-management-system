using Application.Features.Parents.DTOs;
using Application.Features.Students.DTOs;
using Application.Features.Students.Services;
using AutoMapper;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace HGSM_Server.Tests.Features.Students.Services
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentRepository> _studentRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IParentRepository> _parentRepositoryMock;
        private readonly Mock<IClassRepository> _classRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<HgsdbContext> _contextMock;
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            _studentRepositoryMock = new Mock<IStudentRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _parentRepositoryMock = new Mock<IParentRepository>();
            _classRepositoryMock = new Mock<IClassRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapperMock = new Mock<IMapper>();
            _contextMock = new Mock<HgsdbContext>();

            _studentService = new StudentService(
                _studentRepositoryMock.Object,
                _userRepositoryMock.Object,
                _parentRepositoryMock.Object,
                _classRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _mapperMock.Object,
                _contextMock.Object);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnStudentDto_WhenStudentExists()
        {
            // Arrange
            int studentId = 1;
            int academicYearId = 1;
            var student = new Student
            {
                StudentId = studentId,
                FullName = "Nguyen Van A",
                ParentId = 2
            };
            var parent = new Parent
            {
                ParentId = 2,
                FullNameFather = "Nguyen Van Father"
            };
            var studentDto = new StudentDto
            {
                StudentId = studentId,
                FullName = "Nguyen Van A",
                Parent = new ParentDto { ParentId = 2, FullNameFather = "Nguyen Van Father" }
            };

            _studentRepositoryMock.Setup(repo => repo.GetByIdWithParentsAsync(studentId, academicYearId))
                .ReturnsAsync(student);
            _parentRepositoryMock.Setup(repo => repo.GetByIdAsync(2))
                .ReturnsAsync(parent);
            _mapperMock.Setup(m => m.Map<StudentDto>(student))
                .Returns(studentDto);
            _mapperMock.Setup(m => m.Map<ParentDto>(parent))
                .Returns(studentDto.Parent);

            // Act
            var result = await _studentService.GetStudentByIdAsync(studentId, academicYearId);

            // Assert
            result.Should().NotBeNull();
            result.StudentId.Should().Be(studentId);
            result.FullName.Should().Be("Nguyen Van A");
            result.Parent.Should().NotBeNull();
            result.Parent.ParentId.Should().Be(2);
            result.Parent.FullNameFather.Should().Be("Nguyen Van Father");
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnNull_WhenStudentDoesNotExist()
        {
            // Arrange
            int studentId = 1;
            int academicYearId = 1;

            _studentRepositoryMock.Setup(repo => repo.GetByIdWithParentsAsync(studentId, academicYearId))
                .ReturnsAsync((Student)null);

            // Act
            var result = await _studentService.GetStudentByIdAsync(studentId, academicYearId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnStudentDtoWithoutParent_WhenParentDoesNotExist()
        {
            // Arrange
            int studentId = 1;
            int academicYearId = 1;
            var student = new Student
            {
                StudentId = studentId,
                FullName = "Nguyen Van A",
                ParentId = 2
            };
            var studentDto = new StudentDto
            {
                StudentId = studentId,
                FullName = "Nguyen Van A",
                Parent = null
            };

            _studentRepositoryMock.Setup(repo => repo.GetByIdWithParentsAsync(studentId, academicYearId))
                .ReturnsAsync(student);
            _parentRepositoryMock.Setup(repo => repo.GetByIdAsync(2))
                .ReturnsAsync((Parent)null);
            _mapperMock.Setup(m => m.Map<StudentDto>(student))
                .Returns(studentDto);

            // Act
            var result = await _studentService.GetStudentByIdAsync(studentId, academicYearId);

            // Assert
            result.Should().NotBeNull();
            result.StudentId.Should().Be(studentId);
            result.Parent.Should().BeNull();
        }
        [Fact]
        public async Task AddStudentAsync_ShouldAddStudent_WhenDataIsValid()
        {
            // Arrange
            var createStudentDto = new CreateStudentDto
            {
                FullName = "Nguyen Van A",
                Dob = DateOnly.FromDateTime(new DateTime(2010, 1, 1)),
                Gender = "Nam",
                AdmissionDate = DateOnly.FromDateTime(new DateTime(2023, 9, 1)),
                ClassId = 1,
                IdcardNumber = "123456789012"
            };
            var student = new Student { StudentId = 1, FullName = "Nguyen Van A" };

            _studentRepositoryMock.Setup(repo => repo.ExistsAsync(createStudentDto.IdcardNumber))
                .ReturnsAsync(false);
            _classRepositoryMock.Setup(repo => repo.ExistsAsync(createStudentDto.ClassId))
                .ReturnsAsync(true);
            _studentRepositoryMock.Setup(repo => repo.GetCurrentAcademicYearAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new AcademicYear { AcademicYearId = 1 });
            _mapperMock.Setup(m => m.Map<Student>(createStudentDto))
                .Returns(student);
            _studentRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Student>()))
                .Callback<Student>(s => s.StudentId = 1);
            _studentRepositoryMock.Setup(repo => repo.BeginTransactionAsync())
                .ReturnsAsync(new Mock<IDbContextTransaction>().Object);

            // Act
            var result = await _studentService.AddStudentAsync(createStudentDto);

            // Assert
            result.Should().Be(1); // StudentId
        }

        [Fact]
        public async Task AddStudentAsync_ShouldThrowException_WhenIdCardNumberExists()
        {
            // Arrange
            var createStudentDto = new CreateStudentDto
            {
                FullName = "Nguyen Van A",
                Dob = DateOnly.FromDateTime(new DateTime(2010, 1, 1)),
                Gender = "Nam",
                AdmissionDate = DateOnly.FromDateTime(new DateTime(2023, 9, 1)),
                ClassId = 1,
                IdcardNumber = "123456789012"
            };

            _studentRepositoryMock.Setup(repo => repo.ExistsAsync(createStudentDto.IdcardNumber))
                .ReturnsAsync(true);
            _studentRepositoryMock.Setup(repo => repo.BeginTransactionAsync())
                .ReturnsAsync(new Mock<IDbContextTransaction>().Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _studentService.AddStudentAsync(createStudentDto));
        }
        [Fact]
        public async Task DeleteStudentAsync_ShouldDeleteStudent_WhenStudentExists()
        {
            // Arrange
            int studentId = 1;
            _studentRepositoryMock.Setup(repo => repo.DeleteAsync(studentId))
                .Returns(Task.CompletedTask);

            // Act
            await _studentService.DeleteStudentAsync(studentId);

            // Assert
            _studentRepositoryMock.Verify(repo => repo.DeleteAsync(studentId), Times.Once());
        }
        [Fact]
        public async Task GetAllStudentsWithParentsAsync_ShouldReturnStudentList_WhenStudentsExist()
        {
            // Arrange
            int academicYearId = 1;
            var students = new List<Student>
    {
        new Student { StudentId = 1, FullName = "Nguyen Van A", ParentId = 1 },
        new Student { StudentId = 2, FullName = "Tran Thi B", ParentId = null }
    };
            var parent = new Parent { ParentId = 1, FullNameFather = "Nguyen Van Father" };
            var studentDtos = new List<StudentDto>
    {
        new StudentDto { StudentId = 1, FullName = "Nguyen Van A", Parent = new ParentDto { ParentId = 1, FullNameFather = "Nguyen Van Father" } },
        new StudentDto { StudentId = 2, FullName = "Tran Thi B", Parent = null }
    };

            _studentRepositoryMock.Setup(repo => repo.GetAllWithParentsAsync(academicYearId))
                .ReturnsAsync(students);
            _parentRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(parent);
            _mapperMock.Setup(m => m.Map<StudentDto>(It.IsAny<Student>()))
                .Returns((Student s) => studentDtos.First(dto => dto.StudentId == s.StudentId));
            _mapperMock.Setup(m => m.Map<ParentDto>(parent))
                .Returns(studentDtos[0].Parent);

            // Act
            var result = await _studentService.GetAllStudentsWithParentsAsync(academicYearId);

            // Assert
            result.Should().NotBeNull();
            result.Students.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.Students[0].Parent.Should().NotBeNull();
            result.Students[1].Parent.Should().BeNull();
        }

        [Fact]
        public async Task GetAllStudentsWithParentsAsync_ShouldReturnEmptyList_WhenNoStudentsExist()
        {
            // Arrange
            int academicYearId = 1;

            _studentRepositoryMock.Setup(repo => repo.GetAllWithParentsAsync(academicYearId))
                .ReturnsAsync(new List<Student>());

            // Act
            var result = await _studentService.GetAllStudentsWithParentsAsync(academicYearId);

            // Assert
            result.Should().NotBeNull();
            result.Students.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }
        [Fact]
        public async Task UpdateStudentAsync_ShouldUpdateStudent_WhenDataIsValid()
        {
            // Arrange
            int studentId = 1;
            var updateStudentDto = new UpdateStudentDto
            {
                FullName = "Nguyen Van B",
                Dob = DateOnly.FromDateTime(new DateTime(2010, 1, 1)),
                Gender = "Nam",
                ClassId = 2,
                AdmissionDate = DateOnly.FromDateTime(new DateTime(2023, 9, 1)),
                IdcardNumber = "123456789013"
            };
            var student = new Student
            {
                StudentId = studentId,
                FullName = "Nguyen Van A",
                Dob = DateOnly.FromDateTime(new DateTime(2010, 1, 1)),
                Gender = "Nam",
                AdmissionDate = DateOnly.FromDateTime(new DateTime(2023, 9, 1)),
                IdcardNumber = "123456789012",
                StudentClasses = new List<StudentClass> { new StudentClass { ClassId = 1 } }
            };

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _classRepositoryMock.Setup(repo => repo.ExistsAsync(2))
                .ReturnsAsync(true);
            _studentRepositoryMock.Setup(repo => repo.ExistsAsync(updateStudentDto.IdcardNumber))
                .ReturnsAsync(false);
            _studentRepositoryMock.Setup(repo => repo.BeginTransactionAsync())
                .ReturnsAsync(new Mock<IDbContextTransaction>().Object);

            // Act
            await _studentService.UpdateStudentAsync(studentId, updateStudentDto);

            // Assert
            student.FullName.Should().Be("Nguyen Van B");
            student.StudentClasses.First().ClassId.Should().Be(2);
            student.IdcardNumber.Should().Be("123456789013");
            _studentRepositoryMock.Verify(repo => repo.UpdateAsync(student), Times.Once());
        }

        [Fact]
        public async Task UpdateStudentAsync_ShouldThrowException_WhenStudentNotFound()
        {
            // Arrange
            int studentId = 1;
            var updateStudentDto = new UpdateStudentDto { FullName = "Nguyen Van B" };

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(studentId))
                .ReturnsAsync((Student)null);
            _studentRepositoryMock.Setup(repo => repo.BeginTransactionAsync())
                .ReturnsAsync(new Mock<IDbContextTransaction>().Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _studentService.UpdateStudentAsync(studentId, updateStudentDto));
        }
    }
}