//using Application.Features.Timetables.DTOs;
//using Application.Features.Timetables.Interfaces;
//using ClosedXML.Excel;
//using DocumentFormat.OpenXml.Spreadsheet;
//using Domain.Models;
//using Infrastructure.Repositories.Interfaces;

//namespace Application.Features.Timetables.Services
//{
//    public class TimetableService : ITimetableService
//    {
//        private readonly ITimetableRepository _repository;

//        public TimetableService(ITimetableRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task<TimetableResponse> GetTimetableAsync(int classId, DateOnly effectiveDate)
//        {
//            var timetables = await _repository.GetTimetableByClassAsync(classId, effectiveDate);
//            if (timetables.Count == 0) throw new KeyNotFoundException("Không tìm thấy thời khóa biểu.");

//            var groupedData = timetables.GroupBy(t => t.DayOfWeek)
//                .ToDictionary(
//                    g => g.Key,
//                    g => new TimetableDayResponse
//                    {
//                        Morning = g.Where(t => t.Shift == "Morning")
//                                   .Select(t => new TimetablePeriodResponse { Period = t.Period, Subject = t.Subject.SubjectName, Teacher = t.Teacher.TeacherName })
//                                   .ToList(),
//                        Afternoon = g.Where(t => t.Shift == "Afternoon")
//                                   .Select(t => new TimetablePeriodResponse { Period = t.Period, Subject = t.Subject.SubjectName, Teacher = t.Teacher.TeacherName })
//                                   .ToList()
//                    });

//            return new TimetableResponse
//            {
//                ClassId = classId,
//                ClassName = timetables.First().Class.ClassName,
//                EffectiveDate = effectiveDate,
//                Timetable = groupedData
//            };
//        }

//        public async Task<byte[]> ExportTimetableToExcel(int classId, DateOnly effectiveDate)
//        {
//            var timetables = await _repository.GetTimetableByClassAsync(classId, effectiveDate);
//            using var workbook = new XLWorkbook();
//            var worksheet = workbook.Worksheets.Add("Timetable");

//            worksheet.Cell(1, 1).Value = "Day";
//            worksheet.Cell(1, 2).Value = "Shift";
//            worksheet.Cell(1, 3).Value = "Period";
//            worksheet.Cell(1, 4).Value = "Subject";
//            worksheet.Cell(1, 5).Value = "Teacher";

//            int row = 2;
//            foreach (var t in timetables)
//            {
//                worksheet.Cell(row, 1).Value = t.DayOfWeek;
//                worksheet.Cell(row, 2).Value = t.Shift;
//                worksheet.Cell(row, 3).Value = t.Period;
//                worksheet.Cell(row, 4).Value = t.Subject.SubjectName;
//                worksheet.Cell(row, 5).Value = t.Teacher.TeacherName;
//                row++;
//            }

//            using var stream = new MemoryStream();
//            workbook.SaveAs(stream);
//            return stream.ToArray();
//        }
//    }
//}
