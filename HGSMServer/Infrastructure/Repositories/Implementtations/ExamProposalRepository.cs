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

        public async Task UpdateExamProposalStatusAsync(int proposalId, string status)
        {
            var proposal = await _context.ExamProposals.FindAsync(proposalId);
            if (proposal != null)
            {
                proposal.Status = status;
                _context.Entry(proposal).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ExamProposal>> GetExamProposalsByStatusAsync(string status)
        {
            return await _context.ExamProposals
                .Where(ep => ep.Status == status)
                .ToListAsync();
        }
        public async Task UpdateExamProposalAsync(ExamProposal proposal)
        {
            _context.Entry(proposal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}