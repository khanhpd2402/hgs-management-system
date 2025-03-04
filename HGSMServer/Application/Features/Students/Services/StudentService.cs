using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            return student == null ? null : _mapper.Map<StudentDto>(student);
        }

        public async Task AddStudentAsync(StudentDto studentDto)

        {
            var student = _mapper.Map<Student>(studentDto);
            await _studentRepository.AddAsync(student);
        }


        public async Task UpdateStudentAsync(StudentDto studentDto)
        {
            var student = _mapper.Map<Student>(studentDto);
            await _studentRepository.UpdateAsync(student);
        }

        public async Task DeleteStudentAsync(int id)
        {
            await _studentRepository.DeleteAsync(id);
        }
        public async Task<byte[]> ExportStudentsToExcelAsync()
        {
            var students = await _studentRepository.GetAllAsync();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Students");

            // Tiêu đề cột
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Họ và Tên";
            worksheet.Cells[1, 3].Value = "Ngày sinh";
            worksheet.Cells[1, 4].Value = "Giới tính";
            worksheet.Cells[1, 5].Value = "Lớp";
            worksheet.Cells[1, 6].Value = "Trạng thái";

            int row = 2;
            foreach (var student in students)
            {
                worksheet.Cells[row, 1].Value = student.StudentId;
                worksheet.Cells[row, 2].Value = student.FullName;
                worksheet.Cells[row, 3].Value = student.Dob.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 4].Value = student.Gender;
                worksheet.Cells[row, 5].Value = student.ClassId;
                worksheet.Cells[row, 6].Value = student.Status;
                row++;
            }

            return package.GetAsByteArray();
        }

        /// <summary>
        /// Nhập danh sách học sinh từ file Excel
        /// </summary>
        public async Task ImportStudentsFromExcelAsync(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);

            var worksheet = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên
            int rowCount = worksheet.Dimension.Rows;

            var students = new List<Student>();

            for (int row = 2; row <= rowCount; row++)
            {
                var student = new Student
                {
                    FullName = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                    Dob = DateOnly.Parse(worksheet.Cells[row, 3].Value?.ToString() ?? "2000-01-01"),
                    Gender = worksheet.Cells[row, 4].Value?.ToString() ?? "Unknown",
                    ClassId = int.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? "0"),
                    Status = worksheet.Cells[row, 6].Value?.ToString()
                };
                students.Add(student);
            }

            foreach (var student in students)
            {
                await _studentRepository.AddAsync(student);
            }
        }
    }
}