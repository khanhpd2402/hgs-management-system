using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IExamProposalRepository
    {
        Task AddExamProposalAsync(ExamProposal proposal);
        Task<ExamProposal> GetExamProposalAsync(int proposalId);
        Task UpdateExamProposalStatusAsync(int proposalId, string status);
        Task<IEnumerable<ExamProposal>> GetExamProposalsByStatusAsync(string status);
        Task UpdateExamProposalAsync(ExamProposal proposal);
    }
}