using Application.Features.Exams.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Exams.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionDto> AddQuestionAsync(QuestionDto questionDto, IFormFile imageFile);
        Task<List<QuestionDto>> GetAllQuestionsAsync(int subjectId, int grade);
        Task<ExamProposalDto> CreateExamProposalAsync(ExamProposalRequestDto request);
        Task<ExamProposalDto> GetExamProposalAsync(int proposalId);
        Task<QuestionDto> GetQuestionByIdAsync(int id);
    }
}