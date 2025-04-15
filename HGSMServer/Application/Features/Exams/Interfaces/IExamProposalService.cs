using Application.Features.Exams.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Exams.Interfaces
{
    public interface IExamProposalService
    {
        Task<ExamProposalDto> CreateExamProposalAsync(ExamProposalRequestDto request);
        Task<ExamProposalDto> GetExamProposalAsync(int proposalId);
    }
}