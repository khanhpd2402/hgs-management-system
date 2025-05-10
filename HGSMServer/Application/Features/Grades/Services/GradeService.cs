using AutoMapper;
using Infrastructure.Repositories.Interfaces;
using Application.Features.Grades.Interfaces;
using Application.Features.Grades.DTOs;
using Domain.Models;
using Infrastructure.Repositories.Implementtations;
using Common.Constants;
using ClosedXML.Excel;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Grades.Services
{
    public class GradeService : IGradeService
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IGradeBatchRepository _gradeBatchRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IMapper _mapper;

        public GradeService(ISemesterRepository semesterRepository, IGradeRepository gradeRepository, IStudentClassRepository studentClassRepository, ITeachingAssignmentRepository teachingAssignmentRepository, IGradeBatchRepository gradeBatchRepository, IMapper mapper)
        {
            _gradeRepository = gradeRepository;
            _gradeBatchRepository = gradeBatchRepository;
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

        public async Task<ImportGradesResultDto> ImportGradesFromExcelAsync(int classId, int subjectId, int semesterId, int batchId, IFormFile file)
        {
            var result = new ImportGradesResultDto();
            int updatedCount = 0;
            var gradesToUpdate = new List<Grade>();

            var gradeBatch = await _gradeBatchRepository.GetByIdAsync(batchId);
            if (gradeBatch == null)
            {
                result.IsSuccess = false;
                result.Message = $"Đợt nhập điểm với ID {batchId} không tìm thấy.";
                result.Errors.Add(result.Message);
                return result;
            }
            if (gradeBatch.SemesterId != semesterId)
            {
                result.IsSuccess = false;
                result.Message = $"Học kỳ của đợt nhập điểm ({gradeBatch.SemesterId}) không khớp với học kỳ được chọn ({semesterId}).";
                result.Errors.Add(result.Message);
                return result;
            }

            var studentGradeImports = new List<StudentGradeImportDto>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            result.IsSuccess = false; result.Message = "File Excel trống hoặc không tìm thấy worksheet."; result.Errors.Add(result.Message); return result;
                        }

                        const int mainHeaderRowIdx = 4; const int subHeaderRowIdx = 5; const int dataStartRowIdx = 6;
                        var lastRowUsed = worksheet.LastRowUsed()?.RowNumber() ?? 0;
                        var lastColUsed = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;

                        if (lastRowUsed < dataStartRowIdx || lastColUsed == 0)
                        {
                            result.IsSuccess = true; result.Message = "File Excel không có dữ liệu học sinh hoặc cấu trúc không đúng."; return result;
                        }

                        Dictionary<int, string> columnToMainHeaderMap = new Dictionary<int, string>();
                        string currentMainHeaderValue = "";
                        var mainHeaderRowObj = worksheet.Row(mainHeaderRowIdx);
                        for (int col = 1; col <= lastColUsed; col++)
                        {
                            var cell = mainHeaderRowObj.Cell(col);
                            string cellText = cell.GetString()?.Trim() ?? "";
                            if (cell.IsMerged()) { var mergedRange = cell.MergedRange(); cellText = mergedRange.FirstCell().GetString()?.Trim() ?? ""; }
                            if (!string.IsNullOrEmpty(cellText)) currentMainHeaderValue = cellText;
                            columnToMainHeaderMap[col] = currentMainHeaderValue;
                        }

                        List<string> subHeadersList = new List<string>();
                        var subHeaderRowObj = worksheet.Row(subHeaderRowIdx);
                        for (int col = 1; col <= lastColUsed; col++) { subHeadersList.Add(subHeaderRowObj.Cell(col).GetString()?.Trim() ?? ""); }

                        int studentIdCol = -1; var txColumnDetails = new List<Tuple<int, string>>();
                        int gkScoreCol = -1, ckScoreCol = -1;

                        for (int col = 1; col <= lastColUsed; col++)
                        {
                            string mainHeader = columnToMainHeaderMap.ContainsKey(col) ? columnToMainHeaderMap[col] : string.Empty;
                            string subHeader = (col - 1 < subHeadersList.Count) ? subHeadersList[col - 1] : string.Empty;

                            if (mainHeader.Equals("Mã học sinh", StringComparison.OrdinalIgnoreCase)) studentIdCol = col;
                            else if (mainHeader.Equals("ĐĐG TX", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(subHeader))
                            {
                                if (subHeader.StartsWith("TX", StringComparison.OrdinalIgnoreCase))
                                {
                                    string assessmentName = $"ĐĐG {subHeader}";
                                    txColumnDetails.Add(Tuple.Create(col, assessmentName));
                                }
                            }
                            else if (mainHeader.Equals("ĐĐG GK", StringComparison.OrdinalIgnoreCase)) gkScoreCol = col;
                            else if (mainHeader.Equals("ĐĐG CK", StringComparison.OrdinalIgnoreCase)) ckScoreCol = col;
                        }

                        if (studentIdCol == -1)
                        {
                            result.IsSuccess = false; result.Message = "Lỗi cấu trúc file: Không tìm thấy cột 'Mã học sinh'."; result.Errors.Add(result.Message); return result;
                        }

                        for (int rowIdx = dataStartRowIdx; rowIdx <= lastRowUsed; rowIdx++)
                        {
                            var currentRow = worksheet.Row(rowIdx);
                            int studentIdValue;
                            var studentIdCell = currentRow.Cell(studentIdCol);
                            if (studentIdCell.DataType == XLDataType.Number && studentIdCell.TryGetValue(out double studentIdDouble)) studentIdValue = (int)studentIdDouble;
                            else if (studentIdCell.DataType == XLDataType.Text && int.TryParse(studentIdCell.GetString()?.Trim(), out int parsedInt)) studentIdValue = parsedInt;
                            else { var cellValueForError = studentIdCell.GetString()?.Trim(); result.Errors.Add($"Lỗi định dạng Mã học sinh (StudentId) ở dòng {rowIdx}: '{cellValueForError}'. Mong đợi số nguyên."); continue; }
                            if (studentIdValue <= 0) { result.Errors.Add($"Mã học sinh (StudentId) không hợp lệ ở dòng {rowIdx}: '{studentIdValue}'."); continue; }

                            var studentImport = new StudentGradeImportDto { StudentId = studentIdValue };
                            foreach (var txColDetail in txColumnDetails)
                            {
                                var score = currentRow.Cell(txColDetail.Item1).GetString()?.Trim();
                                if (!string.IsNullOrWhiteSpace(score)) studentImport.GradeEntries.Add(new GradeEntryDto { AssessmentsTypeName = txColDetail.Item2, Score = score });
                            }
                            if (gkScoreCol != -1) { var score = currentRow.Cell(gkScoreCol).GetString()?.Trim(); if (!string.IsNullOrWhiteSpace(score)) studentImport.GradeEntries.Add(new GradeEntryDto { AssessmentsTypeName = "ĐĐG GK", Score = score }); }
                            if (ckScoreCol != -1) { var score = currentRow.Cell(ckScoreCol).GetString()?.Trim(); if (!string.IsNullOrWhiteSpace(score)) studentImport.GradeEntries.Add(new GradeEntryDto { AssessmentsTypeName = "ĐĐG CK", Score = score }); }


                            if (studentImport.GradeEntries.Any()) 
                                studentGradeImports.Add(studentImport);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false; result.Message = "Có lỗi xảy ra trong quá trình đọc file Excel."; result.Errors.Add($"Chi tiết: {ex.Message} (StackTrace: {ex.StackTrace})"); return result;
            }

            if (!studentGradeImports.Any() && !result.Errors.Any())
            {
                result.IsSuccess = true; result.Message = "Không tìm thấy dữ liệu học sinh trong file Excel để xử lý."; return result;
            }
            if (result.Errors.Any() && !result.IsSuccess) return result;

            var teachingAssignment = await _teachingAssignmentRepository.GetAssignmentByClassSubjectTeacherAsync(classId, subjectId, semesterId);
            if (teachingAssignment == null)
            {
                result.IsSuccess = false; result.Message = $"Không tìm thấy phân công giảng dạy cho Lớp ID: {classId}, Môn học ID: {subjectId}, Học kỳ ID: {semesterId}."; result.Errors.Add(result.Message); return result;
            }
            int assignmentId = teachingAssignment.AssignmentId;

            foreach (var studentImport in studentGradeImports)
            {
                var studentClass = await _studentClassRepository.GetStudentClassByStudentAndClassIdAsync(studentImport.StudentId, classId);
                if (studentClass == null)
                {
                    result.Errors.Add($"Học sinh với ID '{studentImport.StudentId}' (Lớp ID: {classId}) không tìm thấy. Dữ liệu cho học sinh này sẽ được bỏ qua."); continue;
                }
                int studentClassId = studentClass.Id;

                foreach (var gradeEntry in studentImport.GradeEntries)
                {
                    var existingGrade = await _gradeRepository.GetGradeAsync(studentClassId, assignmentId, batchId, gradeEntry.AssessmentsTypeName);
                    if (existingGrade == null)
                    {
                        result.Errors.Add($"Lỗi dữ liệu: Không tìm thấy bản ghi điểm có sẵn cho HS ID '{studentImport.StudentId}', Loại điểm: '{gradeEntry.AssessmentsTypeName}', Đợt ID: {batchId}."); continue;
                    }

                    if (existingGrade.Score != gradeEntry.Score) // Chỉ cập nhật nếu điểm số thay đổi
                    {
                        existingGrade.Score = gradeEntry.Score;
                        if (!gradesToUpdate.Any(g => g.GradeId == existingGrade.GradeId)) gradesToUpdate.Add(existingGrade);
                    }
                }
            }

            if (gradesToUpdate.Any())
            {
                var distinctGradesToUpdate = gradesToUpdate.DistinctBy(g => g.GradeId).ToList();
                try
                {
                    await _gradeRepository.UpdateMultipleGradesAsync(distinctGradesToUpdate);
                    updatedCount = distinctGradesToUpdate.Count;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false; result.Message = "Có lỗi xảy ra trong quá trình cập nhật điểm vào cơ sở dữ liệu."; result.Errors.Add($"Chi tiết: {ex.Message}"); return result;
                }
            }

            bool hasCriticalDataErrors = result.Errors.Any(e => e.StartsWith("Lỗi dữ liệu:"));
            if (hasCriticalDataErrors)
            {
                result.IsSuccess = false; result.Message = $"Quá trình cập nhật điểm gặp lỗi về dữ liệu. {updatedCount} mục điểm có thể đã được cập nhật. Vui lòng kiểm tra danh sách lỗi.";
            }
            else if (updatedCount > 0)
            {
                result.IsSuccess = true; result.Message = $"Cập nhật điểm thành công. Đã cập nhật thông tin cho {updatedCount} mục điểm.";
                if (result.Errors.Any()) result.Message += " Một số mục có cảnh báo, vui lòng kiểm tra danh sách.";
            }
            else if (!result.Errors.Any())
            {
                result.IsSuccess = true; result.Message = "Không có thay đổi nào về điểm được phát hiện trong file so với dữ liệu hiện tại.";
            }
            else
            {
                result.IsSuccess = true; result.Message = "Không có điểm nào được cập nhật. Vui lòng kiểm tra danh sách cảnh báo/lỗi.";
            }
            result.UpdatedRecords = updatedCount;
            return result;
        }

    }
}
