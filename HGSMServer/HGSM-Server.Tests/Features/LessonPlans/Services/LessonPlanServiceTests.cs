
﻿//using Application.Features.LessonPlans.DTOs;
//using Application.Features.LessonPlans.Services;
//using AutoMapper;
//using Domain.Models;
//using FluentAssertions;
//using Infrastructure.Repositories.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Xunit;

//namespace HGSM_Server.Tests.Features.LessonPlans.Services
//{
//    public class LessonPlanServiceTests
//    {
//        private readonly Mock<ILessonPlanRepository> _lessonPlanRepositoryMock;
//        private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
//        private readonly Mock<IUserRepository> _userRepositoryMock;
//        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
//        private readonly Mock<IMapper> _mapperMock;
//        private readonly LessonPlanService _lessonPlanService;

//        public LessonPlanServiceTests()
//        {
//            _lessonPlanRepositoryMock = new Mock<ILessonPlanRepository>();
//            _teacherRepositoryMock = new Mock<ITeacherRepository>();
//            _userRepositoryMock = new Mock<IUserRepository>();
//            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
//            _mapperMock = new Mock<IMapper>();

//            _lessonPlanService = new LessonPlanService(
//                _lessonPlanRepositoryMock.Object,
//                _teacherRepositoryMock.Object,
//                _userRepositoryMock.Object,
//                _httpContextAccessorMock.Object,
//                _mapperMock.Object);
//        }

//        private void SetupHttpContext(int userId)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim("sub", userId.ToString())
//            };
//            var identity = new ClaimsIdentity(claims, "TestAuthType");
//            var principal = new ClaimsPrincipal(identity);
//            var context = new Mock<HttpContext>();
//            context.Setup(c => c.User).Returns(principal);
//            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context.Object);
//        }

//        private void SetupUserAndTeacher(int userId, int teacherId, bool isHeadOfDepartment)
//        {
//            var user = new User { UserId = userId };
//            var teacher = new Teacher { TeacherId = teacherId, IsHeadOfDepartment = isHeadOfDepartment };

//            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
//            _teacherRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(teacher);
//            _teacherRepositoryMock.Setup(repo => repo.GetByIdAsync(teacherId)).ReturnsAsync(teacher);
//        }

//        [Fact]
//        public async Task CreateLessonPlanAsync_ShouldCreatePlan_WhenUserIsHeadOfDepartment()
//        {
//            // Arrange
//            int userId = 1;
//            int teacherId = 1;
//            var createDto = new LessonPlanCreateDto
//            {
//                TeacherId = 2,
//                SubjectId = 1,
//                SemesterId = 1,
//                Title = "Test Plan",
//                PlanContent = "Content",
//                StartDate = DateTime.Now,
//                EndDate = DateTime.Now.AddDays(1)
//            };
//            var lessonPlan = new LessonPlan { PlanId = 1, TeacherId = 2, Status = "Đang chờ" };
//            var lessonPlanResponseDto = new LessonPlanResponseDto { PlanId = 1, TeacherId = 2, Status = "Đang chờ" };

//            SetupHttpContext(userId);
//            SetupUserAndTeacher(userId, teacherId, true);
//            _lessonPlanRepositoryMock.Setup(repo => repo.AddLessonPlanAsync(It.IsAny<LessonPlan>())).Returns(Task.CompletedTask);
//            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdIncludingDetailsAsync(1)).ReturnsAsync(lessonPlan);
//            _mapperMock.Setup(m => m.Map<LessonPlanResponseDto>(lessonPlan)).Returns(lessonPlanResponseDto);

//            // Act
//            var result = await _lessonPlanService.CreateLessonPlanAsync(createDto);

//            // Assert
//            result.Should().NotBeNull();
//            result.PlanId.Should().Be(1);
//            result.Status.Should().Be("Đang chờ");
//            _lessonPlanRepositoryMock.Verify(repo => repo.AddLessonPlanAsync(It.IsAny<LessonPlan>()), Times.Once());
//        }

//        [Fact]
//        public async Task CreateLessonPlanAsync_ShouldThrowException_WhenUserIsNotHeadOfDepartment()
//        {
//            // Arrange
//            int userId = 1;
//            int teacherId = 1;
//            var createDto = new LessonPlanCreateDto
//            {
//                TeacherId = 2,
//                SubjectId = 1,
//                SemesterId = 1
//            };

//            SetupHttpContext(userId);
//            SetupUserAndTeacher(userId, teacherId, false);

//            // Act & Assert
//            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _lessonPlanService.CreateLessonPlanAsync(createDto));
//        }

