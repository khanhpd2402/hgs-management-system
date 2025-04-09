using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories.UnitOfWork
{
    public interface IGradeUnitOfWork
    {
        IGradeBatchRepository GradeBatchRepository { get; }
        IGradeRepository GradeRepository { get; }
        ITeachingAssignmentRepository TeachingAssignmentRepository { get; }
        IStudentClassRepository StudentClassRepository { get; }
        IGradeLevelSubjectRepository GradeLevelSubjectRepository { get; }
        ISemesterRepository SemesterRepository { get; }
        Task<int> SaveChangesAsync();
    }

}
