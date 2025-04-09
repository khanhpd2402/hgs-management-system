using Domain.Models;
using Infrastructure.Repositories.Interfaces;


namespace Infrastructure.Repositories.UnitOfWork
{
    public class GradeUnitOfWork : IGradeUnitOfWork
    {
        private readonly HgsdbContext _context;

        public GradeUnitOfWork(
            IGradeBatchRepository gradeBatchRepo,
            IGradeRepository gradeRepo,
            ITeachingAssignmentRepository assignmentRepo,
            IStudentClassRepository studentClassRepo,
            IGradeLevelSubjectRepository glsRepo, ISemesterRepository semesterRepository,
            HgsdbContext context)
        {
            GradeBatchRepository = gradeBatchRepo;
            GradeRepository = gradeRepo;
            TeachingAssignmentRepository = assignmentRepo;
            StudentClassRepository = studentClassRepo;
            GradeLevelSubjectRepository = glsRepo;
            SemesterRepository = semesterRepository;
            _context = context;
        }
        public ISemesterRepository SemesterRepository { get; }
        public IGradeBatchRepository GradeBatchRepository { get; }
        public IGradeRepository GradeRepository { get; }
        public ITeachingAssignmentRepository TeachingAssignmentRepository { get; }
        public IStudentClassRepository StudentClassRepository { get; }
        public IGradeLevelSubjectRepository GradeLevelSubjectRepository { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
