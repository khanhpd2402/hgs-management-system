using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task AddQuestionAsync(Question question);
        Task<List<Question>> GetQuestionsBySubjectAndGradeAsync(int subjectId, int grade);
        Task<List<Question>> GetQuestionsByIdsAsync(List<int> questionIds);
        Task AddExamProposalAsync(ExamProposal proposal, List<ExamProposalQuestion> proposalQuestions);
        Task<ExamProposal> GetExamProposalAsync(int proposalId);
        Task<Question> GetQuestionByIdAsync(int id);
    }
}