using Application.Features.Timetables.DTOs;
using Application.Features.Timetables.Interfaces;
using AutoMapper;
using Common.Utils;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Timetables.Services
{
    public class TimetableService : ITimetableService
    {
        private readonly ITimetableRepository _repository;
        private readonly IMapper _mapper;

        public TimetableService(ITimetableRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Timetable> CreateTimetableAsync(CreateTimetableDto dto)
        {
            var timetable = _mapper.Map<Timetable>(dto);
            var createdTimetable = await _repository.CreateTimetableAsync(timetable);

            foreach (var detailDto in dto.Details)
            {
                var timetableDetail = _mapper.Map<TimetableDetail>(detailDto);
                timetableDetail.TimetableId = createdTimetable.TimetableId;
                await _repository.AddTimetableDetailAsync(timetableDetail);
                createdTimetable.TimetableDetails.Add(timetableDetail);
            }

            return createdTimetable;
        }

        public async Task<IEnumerable<TimetableDto>> GetTimetableByStudentAsync(int studentId, int semesterId)
        {
            var timetables = await _repository.GetByStudentIdAsync(studentId, semesterId);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
        }

        public async Task<IEnumerable<TimetableDto>> GetTimetableByTeacherAsync(int teacherId)
        {
            var timetables = await _repository.GetByTeacherIdAsync(teacherId);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
        }

        public async Task<IEnumerable<TimetableDto>> GetTimetablesForPrincipalAsync(int timetableId, string? status = null)
        {
            var timetables = await _repository.GetTimetablesForPrincipalAsync(timetableId, status);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
        }

        public async Task<IEnumerable<TimetableListDto>> GetTimetablesBySemesterAsync(int semesterId)
        {
            var timetables = await _repository.GetTimetablesBySemesterAsync(semesterId);
            return _mapper.Map<IEnumerable<TimetableListDto>>(timetables);
        }
        public async Task<TimetableDto> UpdateTimetableInfoAsync(UpdateTimetableInfoDto dto)
        {
            var timetable = await _repository.GetByIdAsync(dto.TimetableId);
            if (timetable == null)
            {
                throw new KeyNotFoundException($"Timetable with ID {dto.TimetableId} not found.");
            }

            timetable.SemesterId = dto.SemesterId;
            timetable.EffectiveDate = dto.EffectiveDate;
            timetable.Status = dto.Status;

            await _repository.UpdateTimetableAsync(timetable);

            var updatedTimetable = await _repository.GetByIdAsync(dto.TimetableId);
            return _mapper.Map<TimetableDto>(updatedTimetable);
        }
        public async Task<bool> UpdateMultipleDetailsAsync(UpdateTimetableDetailsDto dto)
        {
            var timetable = await _repository.GetByIdAsync(dto.TimetableId);
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

                if (await _repository.IsConflictAsync(detail))
                {
                    throw new InvalidOperationException($"Conflict detected for timetable detail ID {detail.TimetableDetailId} on {detail.DayOfWeek} at period {detail.PeriodId}.");
                }

                detailsToUpdate.Add(detail);
            }

            return await _repository.UpdateMultipleDetailsAsync(detailsToUpdate);
        }

        public async Task<bool> DeleteDetailAsync(int detailId)
        {
            return await _repository.DeleteDetailAsync(detailId);
        }

        public async Task<bool> IsConflictAsync(TimetableDetailDto detailDto)
        {
            var detail = _mapper.Map<TimetableDetail>(detailDto);
            return await _repository.IsConflictAsync(detail);
        }

        // using Microsoft.AspNetCore.Http; // Thêm ở đầu file nếu chưa có
        // using Common.Utils; // Namespace chứa ExcelImportHelper
        // using System.Globalization; // Để parse DayOfWeek

        public async Task<int> ImportTimetableFromExcelAsync(IFormFile file, int semesterId, DateOnly effectiveDate)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded or file is empty.");
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid file format. Only .xlsx files are allowed.");
            }

            var excelData = ExcelImportHelper.ReadExcelData(file);
            if (excelData == null || !excelData.Any())
            {
                throw new ArgumentException("Excel file is empty or could not be read.");
            }

            var existingTimetable = await _repository.FindBySemesterAndEffectiveDateAsync(semesterId, effectiveDate);
            if (existingTimetable != null)
            {
                throw new InvalidOperationException($"A timetable for semester {semesterId} with effective date {effectiveDate:dd/MM/yyyy} already exists (TimetableId: {existingTimetable.TimetableId}).");
            }

            var newTimetable = new Timetable
            {
                SemesterId = semesterId,
                EffectiveDate = effectiveDate,
                Status = "PendingImport", // Trạng thái ban đầu khi import
                TimetableDetails = new List<TimetableDetail>()
            };

            var createdTimetable = await _repository.CreateTimetableAsync(newTimetable);
            if (createdTimetable == null || createdTimetable.TimetableId <= 0)
            {
                throw new Exception("Failed to create the base timetable record.");
            }


            int importedCount = 0;
            var classes = await _repository.GetAllClassesAsync();
            var subjects = await _repository.GetAllSubjectsAsync();
            var periods = await _repository.GetAllPeriodsAsync();

            if (!classes.Any() || !subjects.Any() || !periods.Any())
            {
                // Có thể throw lỗi hoặc trả về 0 tùy logic
                throw new KeyNotFoundException("Essential data missing: Classes, Subjects, or Periods not found in the database.");
            }

            var headerRow = excelData.First();
            var classColumns = headerRow.Keys
                                     .Where(k => !string.IsNullOrWhiteSpace(k) && (k.Contains("A") || k.Contains("B") || k.Contains("C")) && !k.Equals("Buổi") && !k.Equals("Tiết") && !k.Equals("Thứ"))
                                     .Distinct(StringComparer.OrdinalIgnoreCase)
                                     .ToList();

            if (!classColumns.Any())
            {
                throw new ArgumentException("Could not identify class columns in the Excel header.");
            }


            foreach (var row in excelData.Skip(1))
            {
                string dayOfWeekString = row.GetValueOrDefault("Thứ")?.Trim();
                string sessionExcel = row.GetValueOrDefault("Buổi")?.Trim();
                string periodNumberString = row.GetValueOrDefault("Tiết")?.Trim();

                if (string.IsNullOrWhiteSpace(dayOfWeekString) || string.IsNullOrWhiteSpace(sessionExcel) || !int.TryParse(periodNumberString, out int periodNumberExcel))
                {
                    // _logger.LogWarning(...);
                    continue;
                }

                // Xác thực giá trị DayOfWeek đọc từ Excel
                bool isValidDay = dayOfWeekString switch
                {
                    "2" or "3" or "4" or "5" or "6" or "7" => true,
                    _ => false
                };

                if (!isValidDay)
                {
                    // _logger.LogWarning($"Skipping row due to invalid DayOfWeek string: '{dayOfWeekString}'");
                    continue; // Bỏ qua nếu không phải "2" đến "7"
                }



                byte expectedShift;
                if (sessionExcel.Equals("Sáng", StringComparison.OrdinalIgnoreCase))
                {
                    expectedShift = 1;
                }
                else if (sessionExcel.Equals("Chiều", StringComparison.OrdinalIgnoreCase))
                {
                    expectedShift = 2;
                }
                else
                {
                    // _logger.LogWarning($"Skipping row due to unknown session: '{sessionExcel}'");
                    continue;
                }

                var period = periods.FirstOrDefault(p =>
                                        p.Shift == expectedShift &&
                                        !string.IsNullOrEmpty(p.PeriodName) &&
                                        p.PeriodName.Contains(periodNumberExcel.ToString()));

                if (period == null)
                {
                    continue;
                }

                foreach (var className in classColumns)
                {
                    string subjectName = row.GetValueOrDefault(className)?.Trim();
                    if (string.IsNullOrWhiteSpace(subjectName))
                    {
                        continue;
                    }

                    var classInfo = classes.FirstOrDefault(c => c.ClassName.Equals(className, StringComparison.OrdinalIgnoreCase));
                    if (classInfo == null)
                    {
                        // _logger.LogWarning($"Class not found for name '{className}' in row. Skipping detail.");
                        continue;
                    }

                    var subjectInfo = subjects.FirstOrDefault(s => s.SubjectName.Equals(subjectName, StringComparison.OrdinalIgnoreCase));
                    if (subjectInfo == null)
                    {
                        // _logger.LogWarning($"Subject not found for name '{subjectName}' in row for class {className}. Skipping detail.");
                        continue;
                    }


                    var detail = new TimetableDetail
                    {
                        TimetableId = createdTimetable.TimetableId,
                        ClassId = classInfo.ClassId,
                        SubjectId = subjectInfo.SubjectId,
                        TeacherId = 1,
                        DayOfWeek = dayOfWeekString, // Sử dụng dayOfWeek đã được chuyển đổi
                        PeriodId = period.PeriodId
                    };

                    // Cân nhắc: Kiểm tra conflict tại đây trước khi thêm?
                    // if (await _repository.IsConflictAsync(detail)) { ... }

                    try
                    {
                        await _repository.AddTimetableDetailAsync(detail);
                        importedCount++;
                    }
                    catch (Exception detailEx)
                    {
                        // _logger.LogError(detailEx, $"Failed to add timetable detail for Class {className}, Subject {subjectName}, Day {dayOfWeek}, Period {period.PeriodId}.");
                        // Quyết định: Dừng import hay tiếp tục với các dòng khác?
                        // Nếu muốn dừng toàn bộ: throw new Exception("Failed to add one or more timetable details. Import aborted.", detailEx);
                        // Nếu muốn tiếp tục, chỉ cần log lỗi và đi tiếp vòng lặp.
                        Console.WriteLine($"Error adding detail: {detailEx.Message}"); // Tạm thời ghi ra console
                    }
                }
            }

            // Cập nhật trạng thái cuối cùng cho Timetable nếu cần
            // Ví dụ: Nếu import thành công và muốn kích hoạt ngay
            // createdTimetable.Status = "Active"; // Chú ý Trigger có thể chuyển các TKB khác thành Inactive
            // await _repository.UpdateTimetableAsync(createdTimetable);

            if (importedCount == 0 && excelData.Count > 1)
            {
                // Có thể throw lỗi nếu file có dữ liệu nhưng không import được gì
                // throw new InvalidOperationException("No valid timetable details could be imported from the provided file.");
            }

            return importedCount;
        }
    }

}