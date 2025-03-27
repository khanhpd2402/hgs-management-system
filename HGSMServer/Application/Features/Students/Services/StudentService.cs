using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Constants;
using Common.Utils;
using DocumentFormat.OpenXml.Bibliography;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }
        public async Task<StudentListResponseDto> GetAllStudentsWithParentsAsync(int academicYearId)
        {
            var students = await _studentRepository.GetAllWithParentsAsync(academicYearId);
            var studentDtos = _mapper.Map<List<StudentDto>>(students);

            return new StudentListResponseDto
            {
                Students = studentDtos,
                TotalCount = studentDtos.Count
            };
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id, int academicYearId)
        {
            var student = await _studentRepository.GetByIdWithParentsAsync(id, academicYearId);
            return student == null ? null : _mapper.Map<StudentDto>(student);
        }

        public async Task<int> AddStudentAsync(CreateStudentDto createStudentDto)
        {
            var student = _mapper.Map<Student>(createStudentDto);
            await _studentRepository.AddAsync(student);
            return student.StudentId; // Trả về ID sau khi thêm
        }


        public async Task UpdateStudentAsync(UpdateStudentDto updateStudentDto)
        {
            var student = await _studentRepository.GetByIdAsync(updateStudentDto.StudentId);
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found");
            }

            _mapper.Map(updateStudentDto, student); // Cập nhật từ DTO vào entity
            await _studentRepository.UpdateAsync(student);
        }

        public async Task DeleteStudentAsync(int id)
        {
            await _studentRepository.DeleteAsync(id);
        }

        //public async Task<byte[]> ExportStudentsFullToExcelAsync()
        //{
        //    return await ExportStudentsToExcelAsync(null, false); // Xuất toàn bộ cột
        //}

        //public async Task<byte[]> ExportStudentsSelectedToExcelAsync(List<string> selectedColumns)
        //{
        //    return await ExportStudentsToExcelAsync(selectedColumns, true); // Xuất cột được chọn
        //}

        //        private async Task<byte[]> ExportStudentsToExcelAsync(List<string>? selectedColumns, bool isReport)
        //        {
        //            var students = _studentRepository.GetAllStudentsWithParentsAsync();
        //            var studentDtos = students.Select(StudentToStudentExportDto).ToList();

        //            return ExcelExporter.ExportToExcel(studentDtos, StudentColumnMappings, "DANH SÁCH HỌC SINH", "Năm học 2024", selectedColumns, isReport);
        //        }
        //        private static readonly Dictionary<string, string> StudentColumnMappings = new()
        //{
        //    { "StudentId", "Mã học sinh" },
        //    { "FullName", "Họ và tên" },
        //    { "Dob", "Ngày sinh" },
        //    { "Gender", "Giới tính" },
        //    { "ClassName", "Lớp" },
        //    { "AdmissionDate", "Ngày nhập học" },
        //    { "EnrollmentType", "Hình thức nhập học" },
        //    { "Ethnicity", "Dân tộc" },
        //    { "PermanentAddress", "Địa chỉ thường trú" },
        //    { "BirthPlace", "Nơi sinh" },
        //    { "Religion", "Tôn giáo" },
        //    { "RepeatingYear", "Lưu ban" },
        //    { "IdcardNumber", "Số CMND/CCCD" },
        //    { "Status", "Trạng thái" }
        //};
        //        private StudentExportDto StudentToStudentExportDto(Student s)
        //        {
        //            // Lấy lớp hiện tại (giả sử dựa trên năm học hiện tại)
        //            var currentClass = s.StudentClasses
        //                .FirstOrDefault(sc => sc.AcademicYear.YearName == "2024-2025")?.Class;

        //            return new StudentExportDto
        //            {
        //                StudentId = s.StudentId,
        //                FullName = s.FullName,
        //                Dob = s.Dob.ToString(AppConstants.DATE_FORMAT),
        //                Gender = s.Gender,
        //                ClassName = currentClass?.ClassName ?? "Không rõ",
        //                AdmissionDate = s.AdmissionDate.ToString(AppConstants.DATE_FORMAT),
        //                EnrollmentType = s.EnrollmentType,
        //                Ethnicity = s.Ethnicity,
        //                PermanentAddress = s.PermanentAddress,
        //                BirthPlace = s.BirthPlace,
        //                Religion = s.Religion,
        //                RepeatingYear = s.RepeatingYear.HasValue ? (s.RepeatingYear.Value ? "Có" : "Không") : "Không rõ",
        //                IdcardNumber = s.IdcardNumber,
        //                Status = s.Status
        //            };
        //        }

        public async Task ImportStudentsFromExcelAsync(IFormFile file)
        {
            var data = ExcelImportHelper.ReadExcelData(file);
            var students = new List<Student>();

            foreach (var row in data)
            {
                string idCardNumber = row["Số CMND/CCCD"]?.ToString().Trim();
                if (string.IsNullOrWhiteSpace(idCardNumber))
                {
                    throw new Exception("Số CMND/CCCD không được để trống.");
                }

                // Kiểm tra trùng lặp trước khi insert
                bool exists = await _studentRepository.ExistsAsync(idCardNumber);
                if (exists)
                {
                    throw new Exception($"Số CMND/CCCD {idCardNumber} đã tồn tại.");
                }

                var student = new Student
                {
                    FullName = row["Họ và tên"]?.ToString().Trim() ?? throw new Exception("Họ và tên không được để trống."),
                    Dob = DateHelper.ParseDate(row["Ngày sinh"]?.ToString()),
                    Gender = row["Giới tính"]?.ToString().Trim() ?? throw new Exception("Giới tính không được để trống."),
                    AdmissionDate = DateHelper.ParseDate(row["Ngày nhập học"]?.ToString()),
                    EnrollmentType = row["Hình thức nhập học"]?.ToString().Trim(),
                    Ethnicity = row["Dân tộc"]?.ToString().Trim(),
                    PermanentAddress = row["Địa chỉ thường trú"]?.ToString().Trim(),
                    BirthPlace = row["Nơi sinh"]?.ToString().Trim(),
                    Religion = row["Tôn giáo"]?.ToString().Trim(),
                    RepeatingYear = !string.IsNullOrWhiteSpace(row["Lưu ban"]?.ToString()) && row["Lưu ban"].ToString().Trim() == "Có",
                    IdcardNumber = idCardNumber, // Đã kiểm tra trùng lặp và null trước khi gán
                    Status = row["Trạng thái"]?.ToString().Trim() ?? "Đang học",
                    StudentClasses = new List<StudentClass>
    {
        new StudentClass
        {
            ClassId = 1, // Lớp cố định, có thể thay bằng logic động
            AcademicYearId = 1 // Năm học cố định, cần thay bằng giá trị thực tế
        }
    }
                };
                students.Add(student);
            }

            await _studentRepository.AddRangeAsync(students);
        }
    }
}
