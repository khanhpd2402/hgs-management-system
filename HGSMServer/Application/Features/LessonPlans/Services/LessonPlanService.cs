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
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.LessonPlans.Services
{
    public class LessonPlanService : ILessonPlanService
    {
        private readonly ILessonPlanRepository _lessonPlanRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

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

        public async Task<LessonPlanResponseDto> CreateLessonPlanAsync(LessonPlanCreateDto createDto)
        {
            var creatorTeacherId = GetCurrentTeacherId();
            var creatorTeacher = await _teacherRepository.GetByIdAsync(creatorTeacherId);

            bool isAuthorized = (creatorTeacher?.IsHeadOfDepartment ?? false);
            

            if (!isAuthorized)
            {
                throw new UnauthorizedAccessException("You do not have permission to create lesson plans.");
            }

            if (createDto == null) throw new ArgumentNullException(nameof(createDto));
            if (createDto.TeacherId <= 0) throw new ArgumentException("Target TeacherId is required.", nameof(createDto.TeacherId));
            if (createDto.SubjectId <= 0) throw new ArgumentException("SubjectId is required.", nameof(createDto.SubjectId));
            if (createDto.SemesterId <= 0) throw new ArgumentException("SemesterId is required.", nameof(createDto.SemesterId));
            if (createDto.StartDate.HasValue && createDto.EndDate.HasValue && createDto.EndDate < createDto.StartDate)
            {
                throw new ArgumentException("DeadlineDate must be on or after StartDate.");
            }


            var lessonPlan = new LessonPlan
            {
                TeacherId = createDto.TeacherId,
                SubjectId = createDto.SubjectId,
                SemesterId = createDto.SemesterId,
                PlanContent = createDto.PlanContent ?? string.Empty,
                Status = "Đang chờ",
                Title = createDto.Title,
                Startdate = createDto.StartDate,
                EndDate = createDto.EndDate,
                AttachmentUrl = null,
                Feedback = null,
                SubmittedDate = null,
                ReviewedDate = null,
                ReviewerId = null
            };

            await _lessonPlanRepository.AddLessonPlanAsync(lessonPlan);

            var createdPlan = await _lessonPlanRepository.GetLessonPlanByIdIncludingDetailsAsync(lessonPlan.PlanId);
            return _mapper.Map<LessonPlanResponseDto>(createdPlan);
        }

        public async Task UpdateMyLessonPlanAsync(int planId, LessonPlanUpdateDto updateDto)
        {
            var teacherId = GetCurrentTeacherId();
            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(planId);

            if (lessonPlan == null)
                throw new KeyNotFoundException("Lesson plan not found.");

            if (lessonPlan.TeacherId != teacherId)
                throw new UnauthorizedAccessException("You are not authorized to update this lesson plan.");

            if (lessonPlan.Status == "Đã duyệt" || lessonPlan.Status == "Đã nộp")
            {
                throw new InvalidOperationException($"Cannot update lesson plan with status '{lessonPlan.Status}'.");
            }

            DateTime currentDate = DateTime.SpecifyKind(DateTime.Now.Date, DateTimeKind.Unspecified);
            if (lessonPlan.Startdate.HasValue && currentDate < lessonPlan.Startdate.Value.Date)
            {
                throw new InvalidOperationException($"You can only start updating the lesson plan from {lessonPlan.Startdate.Value:dd/MM/yyyy}.");
            }

            if (lessonPlan.EndDate.HasValue && currentDate > lessonPlan.EndDate.Value.Date)
            {
                Console.WriteLine($"Warning: Updating lesson plan (ID: {planId}) after the deadline.");
            }

            lessonPlan.PlanContent = updateDto.PlanContent;
            lessonPlan.Title = updateDto.Title;
            lessonPlan.AttachmentUrl = updateDto.AttachmentUrl;
            lessonPlan.SubmittedDate = DateTime.Now;
            lessonPlan.Status = "Đang chờ";

            await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan);
        }

        public async Task ReviewLessonPlanAsync(LessonPlanReviewDto reviewDto)
        {
            if (reviewDto == null || reviewDto.PlanId <= 0 || string.IsNullOrEmpty(reviewDto.Status))
                throw new ArgumentException("PlanId and status are required.");

            if (reviewDto.Status != "Đã duyệt" && reviewDto.Status != "Từ chối")
                throw new ArgumentException("Status must be 'Đã duyệt' or 'Từ chối'.");

            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(reviewDto.PlanId);
            if (lessonPlan == null)
                throw new KeyNotFoundException("Lesson plan not found.");

            var reviewerId = GetCurrentTeacherId();
            var reviewer = await _teacherRepository.GetByIdAsync(reviewerId);
            if (reviewer == null || !(reviewer.IsHeadOfDepartment ?? false))
            {
                throw new UnauthorizedAccessException("Only Head of Department can review lesson plans.");
            }

            lessonPlan.Status = reviewDto.Status;
            lessonPlan.Feedback = reviewDto.Feedback;
            lessonPlan.ReviewedDate = DateTime.Now;
            lessonPlan.ReviewerId = reviewerId;

            await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan);
        }

        public async Task<LessonPlanResponseDto> GetLessonPlanByIdAsync(int planId)
        {
            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdIncludingDetailsAsync(planId);
            if (lessonPlan == null)
                throw new KeyNotFoundException("Lesson plan not found.");
            return _mapper.Map<LessonPlanResponseDto>(lessonPlan);
        }

        public async Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetAllLessonPlansAsync(int pageNumber, int pageSize)
        {
            var (lessonPlans, totalCount) = await _lessonPlanRepository.GetAllLessonPlansIncludingDetailsAsync(pageNumber, pageSize);
            var lessonPlanDtos = _mapper.Map<List<LessonPlanResponseDto>>(lessonPlans);
            return (lessonPlanDtos, totalCount);
        }

        public async Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetLessonPlansByTeacherAsync(int teacherId, int pageNumber, int pageSize)
        {
            var (lessonPlans, totalCount) = await _lessonPlanRepository.GetLessonPlansByTeacherIncludingDetailsAsync(teacherId, pageNumber, pageSize);
            var lessonPlanDtos = _mapper.Map<List<LessonPlanResponseDto>>(lessonPlans);
            return (lessonPlanDtos, totalCount);
        }


        public async Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetLessonPlansByStatusAsync(string status, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Status is required.");

            var (lessonPlans, totalCount) = await _lessonPlanRepository.GetLessonPlansByStatusIncludingDetailsAsync(status, pageNumber, pageSize);
            var lessonPlanDtos = _mapper.Map<List<LessonPlanResponseDto>>(lessonPlans);
            return (lessonPlanDtos, totalCount);
        }

        private int GetCurrentTeacherId()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.Claims?.ToList() ?? new List<Claim>();
            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
               ?? claims.FirstOrDefault(c => c.Type == "sub")
               ?? throw new UnauthorizedAccessException("User ID not found in token.");

            if (!int.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid User ID format in token.");

            var teacher = Task.Run(() => _teacherRepository.GetByUserIdAsync(userId)).GetAwaiter().GetResult();
            if (teacher == null)
                throw new UnauthorizedAccessException($"No teacher profile found for UserID {userId}.");
            return teacher.TeacherId;
        }

    }
}