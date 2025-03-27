using Application.Features.GradeBatchs.DTOs;
using Application.Features.GradeBatchs.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services
{
    public class GradeBatchService : IGradeBatchService
    {
        private readonly IGradeBatchRepository _gradeBatchRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly IMapper _mapper;
        public GradeBatchService(IGradeBatchRepository gradeBatchRepository, IStudentRepository studentRepository, IGradeRepository gradeRepository, IMapper mapper)
        {
            _gradeBatchRepository = gradeBatchRepository;
            _studentRepository = studentRepository;
            _gradeRepository = gradeRepository;
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
                var classId = student.StudentClasses
    .Where(sc => sc.AcademicYearId == academicYearId)
    .Select(sc => sc.ClassId)
    .FirstOrDefault();
                foreach (var subjectId in subjectIds)
                {
                    foreach (var assessmentType in assessmentTypes)
                    {
                        
                        grades.Add(new Grade
                        {
                            BatchId = newBatch.BatchId,
                            StudentId = student.StudentId,
                            ClassId = classId,
                            SubjectId = subjectId,
                            Score = null, // Giáo viên update sau
                            AssessmentsTypeName = assessmentType,
                            TeacherComment = null
                        });
                    }
                }
            }

            // Thêm vào bảng Grades
            await _gradeRepository.AddRangeAsync(grades);

            return _mapper.Map<GradeBatchDto>(newBatch);
        }


        public async Task<bool> UpdateAsync(GradeBatchDto gradeBatchDto)
        {
            var batch = _mapper.Map<GradeBatch>(gradeBatchDto);
            return await _gradeBatchRepository.UpdateAsync(batch);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _gradeBatchRepository.DeleteAsync(id);
        }
    }
}
