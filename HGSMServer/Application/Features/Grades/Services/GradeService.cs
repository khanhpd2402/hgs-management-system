using AutoMapper;
using Infrastructure.Repositories.Interfaces;
using Application.Features.Grades.Interfaces;
using Application.Features.Grades.DTOs;
using Domain.Models;
using Infrastructure.Repositories.Implementtations;

namespace Application.Features.Grades.Services
{
    public class GradeService : IGradeService
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;
        private readonly IMapper _mapper;

        public GradeService(IGradeRepository gradeRepository, IStudentClassRepository studentClassRepository, ITeachingAssignmentRepository teachingAssignmentRepository, IMapper mapper)
        {
            _gradeRepository = gradeRepository;
            _studentClassRepository = studentClassRepository;
            _teachingAssignmentRepository = teachingAssignmentRepository;
            _mapper = mapper;
        }

        public async Task AddGradesAsync(IEnumerable<GradeDto> gradeDtos)
        {
            var grades = new List<Grade>();
            foreach (var dto in gradeDtos)
            {
                var studentClass = await _studentClassRepository.GetStudentClassByStudentAndClassIdAsync(dto.StudentId, dto.ClassId);
                var assignment = await _teachingAssignmentRepository.GetAssignmentByClassSubjectTeacherAsync(dto.ClassId, dto.SubjectId, dto.TeacherId);

                if (studentClass == null || assignment == null)
                    throw new Exception("Invalid StudentClassId or AssignmentId");

                var grade = new Grade
                {
                    BatchId = dto.BatchId,
                    StudentClassId = studentClass.Id,
                    AssignmentId = assignment.AssignmentId,
                    AssessmentsTypeName = dto.AssessmentsTypeName,
                    Score = dto.Score,
                    TeacherComment = dto.TeacherComment
                };
                grades.Add(grade);
            }

            await _gradeRepository.AddRangeAsync(grades);
        }
       
        public async Task<List<GradeRespondDto>> GetGradesForStudentAsync(int studentId, int semesterId)
        {
            var grades = await _gradeRepository.GetGradesByStudentAsync(studentId, semesterId);
            return _mapper.Map<List<GradeRespondDto>>(grades);
        }

        public async Task<List<GradeRespondDto>> GetGradesForTeacherAsync(int teacherId, int classId, int subjectId, int semesterId)
        {
            var grades = await _gradeRepository.GetGradesByTeacherAsync(teacherId, classId, subjectId, semesterId);
            return _mapper.Map<List<GradeRespondDto>>(grades);
        }

        public async Task<List<GradeRespondDto>> GetGradesForPrincipalAsync(int classId, int subjectId, int semesterId)
        {
            var grades = await _gradeRepository.GetGradesByClassAsync(classId, subjectId, semesterId);
            return _mapper.Map<List<GradeRespondDto>>(grades);
        }
        public async Task<bool> UpdateMultipleGradesAsync(UpdateMultipleGradesDto dto)
        {
            var gradeIds = dto.Grades.Select(g => g.GradeID).ToList();
            var gradeEntities = await _gradeRepository.GetGradesByIdsAsync(gradeIds);

            if (gradeEntities.Count == 0) return false;

            foreach (var gradeEntity in gradeEntities)
            {
                var updateData = dto.Grades.FirstOrDefault(g => g.GradeID == gradeEntity.GradeId);
                if (updateData != null)
                {
                    gradeEntity.Score = updateData.Score;
                    gradeEntity.TeacherComment = updateData.TeacherComment;
                }
            }

            return await _gradeRepository.UpdateMultipleGradesAsync(gradeEntities);
        }

    }
}
