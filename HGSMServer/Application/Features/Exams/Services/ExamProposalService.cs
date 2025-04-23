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
                throw new Exception("File không được để trống.");

            var allowedExtensions = new[] { ".doc", ".docx" };
            var extension = Path.GetExtension(request.File.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Chỉ hỗ trợ file Word (.doc, .docx).");

            var subject = await _subjectRepository.GetByIdAsync(request.SubjectId);
            if (subject == null)
            {
                throw new Exception("Môn học không tồn tại.");
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

        public async Task<ExamProposalDto> GetExamProposalAsync(int proposalId)
        {
            var proposal = await _examProposalRepository.GetExamProposalAsync(proposalId);
            if (proposal == null)
                throw new Exception("Đề thi không tồn tại.");

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
                throw new Exception("Đề thi không tồn tại.");
            }

            proposal.Status = status;
            proposal.Comment = comment;

            if (status == "Từ chối" && !string.IsNullOrEmpty(proposal.FileUrl))
            {
                await _googleDriveService.DeleteFileAsync(proposal.FileUrl);
                proposal.FileUrl = null;
            }

            await _examProposalRepository.UpdateExamProposalAsync(proposal);

            // Gửi email thông báo khi trạng thái thay đổi thành "Đã duyệt" hoặc "Từ chối"
            if (status == "Đã duyệt" || status == "Từ chối")
            {
                var teacher = await _teacherRepository.GetByIdAsync(proposal.CreatedBy);
                if (teacher != null)
                {
                    var subject = await _subjectRepository.GetByIdAsync(proposal.SubjectId);
                    if (subject != null)
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
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Không thể gửi email thông báo trạng thái đề thi đến {teacher.User.Email}: {ex.Message}");
                        }
                    }
                }
            }
        }

        public async Task<IEnumerable<ExamProposalDto>> GetExamProposalsByStatusAsync(string status)
        {
            if (status != "Chờ duyệt" && status != "Đã duyệt" && status != "Từ chối")
            {
                throw new ArgumentException("Trạng thái không hợp lệ. Chỉ chấp nhận: Chờ duyệt, Đã duyệt, Từ chối.");
            }
            var proposals = await _examProposalRepository.GetExamProposalsByStatusAsync(status);
            return _mapper.Map<IEnumerable<ExamProposalDto>>(proposals);
        }

        public async Task<ExamProposalDto> UpdateExamProposalAsync(int proposalId, ExamProposalUpdateDto request)
        {
            var proposal = await _examProposalRepository.GetExamProposalAsync(proposalId);
            if (proposal == null)
            {
                throw new Exception("Đề thi không tồn tại.");
            }

            if (proposal.Status != "Chờ duyệt")
            {
                throw new Exception("Chỉ có thể sửa đề thi ở trạng thái Chờ duyệt.");
            }

            if (!string.IsNullOrEmpty(request.Title))
            {
                proposal.Title = request.Title;
            }

            if (request.File != null && request.File.Length > 0)
            {
                if (!string.IsNullOrEmpty(proposal.FileUrl))
                {
                    await _googleDriveService.DeleteFileAsync(proposal.FileUrl);
                }

                var subject = await _subjectRepository.GetByIdAsync(proposal.SubjectId);
                if (subject == null)
                {
                    throw new Exception("Môn học không tồn tại.");
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

        public async Task<IEnumerable<ExamProposalDto>> GetAllAsync()
        {
            var proposals = await _examProposalRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ExamProposalDto>>(proposals);
        }

        public async Task<IEnumerable<ExamProposalDto>> GetAllByTeacherIdAsync(int teacherId)
        {
            var proposals = await _examProposalRepository.GetAllByTeacherIdAsync(teacherId);
            return _mapper.Map<IEnumerable<ExamProposalDto>>(proposals);
        }

        private int GetCurrentTeacherId()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.Claims?.ToList() ?? new List<Claim>();
            var teacherIdClaim = claims.FirstOrDefault(c => c.Type == "teacherId")
                ?? throw new UnauthorizedAccessException("Teacher ID not found in token.");

            return int.Parse(teacherIdClaim.Value);
        }
    }
}