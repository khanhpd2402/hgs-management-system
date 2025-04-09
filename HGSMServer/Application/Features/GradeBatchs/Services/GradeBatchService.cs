using Application.Features.GradeBatchs.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Repositories.UnitOfWork;

namespace Application.Services
{
    public class GradeBatchService : IGradeBatchService
    {
        private readonly IGradeUnitOfWork _unitOfWork;

        public GradeBatchService(IGradeUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateBatchAndInsertGradesAsync(string batchName, int semesterId, DateOnly start, DateOnly end, string status)
        {
            // 1. Tạo đợt nhập điểm
            var newBatch = new GradeBatch
            {
                BatchName = batchName,
                SemesterId = semesterId,
                StartDate = start,
                EndDate = end,
                Status = status
            };

            await _unitOfWork.GradeBatchRepository.AddAsync(newBatch);
            var batchId = newBatch.BatchId;

            // 2. Lấy danh sách phân công giảng dạy
            var assignments = await _unitOfWork.TeachingAssignmentRepository.GetBySemesterIdAsync(semesterId);
            var semester = await _unitOfWork.SemesterRepository.GetByIdAsync(semesterId);
            var allGrades = new List<Grade>();

            foreach (var assignment in assignments)
            {
                var classId = assignment.ClassId;
                var subjectId = assignment.SubjectId;

                var studentClasses = await _unitOfWork.StudentClassRepository.GetByClassIdAsync(classId);
                var gls = await _unitOfWork.GradeLevelSubjectRepository.GetByGradeAndSubjectAsync(assignment.Class.GradeLevelId, subjectId);
                if (gls == null) continue;

                // Sinh các loại đầu điểm dựa vào tên học kỳ
                var assessments = new List<string>();
                bool isSemester1 = semester.SemesterName.Contains("1");

                int continuousCount = isSemester1 ? gls.ContinuousAssessmentsHki : gls.ContinuousAssessmentsHkii;
                for (int i = 1; i <= continuousCount; i++)
                    assessments.Add($"ĐĐG TX {i}");

                if (gls.MidtermAssessments > 0)
                    assessments.Add("ĐĐG GK");

                if ( gls.FinalAssessments > 0)
                    assessments.Add("ĐĐG CK");

                // Tạo điểm rỗng
                foreach (var sc in studentClasses)
                {
                    foreach (var assess in assessments)
                    {
                        allGrades.Add(new Grade
                        {
                            BatchId = batchId,
                            AssignmentId = assignment.AssignmentId,
                            StudentClassId = sc.Id,
                            AssessmentsTypeName = assess,
                            Score = null,
                            TeacherComment = null
                        });
                    }
                }
            }
            await _unitOfWork.GradeRepository.AddRangeAsync(allGrades);
            return batchId;
        }
    }
}