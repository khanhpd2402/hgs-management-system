using Application.Features.Timetables.DTOs;
using Application.Features.Timetables.Interfaces;
using AutoMapper;
using ClosedXML.Excel;
using Common.Constants;
using Domain.Models;
using Infrastructure.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Http;
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

                detailsToUpdate.Add(detail);
            }

            return await _unitOfWork.Timetables.UpdateMultipleDetailsAsync(detailsToUpdate);
        }

        public async Task<bool> DeleteDetailAsync(int detailId)
        {
            return await _unitOfWork.Timetables.DeleteDetailAsync(detailId);
        }

        public async Task CreateDetailAsync(CreateTimetableDetailRequest request)
        {
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

                // Xác định Tiết học
                var periodCell = worksheet.Cell(row, periodCol).GetString()?.Trim();
                if (!int.TryParse(periodCell, out int periodNumber) || periodNumber <= 0) continue;

                // Nếu là tiết 1 → phải kiểm tra lại shift từ file Excel
                if (periodNumber == 1)
                {
                    var shiftCell = worksheet.Cell(row, shiftCol).GetString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(shiftCell))
                    {
                        currentShiftName = shiftCell;
                        if (shiftCell.Equals("Sáng", StringComparison.OrdinalIgnoreCase))
                            currentShiftValue = AppConstants.Shift.MORNING;
                        else if (shiftCell.Equals("Chiều", StringComparison.OrdinalIgnoreCase))
                            currentShiftValue = AppConstants.Shift.AFTERNOON;
                        else if (shiftCell.Equals("Tối", StringComparison.OrdinalIgnoreCase))
                            continue;
                        else
                        {
                            errors.AppendLine($"Giá trị Buổi không hợp lệ '{shiftCell}' tại dòng {row}.");
                            currentShiftValue = 0;
                        }
                    }
                    else
                    {
                        errors.AppendLine($"Thiếu thông tin Buổi tại dòng {row} (Tiết 1).");
                        currentShiftValue = 0;
                    }
                }

                // Nếu không xác định được buổi → bỏ qua dòng
                if (currentShiftValue == 0) continue;
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


                    int? teacherId = null; // Khai báo biến để lưu TeacherId (nullable)
                    var teacherKey = (subjectId, classId, semesterId); // Key cho cache

                    // Kiểm tra cache TeacherId (vẫn dùng int?)
                    if (teacherIdCache.TryGetValue(teacherKey, out int? cachedTeacherId))
                    {
                        teacherId = cachedTeacherId; // Lấy TeacherId? từ cache
                    }
                    else
                    {
                        // Gọi phương thức GetAssignmentByClassSubjectTeacherAsync
                        var assignment = await _unitOfWork.TeachingAssignments.GetAssignmentByClassSubjectTeacherAsync(classId, subjectId, semesterId); // Gọi đúng tên phương thức

                        if (assignment != null)
                        {
                            teacherId = assignment.TeacherId; // Lấy TeacherId từ đối tượng trả về
                        }

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
                        TeacherId = teacherId.Value,
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
                Status = AppConstants.Status.PENDING
            };
            var createdTimetable = await _unitOfWork.Timetables.CreateTimetableAsync(timetable);
            if (createdTimetable == null || createdTimetable.TimetableId <= 0)
            {
                throw new Exception("Không thể tạo bản ghi thời khóa biểu chính.");
            }

            // Gán TimetableId và Lưu Details
            foreach (var detail in detailsToCreate)
            {
                detail.TimetableId = createdTimetable.TimetableId;
            }
            foreach (var detail in detailsToCreate)
            {
                await _unitOfWork.TimetableDetails.AddAsync(detail);
            }
            await _unitOfWork.SaveChangesAsync(); // Lưu tất cả thay đổi

            return createdTimetable; // Trả về Timetable chính đã tạo
        }
    }
}