using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;
using AutoMapper;
using DocumentFormat.OpenXml.Math;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Teachers.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IMapper _mapper;

        public TeacherService(ITeacherRepository teacherRepository, IMapper mapper)
        {
            _teacherRepository = teacherRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TeacherDetailDto>> GetAllTeachersAsync()
        {
            var teachers = await _teacherRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TeacherDetailDto>>(teachers);
        }

        public async Task<TeacherDetailDto?> GetTeacherByIdAsync(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            return teacher == null ? null : _mapper.Map<TeacherDetailDto>(teacher);
        }

        public async Task AddTeacherAsync(TeacherDetailDto teacherDto)
        {
            var teacher = _mapper.Map<Teacher>(teacherDto);
            await _teacherRepository.AddAsync(teacher);
        }

        public async Task UpdateTeacherAsync(int id, TeacherDetailDto teacherDto)
        {
            var teacher = _mapper.Map<Teacher>(teacherDto);
            teacher.TeacherId = id; // Giữ nguyên ID khi update
            await _teacherRepository.UpdateAsync(teacher);
        }

        public async Task DeleteTeacherAsync(int id)
        {
            await _teacherRepository.DeleteAsync(id);
        }
        public async Task<byte[]> ExportTeachersToExcelAsync()
        {
            var teachers = await _teacherRepository.GetAllAsync();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Teachers");

            // Header
            string[] headers = new string[]
            {
        "ID", "Họ và Tên", "Ngày sinh", "Giới tính", "Dân tộc", "Tôn giáo",
        "Tình trạng hôn nhân", "CMND/CCCD", "Số sổ bảo hiểm", "Hình thức hợp đồng",
        "Vị trí việc làm", "Tổ bộ môn", "Nhiệm vụ kiêm nhiệm", "Là tổ trưởng",
        "Trạng thái cán bộ", "Cơ quan tuyển dụng", "Ngày tuyển dụng",
        "Ngày vào biên chế", "Ngày vào trường", "Địa chỉ thường trú", "Quê quán"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            // Dữ liệu
            int row = 2;
            foreach (var teacher in teachers)
            {
                worksheet.Cells[row, 1].Value = teacher.TeacherId;
                worksheet.Cells[row, 2].Value = teacher.FullName;
                worksheet.Cells[row, 3].Value = teacher.Dob.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 4].Value = teacher.Gender;
                worksheet.Cells[row, 5].Value = teacher.Ethnicity;
                worksheet.Cells[row, 6].Value = teacher.Religion;
                worksheet.Cells[row, 7].Value = teacher.MaritalStatus;
                worksheet.Cells[row, 8].Value = teacher.IdcardNumber;
                worksheet.Cells[row, 9].Value = teacher.InsuranceNumber;
                worksheet.Cells[row, 10].Value = teacher.EmploymentType;
                worksheet.Cells[row, 11].Value = teacher.Position;
                worksheet.Cells[row, 12].Value = teacher.Department;
                worksheet.Cells[row, 13].Value = teacher.AdditionalDuties;
                worksheet.Cells[row, 14].Value = (bool)teacher.IsHeadOfDepartment ? "Có" : "Không";
                worksheet.Cells[row, 15].Value = teacher.EmploymentStatus;
                worksheet.Cells[row, 16].Value = teacher.RecruitmentAgency;
                worksheet.Cells[row, 17].Value = teacher.HiringDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 18].Value = teacher.PermanentEmploymentDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 19].Value = teacher.SchoolJoinDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 20].Value = teacher.PermanentAddress;
                worksheet.Cells[row, 21].Value = teacher.Hometown;
                row++;
            }

            return package.GetAsByteArray();
        }

        public async Task<byte[]> ExportTeachersSelectedToExcelAsync(List<string> selectedColumns)
        {
            var teachers = await _teacherRepository.GetAllAsync();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Teachers");

            // Danh sách cột có thể chọn
            var columnMappings = new Dictionary<string, Func<Teacher, object>>
    {
        { "ID", t => t.TeacherId },
        { "Họ và Tên", t => t.FullName },
        { "Ngày sinh", t => t.Dob.ToString("yyyy-MM-dd") },
        { "Giới tính", t => t.Gender },
        { "Dân tộc", t => t.Ethnicity },
        { "Tôn giáo", t => t.Religion },
        { "Tình trạng hôn nhân", t => t.MaritalStatus },
        { "CMND/CCCD", t => t.IdcardNumber },
        { "Số sổ bảo hiểm", t => t.InsuranceNumber },
        { "Hình thức hợp đồng", t => t.EmploymentType },
        { "Vị trí việc làm", t => t.Position },
        { "Tổ bộ môn", t => t.Department },
        { "Nhiệm vụ kiêm nhiệm", t => t.AdditionalDuties },
        { "Là tổ trưởng", t => (bool)t.IsHeadOfDepartment ? "Có" : "Không" },
        { "Trạng thái cán bộ", t => t.EmploymentStatus },
        { "Cơ quan tuyển dụng", t => t.RecruitmentAgency },
        { "Ngày tuyển dụng", t => t.HiringDate?.ToString("yyyy-MM-dd") },
        { "Ngày vào biên chế", t => t.PermanentEmploymentDate?.ToString("yyyy-MM-dd") },
        { "Ngày vào trường", t => t.SchoolJoinDate.ToString("yyyy-MM-dd") },
        { "Địa chỉ thường trú", t => t.PermanentAddress },
        { "Quê quán", t => t.Hometown },
        { "Email", t => t.User?.Email },
        { "Số điện thoại", t => t.User?.PhoneNumber }
    };

            // Ghi tiêu đề
            for (int i = 0; i < selectedColumns.Count; i++)
            {
                worksheet.Cells[1, i + 1].Value = selectedColumns[i];
            }

            // Ghi dữ liệu
            int row = 2;
            foreach (var teacher in teachers)
            {
                for (int col = 0; col < selectedColumns.Count; col++)
                {
                    if (columnMappings.TryGetValue(selectedColumns[col], out var valueFunc))
                    {
                        worksheet.Cells[row, col + 1].Value = valueFunc(teacher);
                    }
                }
                row++;
            }

            return package.GetAsByteArray();
        }

        public async Task ImportTeachersFromExcelAsync(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);

            var worksheet = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên
            int rowCount = worksheet.Dimension.Rows;

            var teachers = new List<Teacher>();

            for (int row = 2; row <= rowCount; row++)
            {
                var email = worksheet.Cells[row, 5].Value?.ToString();
                var phoneNumber = worksheet.Cells[row, 6].Value?.ToString();

                var user = new User
                {
                    Email = email,
                    PhoneNumber = phoneNumber,
                    RoleId = 2, // Mặc định giáo viên
                    Username = "",
                    PasswordHash = ("DefaultPassword@123") // Hash mật khẩu mặc định
                };

                var teacher = new Teacher
                {
                    FullName = worksheet.Cells[row, 2].Value?.ToString(),
                    Dob = DateOnly.Parse(worksheet.Cells[row, 3].Value.ToString()),
                    Gender = worksheet.Cells[row, 4].Value?.ToString(),
                    User = user
                };

                teachers.Add(teacher);
            }

            foreach (var teacher in teachers)
            {
                await _teacherRepository.AddAsync(teacher);
            }
        }

    }
}