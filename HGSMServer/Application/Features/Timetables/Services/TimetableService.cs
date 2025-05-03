using Application.Features.Timetables.DTOs;
using Application.Features.Timetables.Interfaces;
using AutoMapper;
using ClosedXML.Excel;
using Common.Constants;
using Common.Utils;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Application.Features.Timetables.Services
{
    public class TimetableService : ITimetableService
    {
        private readonly IMapper _mapper;
        private readonly ITimetableUnitOfWork _unitOfWork;

        public TimetableService(ITimetableUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper;
        }
        public async Task<Timetable> CreateTimetableAsync(CreateTimetableDto dto)
        {
            var timetable = _mapper.Map<Timetable>(dto);
            var createdTimetable = await _unitOfWork.Timetables.CreateTimetableAsync(timetable);

            foreach (var detailDto in dto.Details)
            {
                var timetableDetail = _mapper.Map<TimetableDetail>(detailDto);
                timetableDetail.TimetableId = createdTimetable.TimetableId;
                await _unitOfWork.Timetables.AddTimetableDetailAsync(timetableDetail);
                createdTimetable.TimetableDetails.Add(timetableDetail);
            }

            return createdTimetable;
        }

        public async Task<IEnumerable<TimetableDto>> GetTimetableByStudentAsync(int studentId, int semesterId)
        {
            var timetables = await _unitOfWork.Timetables.GetByStudentIdAsync(studentId, semesterId);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
        }

        public async Task<IEnumerable<TimetableDto>> GetTimetableByTeacherAsync(int teacherId)
        {
            var timetables = await _unitOfWork.Timetables.GetByTeacherIdAsync(teacherId);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
        }

        public async Task<IEnumerable<TimetableDto>> GetTimetablesForPrincipalAsync(int timetableId, string? status = null)
        {
            var timetables = await _unitOfWork.Timetables.GetTimetablesForPrincipalAsync(timetableId, status);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
        }

        public async Task<IEnumerable<TimetableListDto>> GetTimetablesBySemesterAsync(int semesterId)
        {
            var timetables = await _unitOfWork.Timetables.GetTimetablesBySemesterAsync(semesterId);
            return _mapper.Map<IEnumerable<TimetableListDto>>(timetables);
        }
        public async Task<TimetableDto> UpdateTimetableInfoAsync(UpdateTimetableInfoDto dto)
        {
            var timetable = await _unitOfWork.Timetables.GetByIdAsync(dto.TimetableId);
            if (timetable == null)
            {
                throw new KeyNotFoundException($"Timetable with ID {dto.TimetableId} not found.");
            }

            timetable.SemesterId = dto.SemesterId;
            timetable.EffectiveDate = dto.EffectiveDate;
            timetable.EndDate = dto.EndDate;
            timetable.Status = dto.Status;

            await _unitOfWork.Timetables.UpdateTimetableAsync(timetable);

            var updatedTimetable = await _unitOfWork.Timetables.GetByIdAsync(dto.TimetableId);
            return _mapper.Map<TimetableDto>(updatedTimetable);
        }
        public async Task<bool> UpdateMultipleDetailsAsync(UpdateTimetableDetailsDto dto)
        {
            var timetable = await _unitOfWork.Timetables.GetByIdAsync(dto.TimetableId);
            if (timetable == null)
            {
                throw new KeyNotFoundException($"Timetable with ID {dto.TimetableId} not found.");
            }

            var existingDetails = timetable.TimetableDetails.ToDictionary(td => td.TimetableDetailId);

            var detailsToUpdate = new List<TimetableDetail>();
            foreach (var detailDto in dto.Details)
            {
                if (!existingDetails.TryGetValue(detailDto.TimetableDetailId, out var detail))
                {
                    throw new KeyNotFoundException($"TimetableDetail with ID {detailDto.TimetableDetailId} not found in Timetable {dto.TimetableId}.");
                }

                detail.ClassId = detailDto.ClassId;
                detail.SubjectId = detailDto.SubjectId;
                detail.TeacherId = detailDto.TeacherId;
                detail.DayOfWeek = detailDto.DayOfWeek;
                detail.PeriodId = detailDto.PeriodId;

                //if (await _repository.IsConflictAsync(detail))
                //{
                //    throw new InvalidOperationException($"Conflict detected for timetable detail ID {detail.TimetableDetailId} on {detail.DayOfWeek} at period {detail.PeriodId}.");
                //}

                detailsToUpdate.Add(detail);
            }

            return await _unitOfWork.Timetables.UpdateMultipleDetailsAsync(detailsToUpdate);
        }

        public async Task<bool> DeleteDetailAsync(int detailId)
        {
            return await _unitOfWork.Timetables.DeleteDetailAsync(detailId);
        }

        //public async Task<bool> IsConflictAsync(TimetableDetailDto detailDto)
        //{
        //    var detail = _mapper.Map<TimetableDetail>(detailDto);
        //    return await _repository.IsConflictAsync(detail);
        //}

        // using Microsoft.AspNetCore.Http; // Thêm ở đầu file nếu chưa có
        // using Common.Utils; // Namespace chứa ExcelImportHelper
        // using System.Globalization; // Để parse DayOfWeek


        public async Task CreateDetailAsync(CreateTimetableDetailRequest request)
        {
            //var conflict = await _timetableDetailRepository.IsConflictAsync(request.ClassId, request.DayOfWeek, request.PeriodId);
            //if (conflict)
            //{
            //    throw new Exception("Lớp này đã có tiết học vào thời điểm này.");
            //}

            var detail = new TimetableDetail
            {
                TimetableId = request.TimetableId,
                ClassId = request.ClassId,
                SubjectId = request.SubjectId,
                TeacherId = request.TeacherId,
                DayOfWeek = request.DayOfWeek,
                PeriodId = request.PeriodId
            };

            await _unitOfWork.TimetableDetails.AddAsync(detail);
            await _unitOfWork.TimetableDetails.SaveChangesAsync();
        }

        public async Task<Timetable> ImportTimetableAsync(IFormFile file, int semesterId, DateOnly effectiveDate)
        {
            // _logger?.LogInformation("Starting timetable import for Semester {SemesterId}, EffectiveDate {EffectiveDate}", semesterId, effectiveDate);

            var detailsToCreate = new List<TimetableDetail>();
            var errors = new StringBuilder();

            // Cache để giảm truy vấn DB
            var classIdCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase); // Không phân biệt hoa thường cho tên lớp
            var subjectIdCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase); // Không phân biệt hoa thường cho tên môn
            var periodIdCache = new Dictionary<(string, byte), int>(); // Key: (Tên Tiết, Ca Học)
            var teacherIdCache = new Dictionary<(int, int, int), int?>(); // Key: (SubjectId, ClassId, SemesterId), Value: TeacherId?

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            if (worksheet == null)
            {
                throw new ArgumentException("File Excel không hợp lệ hoặc không tìm thấy sheet đầu tiên.");
            }

            // Xác định cấu trúc file Excel
            int headerRow = 6;
            int firstDataRow = 7;
            int dayOfWeekCol = 2;
            int shiftCol = 3;
            int periodCol = 4;
            int firstClassCol = 5;

            var lastRowUsed = worksheet.LastRowUsed()?.RowNumber() ?? 0;
            var lastColUsed = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;

            if (lastRowUsed < firstDataRow || lastColUsed < firstClassCol)
            {
                throw new ArgumentException("File Excel không có dữ liệu hoặc cấu trúc không đúng (thiếu header lớp hoặc dòng dữ liệu).");
            }

            // Đọc Header Lớp và Cache ClassId
            var classHeaders = new Dictionary<int, string>();
            for (int col = firstClassCol; col <= lastColUsed; col++)
            {
                var className = worksheet.Cell(headerRow, col).GetString()?.Trim();
                if (!string.IsNullOrWhiteSpace(className))
                {
                    classHeaders.Add(col, className);
                    if (!classIdCache.ContainsKey(className))
                    {
                        var classEntity = await _unitOfWork.Classes.GetClassByNameAsync(className);
                        if (classEntity == null)
                        {
                            errors.AppendLine($"Không tìm thấy lớp học với tên '{className}'.");
                        }
                        else
                        {
                            classIdCache[className] = classEntity.ClassId;
                        }
                    }
                }
            }
            if (errors.Length > 0) throw new KeyNotFoundException($"Lỗi tìm thông tin lớp học:\n{errors}");


            // Duyệt qua các dòng dữ liệu TKB
            string currentDayOfWeek = string.Empty;
            string currentShiftName = string.Empty;
            byte currentShiftValue = 0;

            for (int row = firstDataRow; row <= lastRowUsed; row++)
            {
                // Xác định Ngày (Thứ)
                var dayCell = worksheet.Cell(row, dayOfWeekCol).GetString()?.Trim();
                if (!string.IsNullOrWhiteSpace(dayCell))
                {
                    currentDayOfWeek = dayCell;
                    currentShiftName = string.Empty;
                    currentShiftValue = 0;
                }
                if (string.IsNullOrWhiteSpace(currentDayOfWeek)) continue;

                // Xác định Buổi (Sáng/Chiều)
                if (string.IsNullOrWhiteSpace(currentShiftName))
                {
                    currentShiftName = worksheet.Cell(row, shiftCol).GetString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(currentShiftName))
                    {
                        if (currentShiftName.Equals("Sáng", StringComparison.OrdinalIgnoreCase)) currentShiftValue = AppConstants.Shift.MORNING;
                        else if (currentShiftName.Equals("Chiều", StringComparison.OrdinalIgnoreCase)) currentShiftValue = AppConstants.Shift.AFTERNOON;
                        // else if (currentShiftName.Equals("Tối", ...)) currentShiftValue = 3; // Thêm nếu cần
                        else { errors.AppendLine($"Giá trị Buổi không hợp lệ '{currentShiftName}' tại dòng {row}."); currentShiftValue = 0; }
                    }
                    else currentShiftValue = 0;
                }
                if (currentShiftValue == 0) continue; // Bỏ qua nếu buổi không hợp lệ


                // Xác định Tiết học (PeriodId)
                var periodCell = worksheet.Cell(row, periodCol).GetString()?.Trim();
                if (!int.TryParse(periodCell, out int periodNumber) || periodNumber <= 0) continue;
                string periodName = $"Tiết {periodNumber}";
                var periodKey = (periodName, currentShiftValue);
                int periodId;
                if (periodIdCache.TryGetValue(periodKey, out int cachedPeriodId)) periodId = cachedPeriodId;
                else
                {
                    var periodEntity = await _unitOfWork.Periods.GetByPeriodNameAndShiftAsync(periodName, currentShiftValue);
                    if (periodEntity == null) { errors.AppendLine($"Không tìm thấy thông tin tiết học '{periodName}' cho buổi '{currentShiftName}' (Shift={currentShiftValue}). Dòng {row}"); continue; }
                    periodId = periodEntity.PeriodId;
                    periodIdCache[periodKey] = periodId;
                }


                // Duyệt qua các cột lớp học trong dòng
                foreach (var kvp in classHeaders)
                {
                    int classCol = kvp.Key;
                    string className = kvp.Value;
                    int classId = classIdCache[className];

                    var subjectName = worksheet.Cell(row, classCol).GetString()?.Trim();
                    if (string.IsNullOrWhiteSpace(subjectName)) continue; // Bỏ qua ô môn học trống

                    // Xác định Môn học (SubjectId)
                    int subjectId;
                    if (subjectIdCache.TryGetValue(subjectName, out int cachedSubjectId)) subjectId = cachedSubjectId;
                    else
                    {
                        var subjectEntity = await _unitOfWork.Subjects.GetByNameAsync(subjectName);
                        if (subjectEntity == null) { errors.AppendLine($"Không tìm thấy môn học '{subjectName}'. Lớp: {className}, {currentDayOfWeek}, {currentShiftName}, {periodName}. Dòng {row}"); continue; }
                        subjectId = subjectEntity.SubjectId;
                        subjectIdCache[subjectName] = subjectId;
                    }


                    // --- Xác định Giáo viên (TeacherId) bằng phương thức của bạn ---
                    int? teacherId = null; // Khai báo biến để lưu TeacherId (nullable)
                    var teacherKey = (subjectId, classId, semesterId); // Key cho cache

                    // Kiểm tra cache TeacherId (vẫn dùng int?)
                    if (teacherIdCache.TryGetValue(teacherKey, out int? cachedTeacherId))
                    {
                        teacherId = cachedTeacherId; // Lấy TeacherId? từ cache
                    }
                    else
                    {
                        // Gọi phương thức GetAssignmentByClassSubjectTeacherAsync của bạn
                        var assignment = await _unitOfWork.TeachingAssignments.GetAssignmentByClassSubjectTeacherAsync(classId, subjectId, semesterId); // Gọi đúng tên phương thức

                        if (assignment != null)
                        {
                            teacherId = assignment.TeacherId; // Lấy TeacherId từ đối tượng trả về
                        }
                        // else: assignment là null, teacherId vẫn là null

                        // Lưu kết quả (TeacherId? hoặc null) vào cache
                        teacherIdCache[teacherKey] = teacherId;
                    }

                    // Kiểm tra xem có tìm được TeacherId không
                    if (teacherId == null)
                    {
                        // Không tìm thấy phân công -> ghi lỗi và bỏ qua tiết học này
                        errors.AppendLine($"Không tìm thấy phân công giáo viên cho môn '{subjectName}' (ID:{subjectId}), lớp '{className}' (ID:{classId}), học kỳ ID:{semesterId}. {currentDayOfWeek}, {currentShiftName}, {periodName}. Dòng {row}");
                        continue;
                    }

                    // Tạo TimetableDetail nếu mọi thứ OK
                    var detail = new TimetableDetail
                    {
                        ClassId = classId,
                        SubjectId = subjectId,
                        TeacherId = teacherId.Value, // Đảm bảo teacherId không null ở đây
                        DayOfWeek = currentDayOfWeek,
                        PeriodId = periodId
                        // TimetableId sẽ gán sau
                    };
                    detailsToCreate.Add(detail);

                } // Kết thúc foreach duyệt lớp
            } // Kết thúc for duyệt dòng


            // Kiểm tra lỗi tổng hợp và Lưu dữ liệu
            if (errors.Length > 0)
            {
                // _logger?.LogError("Errors during timetable import: {Errors}", errors.ToString());
                throw new ArgumentException($"Có lỗi trong dữ liệu file Excel:\n{errors}");
            }
            if (!detailsToCreate.Any())
            {
                throw new ArgumentException("Không có dữ liệu thời khóa biểu hợp lệ nào được tạo từ file Excel.");
            }

            // Tạo Timetable chính
            var timetable = new Timetable
            {
                SemesterId = semesterId,
                EffectiveDate = effectiveDate,
                Status = AppConstants.Status.ACTIVE // Hoặc PENDING
            };
            var createdTimetable = await _unitOfWork.Timetables.CreateTimetableAsync(timetable);
            if (createdTimetable == null || createdTimetable.TimetableId <= 0)
            {
                // _logger?.LogError("Failed to create main timetable record.");
                throw new Exception("Không thể tạo bản ghi thời khóa biểu chính.");
            }

            // Gán TimetableId và Lưu Details
            foreach (var detail in detailsToCreate)
            {
                detail.TimetableId = createdTimetable.TimetableId;
            }
            // await _unitOfWork.TimetableDetails.AddRangeAsync(detailsToCreate); // Ưu tiên nếu có
            foreach (var detail in detailsToCreate) // Hoặc dùng AddAsync
            {
                await _unitOfWork.TimetableDetails.AddAsync(detail); // Giả định có AddAsync
            }
            await _unitOfWork.SaveChangesAsync(); // Lưu tất cả thay đổi

            // _logger?.LogInformation("Successfully imported timetable {TimetableId} with {DetailCount} details.", createdTimetable.TimetableId, detailsToCreate.Count);
            return createdTimetable; // Trả về Timetable chính đã tạo
        }
    }
}