//        [Fact]
//        public async Task UpdateMyLessonPlanAsync_ShouldUpdatePlan_WhenAuthorizedAndValid()
//        {
//            // Arrange
//            int userId = 1;
//            int teacherId = 1;
//            int planId = 1;
//            var updateDto = new LessonPlanUpdateDto
//            {
//                PlanContent = "Updated Content",
//                Title = "Updated Title",
//                AttachmentUrl = "http://example.com"
//            };
//            var lessonPlan = new LessonPlan
//            {
//                PlanId = planId,
//                TeacherId = teacherId,
//                Status = "Đang chờ",
//                StartDate = DateTime.Now.AddDays(-1),
//                EndDate = DateTime.Now.AddDays(1)
//            };

//            SetupHttpContext(userId);
//            SetupUserAndTeacher(userId, teacherId, false);
//            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdAsync(planId)).ReturnsAsync(lessonPlan);
//            _lessonPlanRepositoryMock.Setup(repo => repo.UpdateLessonPlanAsync(lessonPlan)).Returns(Task.CompletedTask);

//            // Act
//            await _lessonPlanService.UpdateMyLessonPlanAsync(planId, updateDto);

//            // Assert
//            lessonPlan.PlanContent.Should().Be("Updated Content");
//            lessonPlan.Title.Should().Be("Updated Title");
//            lessonPlan.AttachmentUrl.Should().Be("http://example.com");
//            lessonPlan.Status.Should().Be("Đang chờ");
//            lessonPlan.SubmittedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
//            _lessonPlanRepositoryMock.Verify(repo => repo.UpdateLessonPlanAsync(lessonPlan), Times.Once());
//        }

//        [Fact]
//        public async Task UpdateMyLessonPlanAsync_ShouldThrowException_WhenNotAuthorized()
//        {
//            // Arrange
//            int userId = 1;
//            int teacherId = 1;
//            int planId = 1;
//            var updateDto = new LessonPlanUpdateDto();
//            var lessonPlan = new LessonPlan { PlanId = planId, TeacherId = 2 }; // Different teacher

//            SetupHttpContext(userId);
//            SetupUserAndTeacher(userId, teacherId, false);
//            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdAsync(planId)).ReturnsAsync(lessonPlan);

//            // Act & Assert
//            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _lessonPlanService.UpdateMyLessonPlanAsync(planId, updateDto));
//        }

//        [Fact]
//        public async Task ReviewLessonPlanAsync_ShouldReviewPlan_WhenUserIsHeadOfDepartment()
//        {
//            // Arrange
//            int userId = 1;
//            int teacherId = 1;
//            int planId = 1;
//            var reviewDto = new LessonPlanReviewDto
//            {
//                PlanId = planId,
//                Status = "Đã duyệt",
//                Feedback = "Good job"
//            };
//            var lessonPlan = new LessonPlan { PlanId = planId, Status = "Đang chờ" };

//            SetupHttpContext(userId);
//            SetupUserAndTeacher(userId, teacherId, true);
//            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdAsync(planId)).ReturnsAsync(lessonPlan);
//            _lessonPlanRepositoryMock.Setup(repo => repo.UpdateLessonPlanAsync(lessonPlan)).Returns(Task.CompletedTask);

//            // Act
//            await _lessonPlanService.ReviewLessonPlanAsync(reviewDto);

//            // Assert
//            lessonPlan.Status.Should().Be("Đã duyệt");
//            lessonPlan.Feedback.Should().Be("Good job");
//            lessonPlan.ReviewerId.Should().Be(teacherId);
//            lessonPlan.ReviewedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
//            _lessonPlanRepositoryMock.Verify(repo => repo.UpdateLessonPlanAsync(lessonPlan), Times.Once());
//        }

//        [Fact]
//        public async Task ReviewLessonPlanAsync_ShouldThrowException_WhenUserIsNotHeadOfDepartment()
//        {
//            // Arrange
//            int userId = 1;
//            int teacherId = 1;
//            int planId = 1;
//            var reviewDto = new LessonPlanReviewDto
//            {
//                PlanId = planId,
//                Status = "Đã duyệt"
//            };
//            var lessonPlan = new LessonPlan { PlanId = planId };

//            SetupHttpContext(userId);
//            SetupUserAndTeacher(userId, teacherId, false);
//            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdAsync(planId)).ReturnsAsync(lessonPlan);

//            // Act & Assert
//            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _lessonPlanService.ReviewLessonPlanAsync(reviewDto));
//        }

//        [Fact]
//        public async Task GetLessonPlanByIdAsync_ShouldReturnPlan_WhenPlanExists()
//        {
//            // Arrange
//            int planId = 1;
//            var lessonPlan = new LessonPlan { PlanId = planId, Title = "Plan 1" };
//            var lessonPlanDto = new LessonPlanResponseDto { PlanId = planId, Title = "Plan 1" };

//            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdIncludingDetailsAsync(planId)).ReturnsAsync(lessonPlan);
//            _mapperMock.Setup(m => m.Map<LessonPlanResponseDto>(lessonPlan)).Returns(lessonPlanDto);

//            // Act
//            var result = await _lessonPlanService.GetLessonPlanByIdAsync(planId);

