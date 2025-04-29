using AutoMapper;
using Infrastructure.Repositories.Interfaces;
using Application.Features.Grades.Interfaces;
using Application.Features.Grades.DTOs;
using Domain.Models;
using Infrastructure.Repositories.Implementtations;
using Common.Constants;

namespace Application.Features.Grades.Services
{
    public class GradeService : IGradeService
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IMapper _mapper;

        public GradeService(ISemesterRepository semesterRepository, IGradeRepository gradeRepository, IStudentClassRepository studentClassRepository, ITeachingAssignmentRepository teachingAssignmentRepository, IMapper mapper)
        {
            _gradeRepository = gradeRepository;
            _studentClassRepository = studentClassRepository;
            _teachingAssignmentRepository = teachingAssignmentRepository;
            _semesterRepository = semesterRepository;
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
            if (dto == null || dto.Grades == null || !dto.Grades.Any())
                return false;
            var gradeIds = dto.Grades.Select(g => g.GradeID).ToList();
            var gradeEntities = await _gradeRepository.GetGradesByIdsAsync(gradeIds);
            if (gradeEntities.Count == 0) return false;

            var invalidBatches = gradeEntities
            .Where(g => g.Batch.Status != AppConstants.Status.ACTIVE)
            .Select(g => g.BatchId)
            .Distinct()
            .ToList();
            if (invalidBatches.Any())
            {
                throw new InvalidOperationException(
                    $"Cannot update grades because the following GradeBatch IDs are not in 'Hoạt Động'");
            }


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
        public async Task<List<GradeSummaryEachSubjectNameDto>> GetGradeSummaryEachSubjectByStudentAsync(int studentId, int semesterId)
        {
            var allGrades = await _gradeRepository.GetGradesByStudentAsync(studentId, semesterId);


            allGrades = allGrades.Where(x => x.Batch.SemesterId == semesterId).ToList();

            var groupedGrades = allGrades.GroupBy(g => g.Assignment.Subject.SubjectName);

            var result = new List<GradeSummaryEachSubjectNameDto>();

            foreach (var group in groupedGrades)
            {
                foreach (var grade in group)
                {
                    Console.WriteLine($"GradeId: {grade.GradeId}, SemesterName: {grade.Batch?.Semester?.SemesterName}");
                }

                var subjectName = group.Key;
                var semester1Grades = group
                                        .Where(x => x.Batch?.Semester?.SemesterName == "Học kỳ 1")
                                        .ToList();

                var semester2Grades = group
                    .Where(x => x.Batch?.Semester?.SemesterName == "Học kỳ 2")
                    .ToList();


                double? semester1Average = semester1Grades.Any() ? CalculateSemesterAverage(semester1Grades) : null;
                double? semester2Average = semester2Grades.Any() ? CalculateSemesterAverage(semester2Grades) : null;
                double? yearAverage = (semester1Average != null && semester2Average != null)
                                        ? CalculateYearAverage(semester1Average, semester2Average)
                                        : null;

                result.Add(new GradeSummaryEachSubjectNameDto
                {
                    StudentId = group.First().StudentClass.StudentId,
                    StudentName = group.First().StudentClass.Student.FullName,
                    SubjectName = subjectName,
                    Semester1Average = semester1Average,
                    Semester2Average = semester2Average,
                    YearAverage = yearAverage
                });
            }

            return result;
        }

        public async Task<GradeSummaryDto> GetTotalGradeSummaryByStudentAsync(int studentId, int semesterId)
        {
            var subjectSummaries = await GetGradeSummaryEachSubjectByStudentAsync(studentId, semesterId);

            var semester1Averages = subjectSummaries.Where(x => x.Semester1Average != null).Select(x => x.Semester1Average.Value).ToList();
            var semester2Averages = subjectSummaries.Where(x => x.Semester2Average != null).Select(x => x.Semester2Average.Value).ToList();
            var yearAverages = subjectSummaries.Where(x => x.YearAverage != null).Select(x => x.YearAverage.Value).ToList();

            double? totalSemester1Average = semester1Averages.Any() ? Math.Round(semester1Averages.Average(), 2) : null;
            double? totalSemester2Average = semester2Averages.Any() ? Math.Round(semester2Averages.Average(), 2) : null;
            double? totalYearAverage = yearAverages.Any() ? Math.Round(yearAverages.Average(), 2) : null;

            return new GradeSummaryDto
            {
                StudentId = studentId,
                totalSemester1Average = totalSemester1Average,
                totalSemester2Average = totalSemester2Average,
                totalYearAverage = totalYearAverage,
                gradeSummaryEachSubjectNameDtos = subjectSummaries
            };
        }

        private double? CalculateSemesterAverage(List<Grade> grades)
        {
            var txScores = grades
                .Where(x => x.AssessmentsTypeName.Contains("ĐĐG TX") && double.TryParse(x.Score, out _))
                .Select(x => double.Parse(x.Score))
                .ToList();

            var gkScore = grades.FirstOrDefault(x => x.AssessmentsTypeName == "ĐĐG GK" && double.TryParse(x.Score, out _))?.Score;
            var ckScore = grades.FirstOrDefault(x => x.AssessmentsTypeName == "ĐĐG CK" && double.TryParse(x.Score, out _))?.Score;

            if (!txScores.Any() || gkScore == null || ckScore == null)
                return null;

            double diemGk = double.Parse(gkScore);
            double diemCk = double.Parse(ckScore);

            double tongDiemTx = txScores.Sum();
            double tb = (tongDiemTx + 2 * diemGk + 3 * diemCk) / (txScores.Count + 5);

            return Math.Round(tb, 2);
        }


        private double? CalculateYearAverage(double? semester1Average, double? semester2Average)
        {
            if (semester1Average == null || semester2Average == null)
                return null;

            double tb = (semester1Average.Value + 2 * semester2Average.Value) / 3;
            return Math.Round(tb, 2);
        }
    }
}
