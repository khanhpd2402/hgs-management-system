using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class ExamProposalRepository : IExamProposalRepository
    {
        private readonly HgsdbContext _context;

        public ExamProposalRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task AddExamProposalAsync(ExamProposal proposal)
        {
            _context.ExamProposals.Add(proposal);
            await _context.SaveChangesAsync();
        }

        public async Task<ExamProposal> GetExamProposalAsync(int proposalId)
        {
            return await _context.ExamProposals
                .FirstOrDefaultAsync(ep => ep.ProposalId == proposalId);
        }
    }
}