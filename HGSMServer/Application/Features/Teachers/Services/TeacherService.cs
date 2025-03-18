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
        private readonly ITeacherClassRepository _teacherClassRepository; // Thêm repository mới
        private readonly IMapper _mapper;

        public TeacherService(ITeacherRepository teacherRepository, ITeacherClassRepository teacherClassRepository, IMapper mapper)
        {
            _teacherRepository = teacherRepository;
            _teacherClassRepository = teacherClassRepository;
            _mapper = mapper;
        }
        public async Task<TeacherListResponseDto> GetAllTeachersAsync(bool exportToExcel = false, List<string> selectedColumns = null)
        {
            var query = _teacherRepository.GetAll();

            var teachers = await Task.Run(() => _mapper.ProjectTo<TeacherListDto>(query).ToList());

            if (exportToExcel)
            {
                var exportDtos = query.Select(TeacherToTeacherExportDto).ToList();
                string sheetName = selectedColumns != null ? "Danh sách cán bộ" : "Danh sách giáo viên";
                string title = "Năm học 2024-2025";
                bool isCustomColumns = selectedColumns != null;

                var excelBytes = ExcelExporter.ExportToExcel(exportDtos, GetTeacherColumnMappings(), sheetName, title, selectedColumns, isCustomColumns);
                throw new CustomExportException(excelBytes);
            }

            return new TeacherListResponseDto
            {
                Teachers = teachers,
                TotalCount = teachers.Count
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

        private TeacherExportDto TeacherToTeacherExportDto(Teacher t)
        {
            return new TeacherExportDto
            {
                TeacherId = t.TeacherId,
                FullName = t.FullName,
                Dob = t.Dob.ToString(AppConstants.DATE_FORMAT),
                Gender = t.Gender,
                Ethnicity = t.Ethnicity,
                Religion = t.Religion,
                MaritalStatus = t.MaritalStatus,
                IdcardNumber = t.IdcardNumber,
                InsuranceNumber = t.InsuranceNumber,
                EmploymentType = t.EmploymentType,
                Position = t.Position,
                Department = t.Department,
                AdditionalDuties = t.AdditionalDuties,
                IsHeadOfDepartment = (bool)t.IsHeadOfDepartment ? "Có" : "Không",
                EmploymentStatus = t.EmploymentStatus,
                RecruitmentAgency = t.RecruitmentAgency,
                HiringDate = t.HiringDate?.ToString(AppConstants.DATE_FORMAT) ?? "Không rõ",
                PermanentEmploymentDate = t.PermanentEmploymentDate?.ToString(AppConstants.DATE_FORMAT) ?? "Không rõ",
                SchoolJoinDate = t.SchoolJoinDate.ToString(AppConstants.DATE_FORMAT),
                PermanentAddress = t.PermanentAddress,
                Hometown = t.Hometown,
                Email = t.User?.Email ?? "Không có",
                PhoneNumber = t.User?.PhoneNumber ?? "Không có"
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

        public async Task AssignHomeroomAsync(AssignHomeroomDto assignHomeroomDto)
        {
            if (assignHomeroomDto == null)
                throw new ArgumentNullException(nameof(assignHomeroomDto));

            if (assignHomeroomDto.TeacherId <= 0 || assignHomeroomDto.ClassId <= 0 ||
                assignHomeroomDto.AcademicYearId <= 0 || assignHomeroomDto.SemesterId <= 0)
            {
                throw new ArgumentException("All IDs (TeacherId, ClassId, AcademicYearId, SemesterId) must be positive.");
            }

            // Kiểm tra xem giáo viên đã được phân công làm chủ nhiệm lớp này chưa
            var isAssigned = await _teacherClassRepository.IsHomeroomAssignedAsync(
                assignHomeroomDto.TeacherId,
                assignHomeroomDto.ClassId,
                assignHomeroomDto.AcademicYearId);

            if (isAssigned)
            {
                throw new InvalidOperationException("This teacher is already assigned as homeroom teacher for this class in the specified academic year.");
            }

            await _teacherClassRepository.AssignHomeroomAsync(
                assignHomeroomDto.TeacherId,
                assignHomeroomDto.ClassId,
                assignHomeroomDto.AcademicYearId,
                assignHomeroomDto.SemesterId);
        }

        public async Task<bool> IsHomeroomAssignedAsync(int teacherId, int classId, int academicYearId)
        {
            return await _teacherClassRepository.IsHomeroomAssignedAsync(teacherId, classId, academicYearId);
        }

        public async Task<bool> HasHomeroomTeacherAsync(int classId, int academicYearId)
        {
            return await _teacherClassRepository.HasHomeroomTeacherAsync(classId, academicYearId);
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