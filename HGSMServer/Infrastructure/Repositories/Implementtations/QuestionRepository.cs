using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly HgsdbContext _context;

        public QuestionRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task AddQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Question>> GetQuestionsBySubjectAndGradeAsync(int subjectId, int grade)
        {
            return await _context.Questions
                .Where(q => q.SubjectId == subjectId && q.Grade == grade)
                .ToListAsync();
        }

        public async Task<List<Question>> GetQuestionsByIdsAsync(List<int> questionIds)
        {
            return await _context.Questions
                .Where(q => questionIds.Contains(q.QuestionId))
                .ToListAsync();
        }

        public async Task AddExamProposalAsync(ExamProposal proposal, List<ExamProposalQuestion> proposalQuestions)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.ExamProposals.Add(proposal);
                await _context.SaveChangesAsync();

                foreach (var pq in proposalQuestions)
                {
                    pq.ProposalId = proposal.ProposalId;
                }
                _context.ExamProposalQuestions.AddRange(proposalQuestions);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ExamProposal> GetExamProposalAsync(int proposalId)
        {
            return await _context.ExamProposals
                .Include(ep => ep.ExamProposalQuestions)
                .ThenInclude(epq => epq.Question)
                .FirstOrDefaultAsync(ep => ep.ProposalId == proposalId);
        }
        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            return await _context.Questions
                .FirstOrDefaultAsync(q => q.QuestionId == id);
        }
    }
}