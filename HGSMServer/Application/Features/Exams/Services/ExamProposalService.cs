using Application.Features.Exams.DTOs;
using Application.Features.Exams.Interfaces;
using AutoMapper;
using Domain.Models;
using HGSMAPI;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Features.Exams.Services
{
    public class ExamProposalService : IExamProposalService
    {
        private readonly IMapper _mapper;
        private readonly IExamProposalRepository _examProposalRepository;
        private readonly GoogleDriveService _googleDriveService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExamProposalService(
            IMapper mapper,
            IExamProposalRepository examProposalRepository,
            GoogleDriveService googleDriveService,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _examProposalRepository = examProposalRepository;
            _googleDriveService = googleDriveService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ExamProposalDto> CreateExamProposalAsync(ExamProposalRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
                throw new Exception("File không được để trống.");

            var allowedExtensions = new[] { ".doc", ".docx" };
            var extension = Path.GetExtension(request.File.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Chỉ hỗ trợ file Word (.doc, .docx).");

            var proposal = new ExamProposal
            {
                SubjectId = request.SubjectId,
                Grade = request.Grade,
                Title = request.Title,
                SemesterId = request.SemesterId,
                CreatedBy = GetCurrentUserId(),
                CreatedDate = DateTime.Now
            };

            // Upload file Word lên Google Drive
            proposal.FileUrl = await _googleDriveService.UploadWordFileAsync(
                request.File,
                request.SubjectId,
                request.Grade,
                request.SubjectName
            );

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

        private int GetCurrentUserId()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.Claims?.ToList() ?? new List<Claim>();
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "sub")
                ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token.");

            return int.Parse(userIdClaim.Value);
        }
    }
}