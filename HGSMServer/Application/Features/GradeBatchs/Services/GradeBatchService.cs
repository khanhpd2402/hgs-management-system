using Application.Features.GradeBatchs.DTOs;
using Application.Features.GradeBatchs.Interfaces;
using Application.Features.Subjects.DTOs;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Implementtations;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services
{
    public class GradeBatchService : IGradeBatchService
    {
        private readonly IGradeBatchRepository _gradeBatchRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;
        private readonly IMapper _mapper;

        public GradeBatchService(IGradeBatchRepository gradeBatchRepository, IStudentRepository studentRepository, IGradeRepository gradeRepository,
            IStudentClassRepository studentClassRepository, ITeachingAssignmentRepository teachingAssignmentRepository, IMapper mapper)
        {
            _gradeBatchRepository = gradeBatchRepository;
            _studentRepository = studentRepository;
            _gradeRepository = gradeRepository;
            _studentClassRepository = studentClassRepository;
            _teachingAssignmentRepository = teachingAssignmentRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<GradeBatchDto>> GetAllAsync()
        {
            var batches = await _gradeBatchRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GradeBatchDto>>(batches);
        }

        public async Task<GradeBatchDto?> GetByIdAsync(int id)
        {
            var batch = await _gradeBatchRepository.GetByIdAsync(id);
            return batch == null ? null : _mapper.Map<GradeBatchDto>(batch);
        }

        public async Task<GradeBatchDto> CreateAsync(GradeBatchDto gradeBatchDto, List<int> subjectIds, List<string> assessmentTypes)
        {
            // Map từ DTO sang entity
            var batch = _mapper.Map<GradeBatch>(gradeBatchDto);

            // Thêm vào GradeBatch
            var newBatch = await _gradeBatchRepository.AddAsync(batch);
            int academicYearId = await _studentRepository.GetAcademicYearIdAsync(batch.SemesterId);
            // Lấy danh sách học sinh theo học kỳ
            var students = await _studentRepository.GetAllWithParentsAsync(academicYearId);

            var grades = new List<Grade>();

            // Tạo grade cho toàn bộ học sinh
            foreach (var student in students)
            {
                var studentClass = await _studentClassRepository.GetStudentClassByStudentAndClassIdAsync(
     student.StudentId,
     student.StudentClasses.FirstOrDefault(sc => sc.AcademicYearId == academicYearId).ClassId);
                foreach (var subjectId in subjectIds)
                {
                    foreach (var assessmentType in assessmentTypes)
                    {


                        var assignment = await _teachingAssignmentRepository.GetAssignmentByClassSubjectTeacherAsync(studentClass.Id, subjectId, batch.SemesterId);

                        if (studentClass == null || assignment == null)
                            throw new Exception("Invalid StudentClassId or AssignmentId");

                        var grade = new Grade
                        {
                            BatchId = newBatch.BatchId,
                            StudentClassId = studentClass.Id,
                            AssignmentId = assignment.AssignmentId,
                            AssessmentsTypeName = assessmentType,
                            Score = null,
                            TeacherComment = null
                        };
                        grades.Add(grade);
                    }
                }
            }

            // Thêm vào bảng Grades
            await _gradeRepository.AddRangeAsync(grades);

            return _mapper.Map<GradeBatchDto>(newBatch);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _gradeBatchRepository.DeleteAsync(id);
        }
        public async Task<GradeBatchDetailResponseDto> GetGradeBatchDetailAsync(int batchId)
        {
            var batch = await _gradeBatchRepository.GetGradeBatchWithGradesAsync(batchId);

            var classIds = batch.Grades
                .Select(g => g.StudentClass!.ClassId) // Lấy ClassId từ StudentClass
                .Distinct()
                .ToList();

            var subjects = batch.Grades
                .Select(g => new SubjectGradeBatchDto
                {
                    SubjectId = g.Assignment!.SubjectId, // Lấy SubjectId từ Assignment
                    SubjectName = g.Assignment.Subject.SubjectName
                })
                .DistinctBy(x => x.SubjectId)
                .ToList();

            var assessmentTypes = batch.Grades
                .Select(g => new AssessmentTypeGradeBatchDto
                {
                    SubjectId = g.Assignment!.SubjectId, // Lấy SubjectId từ Assignment
                    AssessmentTypeName = g.AssessmentsTypeName,
                    ClassId = g.StudentClass!.ClassId // Lấy ClassId từ StudentClass
                })
                .DistinctBy(x => new { x.SubjectId, x.AssessmentTypeName, x.ClassId })
                .ToList();

            return new GradeBatchDetailResponseDto
            {
                BatchId = batch.BatchId,
                BatchName = batch.BatchName,
                SemesterId = batch.SemesterId,
                StartDate = batch.StartDate,
                EndDate = batch.EndDate,
                Status = batch.Status,
                ClassIds = classIds,
                Subjects = subjects,
                AssessmentTypes = assessmentTypes
            };
        }


        public async Task<GradeBatchDto> UpdateAsync(GradeBatchToCreateDto request)
        {
            var batch = await _gradeBatchRepository.GetByIdAsync(request.GradeBatch.BatchId);
            if (batch == null)
                throw new Exception("GradeBatch not found");

            // 1. Update thông tin GradeBatch
            batch.BatchName = request.GradeBatch.BatchName;
            batch.SemesterId = request.GradeBatch.SemesterId;
            batch.StartDate = request.GradeBatch.StartDate;
            batch.EndDate = request.GradeBatch.EndDate;
            batch.Status = request.GradeBatch.Status;

            await _gradeBatchRepository.UpdateAsync(batch);

            // 2. Kiểm tra xem có cần thêm đầu điểm mới không
            var existingGrades = await _gradeRepository.GetByBatchIdAsync(batch.BatchId);
            var existingKeys = existingGrades
                .Select(g => new { g.AssignmentId, g.AssessmentsTypeName })
                .Distinct()
                .ToList();

            var academicYearId = await _studentRepository.GetAcademicYearIdAsync(batch.SemesterId);
            var students = await _studentRepository.GetAllWithParentsAsync(academicYearId);

            var newGrades = new List<Grade>();

            foreach (var subjectId in request.SubjectIds)
            {
                foreach (var assessmentType in request.AssessmentTypes)
                {
                    foreach (var student in students)
                    {
                        var studentClass = await _studentClassRepository.GetStudentClassByStudentAndClassIdAsync(student.StudentId, student.StudentId);
                        if (studentClass == null) continue;

                        var assignment = await _teachingAssignmentRepository.GetAssignmentByClassSubjectTeacherAsync(student.StudentId, subjectId, subjectId);
                        if (assignment == null) continue;

                        bool exists = existingKeys.Any(x => x.AssignmentId == assignment.AssignmentId && x.AssessmentsTypeName == assessmentType);
                        if (exists)
                            continue;

                        newGrades.Add(new Grade
                        {
                            BatchId = batch.BatchId,
                            StudentClassId = studentClass.Id,
                            AssignmentId = assignment.AssignmentId,
                            AssessmentsTypeName = assessmentType,
                            Score = null,
                            TeacherComment = null
                        });
                    }
                }
            }

            if (newGrades.Any())
            {
                await _gradeRepository.AddRangeAsync(newGrades);
            }

            return new GradeBatchDto
            {
                BatchId = batch.BatchId,
                BatchName = batch.BatchName,
                SemesterId = batch.SemesterId,
                StartDate = batch.StartDate,
                EndDate = batch.EndDate,
                Status = batch.Status 
            };
        }


    }
}