//            // Assert
//            result.Should().NotBeNull();
//            result.PlanId.Should().Be(planId);
//            result.Title.Should().Be("Plan 1");
//        }

//        [Fact]
//        public async Task GetLessonPlanByIdAsync_ShouldThrowException_WhenPlanDoesNotExist()
//        {
//            // Arrange
//            int planId = 1;

//            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlanByIdIncludingDetailsAsync(planId)).ReturnsAsync((LessonPlan)null);

//            // Act & Assert
//            await Assert.ThrowsAsync<KeyNotFoundException>(() => _lessonPlanService.GetLessonPlanByIdAsync(planId));
//        }

//        [Fact]
//        public async Task GetAllLessonPlansAsync_ShouldReturnPagedPlans_WhenPlansExist()
//        {
//            // Arrange
//            int pageNumber = 1;
//            int pageSize = 10;
//            var lessonPlans = new List<LessonPlan>
//            {
//                new LessonPlan { PlanId = 1, Title = "Plan 1" }
//            };
//            var lessonPlanDtos = new List<LessonPlanResponseDto>
//            {
//                new LessonPlanResponseDto { PlanId = 1, Title = "Plan 1" }
//            };
//            int totalCount = 1;

//            _lessonPlanRepositoryMock.Setup(repo => repo.GetAllLessonPlansIncludingDetailsAsync(pageNumber, pageSize))
//                .ReturnsAsync((lessonPlans, totalCount));
//            _mapperMock.Setup(m => m.Map<List<LessonPlanResponseDto>>(lessonPlans)).Returns(lessonPlanDtos);

//            // Act
//            var (result, resultTotalCount) = await _lessonPlanService.GetAllLessonPlansAsync(pageNumber, pageSize);

//            // Assert
//            result.Should().HaveCount(1);
//            resultTotalCount.Should().Be(totalCount);
//            result.First().PlanId.Should().Be(1);
//        }

//        [Fact]
//        public async Task GetLessonPlansByTeacherAsync_ShouldReturnPagedPlans_WhenPlansExist()
//        {
//            // Arrange
//            int teacherId = 1;
//            int pageNumber = 1;
//            int pageSize = 10;
//            var lessonPlans = new List<LessonPlan>
//            {
//                new LessonPlan { PlanId = 1, TeacherId = teacherId }
//            };
//            var lessonPlanDtos = new List<LessonPlanResponseDto>
//            {
//                new LessonPlanResponseDto { PlanId = 1, TeacherId = teacherId }
//            };
//            int totalCount = 1;

//            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlansByTeacherIncludingDetailsAsync(teacherId, pageNumber, pageSize))
//                .ReturnsAsync((lessonPlans, totalCount));
//            _mapperMock.Setup(m => m.Map<List<LessonPlanResponseDto>>(lessonPlans)).Returns(lessonPlanDtos);

//            // Act
//            var (result, resultTotalCount) = await _lessonPlanService.GetLessonPlansByTeacherAsync(teacherId, pageNumber, pageSize);

//            // Assert
//            result.Should().HaveCount(1);
//            resultTotalCount.Should().Be(totalCount);
//            result.First().TeacherId.Should().Be(teacherId);
//        }

//        [Fact]
//        public async Task GetLessonPlansByStatusAsync_ShouldReturnPagedPlans_WhenStatusIsValid()
//        {
//            // Arrange
//            string status = "Đã duyệt";
//            int pageNumber = 1;
//            int pageSize = 10;
//            var lessonPlans = new List<LessonPlan>
//            {
//                new LessonPlan { PlanId = 1, Status = "Đã duyệt" }
//            };
//            var lessonPlanDtos = new List<LessonPlanResponseDto>
//            {
//                new LessonPlanResponseDto { PlanId = 1, Status = "Đã duyệt" }
//            };
//            int totalCount = 1;

//            _lessonPlanRepositoryMock.Setup(repo => repo.GetLessonPlansByStatusIncludingDetailsAsync(status, pageNumber, pageSize))
//                .ReturnsAsync((lessonPlans, totalCount));
//            _mapperMock.Setup(m => m.Map<List<LessonPlanResponseDto>>(lessonPlans)).Returns(lessonPlanDtos);

//            // Act
//            var (result, resultTotalCount) = await _lessonPlanService.GetLessonPlansByStatusAsync(status, pageNumber, pageSize);

//            // Assert
//            result.Should().HaveCount(1);
//            resultTotalCount.Should().Be(totalCount);
//            result.First().Status.Should().Be("Đã duyệt");
//        }

//        [Fact]
//        public async Task GetLessonPlansByStatusAsync_ShouldThrowException_WhenStatusIsEmpty()
//        {
//            // Arrange
//            string status = "";

//            // Act & Assert
//            await Assert.ThrowsAsync<ArgumentException>(() => _lessonPlanService.GetLessonPlansByStatusAsync(status, 1, 10));
//        }
//    }
//}
