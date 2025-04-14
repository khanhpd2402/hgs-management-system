using Application.Features.LessonPlans.DTOs;
using Application.Features.LessonPlans.Services;
using AutoMapper;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace HGSM_Server.Tests.Features.LessonPlans.Services
{
    public class LessonPlanServiceTests
    {
        private readonly Mock<ILessonPlanRepository> _lessonPlanRepositoryMock;
        private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LessonPlanService _lessonPlanService;

        public LessonPlanServiceTests()
        {
            _lessonPlanRepositoryMock = new Mock<ILessonPlanRepository>();
            _teacherRepositoryMock = new Mock<ITeacherRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _mapperMock = new Mock<IMapper>();

            _lessonPlanService = new LessonPlanService(
                _lessonPlanRepositoryMock.Object,
                _teacherRepositoryMock.Object,
                _userRepositoryMock.Object,
                _httpContextAccessorMock.Object,
                _mapperMock.Object);
        }

        private void SetupHttpContext(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            var context = new Mock<HttpContext>();
            context.Setup(c => c.User).Returns(principal);
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context.Object);
        }

        

        [Fact]
        public async Task UploadLessonPlanAsync_ShouldThrowException_WhenPlanContentIsEmpty()
        {
            // Arrange
            var lessonPlanDto = new LessonPlanUploadDto
            {
                PlanContent = "",
                SubjectId = 1,
                SemesterId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _lessonPlanService.UploadLessonPlanAsync(lessonPlanDto));
        }

        [Fact]
        public async Task ReviewLessonPlanAsync_ShouldReviewLessonPlan_WhenUserIsHeadOfDepartment()
        {
            // Arrange
            int userId = 1;
            int teacherId = 1;
            int planId = 1;
            var reviewDto = new LessonPlanReviewDto
            {
                PlanId = planId,
                Status = "Approved",
                Feedback = "Good job"
            };
            var lessonPlan = new LessonPlan { PlanId = planId, Status = "Processing" };
            var teacher = new Teacher { TeacherId = teacherId, IsHeadOfDepartment = true };
            var user = new User { UserId = userId };

            SetupHttpContext(userId);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _teacherRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(teacher);
            _teacherRepositoryMock.Setup(repo => repo.GetByIdAsync(teacherId))
                .ReturnsAsync(teacher);
            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdAsync(planId))
                .ReturnsAsync(lessonPlan);

            // Act
            await _lessonPlanService.ReviewLessonPlanAsync(reviewDto);

            // Assert
            lessonPlan.Status.Should().Be("Approved");
            lessonPlan.Feedback.Should().Be("Good job");
            lessonPlan.ReviewerId.Should().Be(teacherId);
            lessonPlan.ReviewedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            _lessonPlanRepositoryMock.Verify(repo => repo.UpdateLessonPlanAsync(lessonPlan), Times.Once());
        }

        [Fact]
        public async Task ReviewLessonPlanAsync_ShouldThrowException_WhenUserIsNotHeadOfDepartment()
        {
            // Arrange
            int userId = 1;
            int teacherId = 1;
            int planId = 1;
            var reviewDto = new LessonPlanReviewDto
            {
                PlanId = planId,
                Status = "Approved"
            };
            var lessonPlan = new LessonPlan { PlanId = planId };
            var teacher = new Teacher { TeacherId = teacherId, IsHeadOfDepartment = false };
            var user = new User { UserId = userId };

            SetupHttpContext(userId);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _teacherRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(teacher);
            _teacherRepositoryMock.Setup(repo => repo.GetByIdAsync(teacherId))
                .ReturnsAsync(teacher);
            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdAsync(planId))
                .ReturnsAsync(lessonPlan);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _lessonPlanService.ReviewLessonPlanAsync(reviewDto));
        }

        [Fact]
        public async Task GetAllLessonPlansAsync_ShouldReturnLessonPlans_WhenPlansExist()
        {
            // Arrange
            var lessonPlans = new List<LessonPlan>
            {
                new LessonPlan { PlanId = 1, Title = "Plan 1" }
            };
            var lessonPlanDtos = new List<LessonPlanResponseDto>
            {
                new LessonPlanResponseDto { PlanId = 1, Title = "Plan 1" }
            };

            _lessonPlanRepositoryMock.Setup(repo => repo.GetAllLessonPlansAsync())
                .ReturnsAsync(lessonPlans);
            _mapperMock.Setup(m => m.Map<List<LessonPlanResponseDto>>(lessonPlans))
                .Returns(lessonPlanDtos);

            // Act
            var result = await _lessonPlanService.GetAllLessonPlansAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().PlanId.Should().Be(1);
            result.First().Title.Should().Be("Plan 1");
        }

        [Fact]
        public async Task GetLessonPlanByIdAsync_ShouldReturnLessonPlan_WhenPlanExists()
        {
            // Arrange
            int planId = 1;
            var lessonPlan = new LessonPlan { PlanId = planId, Title = "Plan 1" };
            var lessonPlanDto = new LessonPlanResponseDto { PlanId = planId, Title = "Plan 1" };

            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdAsync(planId))
                .ReturnsAsync(lessonPlan);
            _mapperMock.Setup(m => m.Map<LessonPlanResponseDto>(lessonPlan))
                .Returns(lessonPlanDto);

            // Act
            var result = await _lessonPlanService.GetLessonPlanByIdAsync(planId);

            // Assert
            result.Should().NotBeNull();
            result.PlanId.Should().Be(planId);
            result.Title.Should().Be("Plan 1");
        }

        [Fact]
        public async Task GetLessonPlanByIdAsync_ShouldThrowException_WhenPlanDoesNotExist()
        {
            // Arrange
            int planId = 1;

            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdAsync(planId))
                .ReturnsAsync((LessonPlan)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _lessonPlanService.GetLessonPlanByIdAsync(planId));
        }

        [Fact]
        public async Task GetLessonPlansByStatusAsync_ShouldReturnLessonPlans_WhenStatusIsValid()
        {
            // Arrange
            string status = "Approved";
            var lessonPlans = new List<LessonPlan>
            {
                new LessonPlan { PlanId = 1, Status = "Approved" }
            };
            var lessonPlanDtos = new List<LessonPlanResponseDto>
            {
                new LessonPlanResponseDto { PlanId = 1, Status = "Approved" }
            };

            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlansByStatusAsync(status))
                .ReturnsAsync(lessonPlans);
            _mapperMock.Setup(m => m.Map<List<LessonPlanResponseDto>>(lessonPlans))
                .Returns(lessonPlanDtos);

            // Act
            var result = await _lessonPlanService.GetLessonPlansByStatusAsync(status);

            // Assert
            result.Should().HaveCount(1);
            result.First().Status.Should().Be("Approved");
        }

        [Fact]
        public async Task GetLessonPlansByStatusAsync_ShouldThrowException_WhenStatusIsEmpty()
        {
            // Arrange
            string status = "";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _lessonPlanService.GetLessonPlansByStatusAsync(status));
        }

        [Fact]
        public async Task GetAllLessonPlansAsync_WithPagination_ShouldReturnPagedLessonPlans()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var lessonPlans = new List<LessonPlan>
            {
                new LessonPlan { PlanId = 1, Title = "Plan 1" }
            };
            var lessonPlanDtos = new List<LessonPlanResponseDto>
            {
                new LessonPlanResponseDto { PlanId = 1, Title = "Plan 1" }
            };
            int totalCount = 1;

            _lessonPlanRepositoryMock.Setup(repo => repo.GetAllLessonPlansAsync(pageNumber, pageSize))
                .ReturnsAsync((lessonPlans, totalCount));
            _mapperMock.Setup(m => m.Map<List<LessonPlanResponseDto>>(lessonPlans))
                .Returns(lessonPlanDtos);

            // Act
            var (result, resultTotalCount) = await _lessonPlanService.GetAllLessonPlansAsync(pageNumber, pageSize);

            // Assert
            result.Should().HaveCount(1);
            resultTotalCount.Should().Be(totalCount);
            result.First().PlanId.Should().Be(1);
        }

        [Fact]
        public async Task GetLessonPlansByStatusAsync_WithPagination_ShouldReturnPagedLessonPlans()
        {
            // Arrange
            string status = "Approved";
            int pageNumber = 1;
            int pageSize = 10;
            var lessonPlans = new List<LessonPlan>
            {
                new LessonPlan { PlanId = 1, Status = "Approved" }
            };
            var lessonPlanDtos = new List<LessonPlanResponseDto>
            {
                new LessonPlanResponseDto { PlanId = 1, Status = "Approved" }
            };
            int totalCount = 1;

            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlansByStatusAsync(status, pageNumber, pageSize))
                .ReturnsAsync((lessonPlans, totalCount));
            _mapperMock.Setup(m => m.Map<List<LessonPlanResponseDto>>(lessonPlans))
                .Returns(lessonPlanDtos);

            // Act
            var (result, resultTotalCount) = await _lessonPlanService.GetLessonPlansByStatusAsync(status, pageNumber, pageSize);

            // Assert
            result.Should().HaveCount(1);
            resultTotalCount.Should().Be(totalCount);
            result.First().Status.Should().Be("Approved");
        }
    }
}