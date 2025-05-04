using Application.Features.Exams.DTOs;
using Application.Features.Exams.Interfaces;
using AutoMapper;
using Domain.Models;
using HGSMAPI;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Common.Utils;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Application.Features.Exams.Services
{
    public class ExamProposalService : IExamProposalService
    {
        private readonly IMapper _mapper;
        private readonly IExamProposalRepository _examProposalRepository;
        private readonly GoogleDriveService _googleDriveService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISubjectRepository _subjectRepository;
        private readonly EmailService _emailService;
        private readonly ITeacherRepository _teacherRepository;

        public ExamProposalService(
            IMapper mapper,
            IExamProposalRepository examProposalRepository,
            GoogleDriveService googleDriveService,
            IHttpContextAccessor httpContextAccessor,
            ISubjectRepository subjectRepository,
            EmailService emailService,
            ITeacherRepository teacherRepository)
        {
            _mapper = mapper;
            _examProposalRepository = examProposalRepository;
            _googleDriveService = googleDriveService;
            _httpContextAccessor = httpContextAccessor;
            _subjectRepository = subjectRepository;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
        }

        public async Task<ExamProposalDto> CreateExamProposalAsync(ExamProposalRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                throw new ArgumentException("File không được để trống.");
            }

            var allowedExtensions = new[] { ".doc", ".docx" };
            var extension = Path.GetExtension(request.File.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Chỉ hỗ trợ file Word (.doc, .docx).");
            }

            var subject = await _subjectRepository.GetByIdAsync(request.SubjectId);
            if (subject == null)
            {
                throw new KeyNotFoundException("Môn học không tồn tại.");
            }

            var proposal = new ExamProposal
            {
                SubjectId = request.SubjectId,
                Grade = request.Grade,
                Title = request.Title,
                SemesterId = request.SemesterId,
                CreatedBy = GetCurrentTeacherId(),
                CreatedDate = DateTime.Now,
                Status = "Chờ duyệt",
            };

            try
            {
                proposal.FileUrl = await _googleDriveService.UploadWordFileAsync(
                    request.File,
                    request.SubjectId,
                    request.Grade,
                    subject.SubjectName
                );
                proposal.FileUrl = proposal.FileUrl;

                await _examProposalRepository.AddExamProposalAsync(proposal);
                return _mapper.Map<ExamProposalDto>(proposal);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể tạo đề thi do lỗi hệ thống.", ex);
            }
        }

        public async Task<ExamProposalDto> GetExamProposalAsync(int proposalId)
        {
            var proposal = await _examProposalRepository.GetExamProposalAsync(proposalId);
            if (proposal == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đề thi với ID {proposalId}.");
            }

            return _mapper.Map<ExamProposalDto>(proposal);
        }

        public async Task UpdateExamProposalStatusAsync(int proposalId, string status, string? comment = null)
        {
            if (status != "Chờ duyệt" && status != "Đã duyệt" && status != "Từ chối")
            {
                throw new ArgumentException("Trạng thái không hợp lệ. Chỉ chấp nhận: Chờ duyệt, Đã duyệt, Từ chối.");
            }

            var proposal = await _examProposalRepository.GetExamProposalAsync(proposalId);
            if (proposal == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đề thi với ID {proposalId}.");
            }

            proposal.Status = status;
            proposal.Comment = comment;

            try
            {
                if (status == "Từ chối" && !string.IsNullOrEmpty(proposal.FileUrl))
                {
                    await _googleDriveService.DeleteFileAsync(proposal.FileUrl);
                    proposal.FileUrl = null;
                }

                await _examProposalRepository.UpdateExamProposalAsync(proposal);

                if (status == "Đã duyệt" || status == "Từ chối")
                {
                    var teacher = await _teacherRepository.GetByIdAsync(proposal.CreatedBy);
                    if (teacher != null)
                    {
                        var subject = await _subjectRepository.GetByIdAsync(proposal.SubjectId);
                        if (subject != null && !string.IsNullOrEmpty(teacher.User?.Email))
                        {
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await _emailService.SendExamProposalStatusUpdateAsync(
                                        teacherEmail: teacher.User.Email,
                                        planTitle: proposal.Title,
                                        subjectName: subject.SubjectName,
                                        grade: proposal.Grade,
                                        semesterId: proposal.SemesterId,
                                        status: status,
                                        feedback: comment
                                    );
                                    Console.WriteLine($"Đã gửi email thông báo trạng thái đề thi đến {teacher.User.Email}.");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Không thể gửi email thông báo trạng thái đề thi đến {teacher.User.Email}: {ex.Message}");
                                }
                            });
                        }
                        else
                        {
                            Console.WriteLine($"Không thể gửi email: Môn học không tồn tại hoặc email giáo viên (TeacherId: {proposal.CreatedBy}) không hợp lệ.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Không tìm thấy giáo viên với TeacherId {proposal.CreatedBy}. Bỏ qua gửi email.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể cập nhật trạng thái đề thi do lỗi hệ thống.", ex);
            }
        }

        public async Task<IEnumerable<ExamProposalDto>> GetExamProposalsByStatusAsync(string status)
        {
            if (status != "Chờ duyệt" && status != "Đã duyệt" && status != "Từ chối")
            {
                throw new ArgumentException("Trạng thái không hợp lệ. Chỉ chấp nhận: Chờ duyệt, Đã duyệt, Từ chối.");
            }

            try
            {
                var proposals = await _examProposalRepository.GetExamProposalsByStatusAsync(status);
                return _mapper.Map<IEnumerable<ExamProposalDto>>(proposals);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách đề thi theo trạng thái do lỗi hệ thống.", ex);
            }
        }

        public async Task<ExamProposalDto> UpdateExamProposalAsync(int proposalId, ExamProposalUpdateDto request)
        {
            var proposal = await _examProposalRepository.GetExamProposalAsync(proposalId);
            if (proposal == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đề thi với ID {proposalId}.");
            }

            if (proposal.Status != "Chờ duyệt")
            {
                throw new InvalidOperationException("Chỉ có thể sửa đề thi ở trạng thái Chờ duyệt.");
            }

            if (!string.IsNullOrEmpty(request.Title))
            {
                proposal.Title = request.Title;
            }

            try
            {
                if (request.File != null && request.File.Length > 0)
                {
                    if (!string.IsNullOrEmpty(proposal.FileUrl))
                    {
                        await _googleDriveService.DeleteFileAsync(proposal.FileUrl);
                    }

                    var subject = await _subjectRepository.GetByIdAsync(proposal.SubjectId);
                    if (subject == null)
                    {
                        throw new KeyNotFoundException("Môn học không tồn tại.");
                    }

                    proposal.FileUrl = await _googleDriveService.UploadWordFileAsync(
                        request.File,
                        proposal.SubjectId,
                        proposal.Grade,
                        subject.SubjectName
                    );
                }

                await _examProposalRepository.UpdateExamProposalAsync(proposal);
                return _mapper.Map<ExamProposalDto>(proposal);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể cập nhật đề thi do lỗi hệ thống.", ex);
            }
        }

        public async Task<IEnumerable<ExamProposalDto>> GetAllAsync()
        {
            try
            {
                var proposals = await _examProposalRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<ExamProposalDto>>(proposals);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách tất cả đề thi do lỗi hệ thống.", ex);
            }
        }

        public async Task<IEnumerable<ExamProposalDto>> GetAllByTeacherIdAsync(int teacherId)
        {
            try
            {
                var proposals = await _examProposalRepository.GetAllByTeacherIdAsync(teacherId);
                return _mapper.Map<IEnumerable<ExamProposalDto>>(proposals);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách đề thi của giáo viên do lỗi hệ thống.", ex);
            }
        }

        private int GetCurrentTeacherId()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.Claims?.ToList() ?? new List<Claim>();
            var teacherIdClaim = claims.FirstOrDefault(c => c.Type == "teacherId");
            if (teacherIdClaim == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy Teacher ID trong token.");
            }

            if (!int.TryParse(teacherIdClaim.Value, out var teacherId))
            {
                throw new InvalidOperationException("Teacher ID trong token không hợp lệ.");
            }

            return teacherId;
        }
        public async Task<ExamProposalStatisticsDto> GetDepartmentHeadExamProposalStatisticsAsync()
        {
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Trưởng bộ môn")
            {
                throw new UnauthorizedAccessException("Chỉ Trưởng bộ môn có quyền truy cập thống kê này.");
            }

            var examProposals = await _examProposalRepository.GetAllAsync();
            var statistics = new ExamProposalStatisticsDto
            {
                TotalExamProposals = examProposals.Count(),
                SubmittedCount = examProposals.Count(ep => ep.Status == "Đã duyệt"),
                PendingCount = examProposals.Count(ep => ep.Status == "Từ chối"),
                WaitingForApprovalCount = examProposals.Count(ep => ep.Status == "Chờ duyệt")
            };

            return statistics;
        }
    }
}