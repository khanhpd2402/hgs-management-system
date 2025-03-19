using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;
using AutoMapper;
using Common.Constants;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Common.Utils;
using System.Globalization;

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
        public async Task<TeacherListResponseDto> GetAllTeachersAsync()
        {
            var query = _teacherRepository.GetAll();
            var teacherList = await Task.Run(() => _mapper.ProjectTo<TeacherListDto>(query).ToList());

            return new TeacherListResponseDto
            {
                Teachers = teacherList,
                TotalCount = teacherList.Count
            };
        }
        public async Task<TeacherDetailDto?> GetTeacherByIdAsync(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            return teacher == null ? null : _mapper.Map<TeacherDetailDto>(teacher);
        }

        public async Task AddTeacherAsync(TeacherListDto teacherDto)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(teacherDto.FullName) ||
                teacherDto.Dob == default ||
                string.IsNullOrEmpty(teacherDto.Gender) ||
                string.IsNullOrEmpty(teacherDto.IdcardNumber) ||
                string.IsNullOrEmpty(teacherDto.InsuranceNumber) ||
                string.IsNullOrEmpty(teacherDto.Department) ||
                teacherDto.SchoolJoinDate == default)
            {
                throw new Exception("Thiếu thông tin bắt buộc. Vui lòng kiểm tra dữ liệu.");
            }

            // Kiểm tra trùng lặp IDCardNumber hoặc InsuranceNumber
            if (await _teacherRepository.ExistsAsync(teacherDto.IdcardNumber, teacherDto.InsuranceNumber))
            {
                throw new Exception($"Giáo viên với CMND/CCCD {teacherDto.IdcardNumber} hoặc Số bảo hiểm {teacherDto.InsuranceNumber} đã tồn tại.");
            }

            // Map từ TeacherListDto sang Teacher
            var teacher = _mapper.Map<Teacher>(teacherDto);

            // Thêm User cho giáo viên
            teacher.User = new User
            {
                Email = teacherDto.Email,
                PhoneNumber = teacherDto.PhoneNumber,
                RoleId = 2,
                Username = FormatUserName.GenerateUsername(teacherDto.FullName, 1),
                PasswordHash = PasswordHasher.HashPassword("DefaultPassword@123")
            };

            // Lưu vào database
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
        private static Dictionary<string, string> GetTeacherColumnMappings()
        {
            return new Dictionary<string, string>
            {
                { "TeacherId", "Mã giáo viên" },
                { "FullName", "Họ và tên" },
                { "Dob", "Ngày sinh" },
                { "Gender", "Giới tính" },
                { "Ethnicity", "Dân tộc" },
                { "Religion", "Tôn giáo" },
                { "MaritalStatus", "Tình trạng hôn nhân" },
                { "IdcardNumber", "CMND/CCCD" },
                { "InsuranceNumber", "Số sổ bảo hiểm" },
                { "EmploymentType", "Hình thức hợp đồng" },
                { "Position", "Vị trí việc làm" },
                { "Department", "Tổ bộ môn" },
                { "AdditionalDuties", "Nhiệm vụ kiêm nhiệm" },
                { "IsHeadOfDepartment", "Là tổ trưởng" },
                { "EmploymentStatus", "Trạng thái cán bộ" },
                { "RecruitmentAgency", "Cơ quan tuyển dụng" },
                { "HiringDate", "Ngày tuyển dụng" },
                { "PermanentEmploymentDate", "Ngày vào biên chế" },
                { "SchoolJoinDate", "Ngày vào trường" },
                { "PermanentAddress", "Địa chỉ thường trú" },
                { "Hometown", "Quê quán" },
                { "Email", "Email" },
                { "PhoneNumber", "Số điện thoại" }
            };
        }

        public async Task ImportTeachersFromExcelAsync(IFormFile file)
        {
            var data = ExcelImportHelper.ReadExcelData(file);
            var teachers = new List<Teacher>();

            foreach (var row in data)
            {
                // Kiểm tra các trường NOT NULL không được để trống
                if (string.IsNullOrEmpty(row["Họ và tên"]) ||
                    string.IsNullOrEmpty(row["Ngày sinh"]) ||
                    string.IsNullOrEmpty(row["Giới tính"]) ||
                    string.IsNullOrEmpty(row["CMND/CCCD"]) ||
                    string.IsNullOrEmpty(row["Số sổ bảo hiểm"]) ||
                    string.IsNullOrEmpty(row["Tổ bộ môn"]) ||
                    string.IsNullOrEmpty(row["Ngày vào trường"]))
                {
                    throw new Exception("Thiếu thông tin bắt buộc. Vui lòng kiểm tra dữ liệu.");
                }

                // Chuẩn hóa dữ liệu
                string idCardNumber = row["CMND/CCCD"].Trim();
                string insuranceNumber = row["Số sổ bảo hiểm"].Trim();

                // Kiểm tra trùng lặp IDCardNumber hoặc InsuranceNumber
                if (await _teacherRepository.ExistsAsync(idCardNumber, insuranceNumber))
                {
                    throw new Exception($"Giáo viên với CMND/CCCD {idCardNumber} hoặc Sổ bảo hiểm {insuranceNumber} đã tồn tại.");
                }

                var user = new User
                {
                    Email = row["Email"],
                    PhoneNumber = row["Số điện thoại"],
                    RoleId = 2,  // Không fix cứng là 2 nữa
                    Username = FormatUserName.GenerateUsername(row["Họ và tên"], 1),
                    PasswordHash = PasswordHasher.HashPassword("DefaultPassword@123")
                };

                var teacher = new Teacher
                {
                    FullName = row["Họ và tên"],
                    Dob = DateHelper.ParseDate(row["Ngày sinh"]),
                    Gender = row["Giới tính"],
                    Ethnicity = row["Dân tộc"],
                    Religion = row["Tôn giáo"],
                    MaritalStatus = row["Tình trạng hôn nhân"],
                    IdcardNumber = idCardNumber,
                    InsuranceNumber = insuranceNumber,
                    EmploymentType = row["Hình thức hợp đồng"],
                    Position = row["Vị trí việc làm"],
                    Department = row["Tổ bộ môn"],
                    AdditionalDuties = row["Nhiệm vụ kiêm nhiệm"],
                    IsHeadOfDepartment = row["Là tổ trưởng"] == "1",
                    EmploymentStatus = row["Trạng thái cán bộ"],
                    RecruitmentAgency = row["Cơ quan tuyển dụng"],
                    HiringDate = DateHelper.ParseDate(row["Ngày tuyển dụng"]),
                    PermanentEmploymentDate = DateHelper.ParseDate(row["Ngày vào biên chế"]),
                    SchoolJoinDate = DateHelper.ParseDate(row["Ngày vào trường"]),
                    PermanentAddress = row["Địa chỉ thường trú"],
                    Hometown = row["Quê quán"],
                    User = user
                };

                teachers.Add(teacher);
            }

            await _teacherRepository.AddRangeAsync(teachers);
        }

    }
}
public class CustomExportException : Exception
{
    public byte[] ExcelBytes { get; }

    public CustomExportException(byte[] excelBytes)
    {
        ExcelBytes = excelBytes;
    }
}