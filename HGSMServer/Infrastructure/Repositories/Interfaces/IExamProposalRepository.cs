using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IExamProposalRepository
    {
        Task AddExamProposalAsync(ExamProposal proposal);
        Task<ExamProposal> GetExamProposalAsync(int proposalId);
    }
}