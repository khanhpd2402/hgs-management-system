using Application.Features.LessonPlans.DTOs;
using Application.Features.LessonPlans.Interfaces;
using Application.Features.Teachers.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Common.Utils;

namespace Application.Features.LessonPlans.Services
{
    public class LessonPlanService : ILessonPlanService
    {
        private readonly ILessonPlanRepository _lessonPlanRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITeacherService _teacherService;

        public LessonPlanService(
            ILessonPlanRepository lessonPlanRepository,
            ITeacherRepository teacherRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            EmailService emailService,
            ISubjectRepository subjectRepository,
            ITeacherService teacherService)
        {
            _lessonPlanRepository = lessonPlanRepository ?? throw new ArgumentNullException(nameof(lessonPlanRepository));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _subjectRepository = subjectRepository ?? throw new ArgumentNullException(nameof(subjectRepository));
            _teacherService = teacherService ?? throw new ArgumentNullException(nameof(teacherService));
        }

        public async Task<LessonPlanResponseDto> CreateLessonPlanAsync(LessonPlanCreateDto createDto)
        {
            if (createDto == null)
            {
                throw new ArgumentNullException(nameof(createDto), "Thông tin giáo án không được để trống.");
            }

            if (createDto.TeacherId <= 0)
            {
                throw new ArgumentException("TeacherId phải là một số nguyên dương.", nameof(createDto.TeacherId));
            }

            if (createDto.SubjectId <= 0)
            {
                throw new ArgumentException("SubjectId phải là một số nguyên dương.", nameof(createDto.SubjectId));
            }

            if (createDto.SemesterId <= 0)
            {
                throw new ArgumentException("SemesterId phải là một số nguyên dương.", nameof(createDto.SemesterId));
            }

            if (createDto.StartDate.HasValue && createDto.EndDate.HasValue && createDto.EndDate < createDto.StartDate)
            {
                throw new ArgumentException("Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.");
            }

            var creatorTeacherId = GetCurrentTeacherId();
            var lessonPlan = new LessonPlan
            {
                TeacherId = createDto.TeacherId,
                SubjectId = createDto.SubjectId,
                SemesterId = createDto.SemesterId,
                PlanContent = createDto.PlanContent ?? string.Empty,
                Status = "Chờ duyệt",
                Title = createDto.Title,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                AttachmentUrl = null,
                Feedback = null,
                SubmittedDate = null,
                ReviewedDate = null,
                ReviewerId = null
            };

            try
            {
                await _lessonPlanRepository.AddLessonPlanAsync(lessonPlan);

                var assignedTeacher = await _teacherRepository.GetByIdAsync(createDto.TeacherId);
                if (assignedTeacher != null)
                {
                    var subject = await _subjectRepository.GetByIdAsync(createDto.SubjectId);
                    if (subject != null)
                    {
                        try
                        {
                            var teacherEmail = await _teacherService.GetEmailByTeacherIdAsync(createDto.TeacherId);
                            if (!string.IsNullOrEmpty(teacherEmail))
                            {
                                await _emailService.SendLessonPlanNotificationAsync(
                                    teacherEmail: teacherEmail,
                                    teacherName: assignedTeacher.FullName,
                                    planTitle: createDto.Title,
                                    subjectName: subject.SubjectName,
                                    semesterId: createDto.SemesterId,
                                    startDate: createDto.StartDate,
                                    endDate: createDto.EndDate
                                );
                                Console.WriteLine($"Đã gửi email thông báo kế hoạch bài giảng đến {teacherEmail}.");
                            }
                            else
                            {
                                Console.WriteLine($"Không tìm thấy email cho giáo viên với TeacherId {createDto.TeacherId}.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Không thể gửi email thông báo kế hoạch bài giảng đến TeacherId {createDto.TeacherId}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Không tìm thấy môn học với SubjectId {createDto.SubjectId}.");
                    }
                }
                else
                {
                    Console.WriteLine($"Không tìm thấy giáo viên với TeacherId {createDto.TeacherId}.");
                }

                var createdPlan = await _lessonPlanRepository.GetLessonPlanByIdIncludingDetailsAsync(lessonPlan.PlanId);
                return _mapper.Map<LessonPlanResponseDto>(createdPlan);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể tạo giáo án do lỗi hệ thống.", ex);
            }
        }

        public async Task UpdateMyLessonPlanAsync(int planId, LessonPlanUpdateDto updateDto)
        {
            var teacherId = GetCurrentTeacherId();
            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(planId);

            if (lessonPlan == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy giáo án với ID {planId}.");
            }

            if (lessonPlan.TeacherId != teacherId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật giáo án này.");
            }

            if (lessonPlan.Status == "Đã duyệt" || lessonPlan.Status == "Đã nộp")
            {
                throw new InvalidOperationException($"Không thể cập nhật giáo án với trạng thái '{lessonPlan.Status}'.");
            }

            DateTime currentDate = DateTime.SpecifyKind(DateTime.Now.Date, DateTimeKind.Unspecified);
            if (lessonPlan.StartDate.HasValue && currentDate < lessonPlan.StartDate.Value.Date)
            {
                throw new InvalidOperationException($"Bạn chỉ có thể bắt đầu cập nhật giáo án từ ngày {lessonPlan.StartDate.Value:dd/MM/yyyy}.");
            }

            if (lessonPlan.EndDate.HasValue && currentDate > lessonPlan.EndDate.Value.Date)
            {
                Console.WriteLine($"Cảnh báo: Cập nhật giáo án (ID: {planId}) sau thời hạn.");
            }

            lessonPlan.PlanContent = updateDto.PlanContent;
            lessonPlan.Title = updateDto.Title;
            lessonPlan.AttachmentUrl = updateDto.AttachmentUrl;
            lessonPlan.SubmittedDate = DateTime.Now;
            lessonPlan.Status = "Chờ duyệt";

            try
            {
                await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể cập nhật giáo án do lỗi hệ thống.", ex);
            }
        }

        public async Task ReviewLessonPlanAsync(LessonPlanReviewDto reviewDto)
        {
            if (reviewDto == null || reviewDto.PlanId <= 0 || string.IsNullOrEmpty(reviewDto.Status))
            {
                throw new ArgumentException("PlanId và trạng thái là bắt buộc.");
            }

            if (reviewDto.Status != "Đã duyệt" && reviewDto.Status != "Từ chối")
            {
                throw new ArgumentException("Trạng thái phải là 'Đã duyệt' hoặc 'Từ chối'.");
            }

            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(reviewDto.PlanId);
            if (lessonPlan == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy giáo án với ID {reviewDto.PlanId}.");
            }

            var reviewerId = GetCurrentTeacherId();
            var reviewer = await _teacherRepository.GetByIdAsync(reviewerId);
            if (reviewer == null || !(reviewer.IsHeadOfDepartment ?? false))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng bộ môn mới có quyền duyệt giáo án.");
            }

            lessonPlan.Status = reviewDto.Status;
            lessonPlan.Feedback = reviewDto.Feedback;
            lessonPlan.ReviewedDate = DateTime.Now;
            lessonPlan.ReviewerId = reviewerId;

            try
            {
                await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan);

                if (lessonPlan.Status == "Đã duyệt" || lessonPlan.Status == "Từ chối")
                {
                    var assignedTeacher = await _teacherRepository.GetByIdAsync(lessonPlan.TeacherId);
                    if (assignedTeacher != null)
                    {
                        var subject = await _subjectRepository.GetByIdAsync(lessonPlan.SubjectId);
                        if (subject != null)
                        {
                            try
                            {
                                var teacherEmail = await _teacherService.GetEmailByTeacherIdAsync(lessonPlan.TeacherId);
                                if (!string.IsNullOrEmpty(teacherEmail))
                                {
                                    await _emailService.SendLessonPlanStatusUpdateAsync(
                                        teacherEmail: teacherEmail,
                                        teacherName: assignedTeacher.FullName,
                                        planTitle: lessonPlan.Title,
                                        subjectName: subject.SubjectName,
                                        semesterId: lessonPlan.SemesterId,
                                        status: lessonPlan.Status,
                                        feedback: lessonPlan.Feedback
                                    );
                                    Console.WriteLine($"Đã gửi email cập nhật trạng thái kế hoạch bài giảng đến {teacherEmail}.");
                                }
                                else
                                {
                                    Console.WriteLine($"Không tìm thấy email cho giáo viên với TeacherId {lessonPlan.TeacherId}.");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Không thể gửi email cập nhật trạng thái kế hoạch bài giảng đến TeacherId {lessonPlan.TeacherId}: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Không tìm thấy môn học với SubjectId {lessonPlan.SubjectId}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Không tìm thấy giáo viên với TeacherId {lessonPlan.TeacherId}.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể duyệt giáo án do lỗi hệ thống.", ex);
            }
        }

        public async Task<LessonPlanResponseDto> GetLessonPlanByIdAsync(int planId)
        {
            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdIncludingDetailsAsync(planId);
            if (lessonPlan == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy giáo án với ID {planId}.");
            }

            return _mapper.Map<LessonPlanResponseDto>(lessonPlan);
        }

        public async Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetAllLessonPlansAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (lessonPlans, totalCount) = await _lessonPlanRepository.GetAllLessonPlansIncludingDetailsAsync(pageNumber, pageSize);
                var lessonPlanDtos = _mapper.Map<List<LessonPlanResponseDto>>(lessonPlans);
                return (lessonPlanDtos, totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách giáo án do lỗi hệ thống.", ex);
            }
        }

        public async Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetLessonPlansByTeacherAsync(int teacherId, int pageNumber, int pageSize)
        {
            try
            {
                var (lessonPlans, totalCount) = await _lessonPlanRepository.GetLessonPlansByTeacherIncludingDetailsAsync(teacherId, pageNumber, pageSize);
                var lessonPlanDtos = _mapper.Map<List<LessonPlanResponseDto>>(lessonPlans);
                return (lessonPlanDtos, totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách giáo án của giáo viên do lỗi hệ thống.", ex);
            }
        }

        public async Task<(List<LessonPlanResponseDto> LessonPlans, int TotalCount)> GetLessonPlansByStatusAsync(string status, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("Trạng thái là bắt buộc.");
            }

            try
            {
                var (lessonPlans, totalCount) = await _lessonPlanRepository.GetLessonPlansByStatusIncludingDetailsAsync(status, pageNumber, pageSize);
                var lessonPlanDtos = _mapper.Map<List<LessonPlanResponseDto>>(lessonPlans);
                return (lessonPlanDtos, totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách giáo án theo trạng thái do lỗi hệ thống.", ex);
            }
        }

        private int GetCurrentTeacherId()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.Claims?.ToList() ?? new List<Claim>();
            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
               ?? claims.FirstOrDefault(c => c.Type == "sub");
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy User ID trong token.");
            }

            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID trong token không hợp lệ.");
            }

            var teacher = Task.Run(() => _teacherRepository.GetByUserIdAsync(userId)).GetAwaiter().GetResult();
            if (teacher == null)
            {
                throw new UnauthorizedAccessException($"Không tìm thấy giáo viên với UserID {userId}.");
            }

            return teacher.TeacherId;
        }

        public async Task<LessonPlanStatisticsDto> GetDepartmentHeadLessonPlanStatisticsAsync()
        {

            // Lấy teacherId của trưởng bộ môn từ token
            var departmentHeadTeacherId = GetCurrentTeacherId();

            // Lấy thông tin trưởng bộ môn để biết tổ bộ môn
            var departmentHead = await _teacherRepository.GetByIdAsync(departmentHeadTeacherId);
            if (departmentHead == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy thông tin trưởng bộ môn với TeacherId {departmentHeadTeacherId}.");
            }

            var department = departmentHead.Department;
            if (string.IsNullOrEmpty(department))
            {
                throw new InvalidOperationException("Trưởng bộ môn không thuộc tổ bộ môn nào.");
            }

            // Lấy danh sách giáo viên thuộc cùng tổ bộ môn
            var teachersInDepartment = await _teacherRepository.GetTeachersByDepartmentAsync(department);
            var teacherIdsInDepartment = teachersInDepartment.Select(t => t.TeacherId).ToList();

            // Lấy tất cả giáo án của các giáo viên trong cùng tổ bộ môn
            var lessonPlans = await _lessonPlanRepository.GetLessonPlansByTeacherIdsAsync(teacherIdsInDepartment);

            // Tính toán thống kê
            var statistics = new LessonPlanStatisticsDto
            {
                TotalLessonPlans = lessonPlans.Count(),
                SubmittedCount = lessonPlans.Count(lp => lp.Status == "Đã nộp" || lp.Status == "Đã duyệt"),
                PendingCount = lessonPlans.Count(lp => lp.Status == "Đang chờ"),
                WaitingForApprovalCount = lessonPlans.Count(lp => lp.Status == "Chờ duyệt"),
                RejectedCount = lessonPlans.Count(lp => lp.Status == "Từ chối")
            };

            return statistics;
        }
    }
}