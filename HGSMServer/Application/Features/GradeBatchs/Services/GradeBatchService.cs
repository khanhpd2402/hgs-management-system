using Application.Features.GradeBatchs.DTOs;
using Application.Features.GradeBatchs.Interfaces;
using AutoMapper;
using Common.Constants;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Repositories.UnitOfWork;

namespace Application.Services
{
    public class GradeBatchService : IGradeBatchService
    {
        private readonly IGradeUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GradeBatchService(IGradeUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<GradeBatchDto?> GetByIdAsync(int id)
        {
            var batch = await _unitOfWork.GradeBatchRepository.GetByIdAsync(id);
            return batch == null ? null : _mapper.Map<GradeBatchDto>(batch);
        }

        public async Task<IEnumerable<GradeBatchDto>> GetByAcademicYearIdAsync(int academicYearId)
        {
            var list = await _unitOfWork.GradeBatchRepository.GetAllAsync();
            var result = list.Where(b => b.Semester!.AcademicYearId == academicYearId);
            return _mapper.Map<IEnumerable<GradeBatchDto>>(result);
        }
        public async Task<int> CreateBatchAndInsertGradesAsync(string batchName, int semesterId, DateOnly start, DateOnly end, string status)
        {
            // Validate status trước
            if (!AppConstants.Status.All.Contains(status))
            {
                throw new ArgumentException($"Trạng thái '{status}' không hợp lệ. Các trạng thái hợp lệ: {string.Join(", ", AppConstants.Status.All)}");
            }

            // 1. Lấy dữ liệu cần thiết
            var assignments = await _unitOfWork.TeachingAssignmentRepository.GetBySemesterIdAsync(semesterId);
            var semester = await _unitOfWork.SemesterRepository.GetByIdAsync(semesterId);

            if (semester == null)
            {
                throw new ArgumentException($"Không tìm thấy học kỳ với ID {semesterId}");
            }
            if (start < semester.StartDate || end > semester.EndDate)
            {
                throw new ArgumentException($"Khoảng thời gian của đợt nhập điểm ({start:dd/MM/yyyy} - {end:dd/MM/yyyy}) phải nằm trong khoảng thời gian của học kỳ ({semester.StartDate:dd/MM/yyyy} - {semester.EndDate:dd/MM/yyyy}).");
            }
            if (assignments == null || !assignments.Any())
            {
                throw new InvalidOperationException($"Không có phân công giảng dạy cho học kỳ ID {semesterId}, không thể tạo đợt nhập điểm.");
            }

            var academicYearId = semester.AcademicYearId;
            var allGrades = new List<Grade>();

            foreach (var assignment in assignments)
            {
                var classId = assignment.ClassId;
                var subjectId = assignment.SubjectId;

                var studentClasses = await _unitOfWork.StudentClassRepository.GetByClassIdAndAcademicYearAsync(classId, academicYearId);
                var gls = await _unitOfWork.GradeLevelSubjectRepository.GetByGradeAndSubjectAsync(assignment.Class.GradeLevelId, subjectId);

                if (gls == null || studentClasses == null || !studentClasses.Any())
                {
                    continue; // Không có học sinh hoặc không có cấu hình môn học => skip
                }

                // Sinh đầu điểm
                var assessments = new List<string>();
                bool isSemester1 = semester.SemesterName.Contains("1");

                int continuousCount = isSemester1 ? gls.ContinuousAssessmentsHki : gls.ContinuousAssessmentsHkii;
                for (int i = 1; i <= continuousCount; i++)
                    assessments.Add($"ĐĐG TX {i}");

                if (gls.MidtermAssessments > 0)
                    assessments.Add("ĐĐG GK");

                if (gls.FinalAssessments > 0)
                    assessments.Add("ĐĐG CK");

                foreach (var sc in studentClasses)
                {
                    foreach (var assess in assessments)
                    {
                        allGrades.Add(new Grade
                        {
                            AssignmentId = assignment.AssignmentId,
                            StudentClassId = sc.Id,
                            AssessmentsTypeName = assess,
                            Score = null,
                            TeacherComment = null
                        });
                    }
                }
            }

            // 2. Nếu không sinh được điểm nào thì không cho tạo batch
            if (!allGrades.Any())
            {
                throw new InvalidOperationException($"Không thể tạo đợt nhập điểm vì không có học sinh hoặc cấu hình đầu điểm phù hợp cho học kỳ ID {semesterId}.");
            }

            // 3. Tạo batch + Insert điểm
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

            // Gán batchId cho từng grade
            foreach (var grade in allGrades)
            {
                grade.BatchId = batchId;
            }

            await _unitOfWork.GradeRepository.AddRangeAsync(allGrades);

            return batchId;
        }


        public async Task<UpdateGradeBatchDto?> UpdateAsync(int id, UpdateGradeBatchDto dto)
        {
            // Validate status trước
            if (!AppConstants.Status.All.Contains(dto.Status))
            {
                throw new ArgumentException($"Trạng thái '{dto.Status}' không hợp lệ. Các trạng thái hợp lệ: {string.Join(", ", AppConstants.Status.All)}");
            }

            var existing = await _unitOfWork.GradeBatchRepository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing); // Tự động cập nhật thuộc tính

            await _unitOfWork.GradeBatchRepository.UpdateAsync(existing);

            return _mapper.Map<UpdateGradeBatchDto>(existing); // Trả lại DTO sau khi update
        }
    }
}