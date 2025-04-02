using Application.Features.Exams.DTOs;
using Application.Features.Exams.Interfaces;
using AutoMapper;
using Domain.Models;
using HGSMAPI;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Features.Exams.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;
        private readonly GoogleDriveService _googleDriveService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionService(
            IQuestionRepository questionRepository,
            IMapper mapper,
            GoogleDriveService googleDriveService,
            IHttpContextAccessor httpContextAccessor)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
            _googleDriveService = googleDriveService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<QuestionDto> AddQuestionAsync(QuestionDto questionDto, IFormFile imageFile = null)
        {
            var question = _mapper.Map<Question>(questionDto);
            question.CreatedBy = GetCurrentUserId();
            question.CreatedDate = DateTime.Now;

            if (imageFile != null)
            {
                question.ImageUrl = await _googleDriveService.UploadImageAsync(imageFile);
            }

            await _questionRepository.AddQuestionAsync(question);
            return _mapper.Map<QuestionDto>(question);
        }

        public async Task<List<QuestionDto>> GetAllQuestionsAsync(int subjectId, int grade)
        {
            var questions = await _questionRepository.GetQuestionsBySubjectAndGradeAsync(subjectId, grade);
            return _mapper.Map<List<QuestionDto>>(questions);
        }

        public async Task<ExamProposalDto> CreateExamProposalAsync(ExamProposalRequestDto request)
        {
            var proposal = new ExamProposal
            {
                SubjectId = request.SubjectId,
                Grade = request.Grade,
                Title = request.Title,
                SemesterId = request.SemesterId,
                CreatedBy = GetCurrentUserId(),
                CreatedDate = DateTime.Now
            };

            var questions = await _questionRepository.GetQuestionsByIdsAsync(request.QuestionIds);
            if (questions.Count != request.QuestionIds.Count)
                throw new Exception("Một số câu hỏi không tồn tại.");

            var proposalQuestions = request.QuestionIds.Select((qId, index) => new ExamProposalQuestion
            {
                QuestionId = qId,
                OrderNumber = index + 1
            }).ToList();

            await _questionRepository.AddExamProposalAsync(proposal, proposalQuestions);
            var proposalDto = _mapper.Map<ExamProposalDto>(proposal);
            proposalDto.Questions = _mapper.Map<List<QuestionDto>>(questions);
            return proposalDto;
        }

        public async Task<ExamProposalDto> GetExamProposalAsync(int proposalId)
        {
            var proposal = await _questionRepository.GetExamProposalAsync(proposalId);
            if (proposal == null)
                throw new Exception("Đề thi không tồn tại.");

            var proposalDto = _mapper.Map<ExamProposalDto>(proposal);
            proposalDto.Questions = _mapper.Map<List<QuestionDto>>(proposal.ExamProposalQuestions.Select(epq => epq.Question).ToList());
            return proposalDto;
        }

        private int GetCurrentUserId()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.Claims?.ToList() ?? new List<Claim>();
            Console.WriteLine($"Claims: {string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}"))}");

            var userIdClaim = claims.FirstOrDefault(c => c.Type == "sub")
                ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token.");

            var userId = int.Parse(userIdClaim.Value);
            Console.WriteLine($"Extracted UserId: {userId}");
            return userId;
        }
        public async Task<QuestionDto> GetQuestionByIdAsync(int id)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(id);
            if (question == null)
                throw new Exception("Câu hỏi không tồn tại.");
            return _mapper.Map<QuestionDto>(question);
        }
    }
}