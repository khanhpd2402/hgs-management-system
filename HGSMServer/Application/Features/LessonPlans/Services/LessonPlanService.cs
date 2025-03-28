using Application.Features.LessonPlans.DTOs;
using Application.Features.LessonPlans.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper; // Thêm AutoMapper để ánh xạ

namespace Application.Features.LessonPlans.Services
{
    public class LessonPlanService : ILessonPlanService
    {
        private readonly ILessonPlanRepository _lessonPlanRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper; // Thêm IMapper

        public LessonPlanService(
            ILessonPlanRepository lessonPlanRepository,
            ITeacherRepository teacherRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _lessonPlanRepository = lessonPlanRepository ?? throw new ArgumentNullException(nameof(lessonPlanRepository));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task UploadLessonPlanAsync(LessonPlanUploadDto lessonPlanDto)
        {
            if (lessonPlanDto == null || string.IsNullOrEmpty(lessonPlanDto.PlanContent))
                throw new ArgumentException("Plan content is required.");

            var claims = _httpContextAccessor.HttpContext?.User?.Claims?.ToList() ?? new List<Claim>();
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "sub")
                ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token.");

            var userId = int.Parse(userIdClaim.Value);
            var teacherId = await GetTeacherIdFromUserId(userId);

            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new ArgumentException("Teacher not found.");

            // Không cần kiểm tra lessonPlanDto.TeacherId nữa vì ta đã tự động lấy teacherId
            var lessonPlan = new LessonPlan
            {
                TeacherId = teacherId, // Gán teacherId tự động từ userId
                SubjectId = lessonPlanDto.SubjectId,
                PlanContent = lessonPlanDto.PlanContent,
                Status = "Processing",
                SemesterId = lessonPlanDto.SemesterId,
                Title = lessonPlanDto.Title,
                AttachmentUrl = lessonPlanDto.AttachmentUrl,
                SubmittedDate = DateTime.Now,
                Feedback = null,
                ReviewedDate = null,
                ReviewerId = null
            };

            await _lessonPlanRepository.AddLessonPlanAsync(lessonPlan);
        }


        public async Task ReviewLessonPlanAsync(LessonPlanReviewDto reviewDto)
        {
            if (reviewDto == null || reviewDto.PlanId <= 0 || string.IsNullOrEmpty(reviewDto.Status))
                throw new ArgumentException("PlanId and status are required.");

            if (reviewDto.Status != "Approved" && reviewDto.Status != "Rejected")
                throw new ArgumentException("Status must be 'Approved' or 'Rejected'.");

            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(reviewDto.PlanId);
            if (lessonPlan == null)
                throw new ArgumentException("Lesson plan not found.");

            var claims = _httpContextAccessor.HttpContext?.User?.Claims?.ToList() ?? new List<Claim>();
            Console.WriteLine($"Claims: {string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}"))}");

            var userIdClaim = claims.FirstOrDefault(c => c.Type == "sub")
                ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token.");

            var userId = int.Parse(userIdClaim.Value);

            var teacherId = await GetTeacherIdFromUserId(userId);
            Console.WriteLine($"Mapped TeacherId: {teacherId}");

            var reviewer = await _teacherRepository.GetByIdAsync(teacherId);
            if (reviewer == null)
            {
                Console.WriteLine($"Teacher with ID {teacherId} not found in database.");
                throw new UnauthorizedAccessException($"Teacher with ID {teacherId} not found.");
            }

            bool isHeadOfDepartment = reviewer.IsHeadOfDepartment ?? false;
            Console.WriteLine($"IsHeadOfDepartment for TeacherId {teacherId}: {isHeadOfDepartment}");
            if (!isHeadOfDepartment)
            {
                throw new UnauthorizedAccessException("Only head of department can review lesson plans.");
            }

            lessonPlan.Status = reviewDto.Status;
            lessonPlan.Feedback = reviewDto.Feedback;
            lessonPlan.ReviewedDate = DateTime.Now;
            lessonPlan.ReviewerId = teacherId;

            await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan);
        }

        public async Task<List<LessonPlanResponseDto>> GetAllLessonPlansAsync()
        {
            var lessonPlans = await _lessonPlanRepository.GetAllLessonPlansAsync();
            return _mapper.Map<List<LessonPlanResponseDto>>(lessonPlans);
        }

        public async Task<LessonPlanResponseDto> GetLessonPlanByIdAsync(int planId)
        {
            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(planId);
            if (lessonPlan == null)
                throw new ArgumentException("Lesson plan not found.");
            return _mapper.Map<LessonPlanResponseDto>(lessonPlan);
        }

        public async Task<List<LessonPlanResponseDto>> GetLessonPlansByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Status is required.");

            var lessonPlans = await _lessonPlanRepository.GetLessonPlansByStatusAsync(status);
            return _mapper.Map<List<LessonPlanResponseDto>>(lessonPlans);
        }

        private async Task<int> GetTeacherIdFromUserId(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException($"User with ID {userId} not found.");

            var teacher = await _teacherRepository.GetByUserIdAsync(userId);
            if (teacher == null)
                throw new UnauthorizedAccessException($"No teacher found for UserID {userId}.");
            return teacher.TeacherId;
        }
        public async Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetAllLessonPlansAsync(int pageNumber, int pageSize)
        {
            var (lessonPlans, totalCount) = await _lessonPlanRepository.GetAllLessonPlansAsync(pageNumber, pageSize);
            var lessonPlanDtos = _mapper.Map<List<LessonPlanResponseDto>>(lessonPlans);
            return (lessonPlanDtos, totalCount);
        }

        public async Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetLessonPlansByStatusAsync(string status, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Status is required.");

            var (lessonPlans, totalCount) = await _lessonPlanRepository.GetLessonPlansByStatusAsync(status, pageNumber, pageSize);
            var lessonPlanDtos = _mapper.Map<List<LessonPlanResponseDto>>(lessonPlans);
            return (lessonPlanDtos, totalCount);
        }
    }
}