using Application.Features.LessonPlans.DTOs;
using Application.Features.LessonPlans.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Features.LessonPlans.Services
{
    public class LessonPlanService : ILessonPlanService
    {
        private readonly ILessonPlanRepository _lessonPlanRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserRepository _userRepository; // Để map UserID sang TeacherID
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LessonPlanService(ILessonPlanRepository lessonPlanRepository, ITeacherRepository teacherRepository,
            IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _lessonPlanRepository = lessonPlanRepository ?? throw new ArgumentNullException(nameof(lessonPlanRepository));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task UploadLessonPlanAsync(LessonPlanUploadDto lessonPlanDto)
        {
            if (lessonPlanDto == null || string.IsNullOrEmpty(lessonPlanDto.PlanContent))
                throw new ArgumentException("Plan content is required.");

            var userId = int.Parse(_httpContextAccessor.HttpContext?.User?.Claims
                ?.FirstOrDefault(c => c.Type == "sub")?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in token."));

            var teacherId = await GetTeacherIdFromUserId(userId);
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new ArgumentException("Teacher not found.");
            if (teacher.TeacherId != lessonPlanDto.TeacherId)
                throw new UnauthorizedAccessException("You can only upload lesson plans for yourself.");

            var lessonPlan = new LessonPlan
            {
                TeacherId = lessonPlanDto.TeacherId,
                SubjectId = lessonPlanDto.SubjectId,
                PlanContent = lessonPlanDto.PlanContent,
                Status = "Processing",
                SemesterId = lessonPlanDto.SemesterId,
                Title = lessonPlanDto.Title, // Gán Title
                AttachmentUrl = lessonPlanDto.AttachmentUrl, // Gán AttachmentUrl
                SubmittedDate = DateTime.Now,
                Feedback = null, // Ban đầu chưa có phản hồi
                ReviewedDate = null, // Ban đầu chưa được duyệt
                ReviewerId = null // Ban đầu chưa có người duyệt
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

            // Tìm "sub" hoặc ClaimTypes.NameIdentifier
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
    }
}