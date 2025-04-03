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
        private readonly ITeacherClassRepository _teacherClassRepository;
        private readonly IMapper _mapper;

        public TeacherService(ITeacherRepository teacherRepository, ITeacherClassRepository teacherClassRepository, IMapper mapper)
        {
            _teacherRepository = teacherRepository;
            _teacherClassRepository = teacherClassRepository;
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
            if (await _teacherRepository.ExistsAsync(teacherDto.IdcardNumber))
            {
                throw new Exception($"Giáo viên với CMND/CCCD {teacherDto.IdcardNumber} hoặc Số bảo hiểm {teacherDto.InsuranceNumber} đã tồn tại.");
            }
            // Kiểm tra email hoặc số điện thoại đã tồn tại
            if (await _teacherRepository.IsEmailOrPhoneExistsAsync(teacherDto.Email, teacherDto.PhoneNumber))
            {
                throw new Exception($"Email {teacherDto.Email} hoặc số điện thoại {teacherDto.PhoneNumber} đã tồn tại.");
            }

            // Tạo username không trùng

            // Map từ TeacherListDto sang Teacher
            var teacher = _mapper.Map<Teacher>(teacherDto);
            var username = await GenerateUniqueUsernameAsync(teacherDto.FullName);
            // Thêm User cho giáo viên
            teacher.User = new User
            {
                Email = teacherDto.Email,
                PhoneNumber = teacherDto.PhoneNumber,
                RoleId = 2,
                Username = username,
                PasswordHash = PasswordHasher.HashPassword("DefaultPassword@123")
            };

            // Lưu vào database
            await _teacherRepository.AddAsync(teacher);
        }

        public async Task<string> GenerateUniqueUsernameAsync(string fullName)
        {
            int counter = 1;
            string username;

            do
            {
                username = FormatUserName.GenerateUsername(fullName, counter);
                counter++;
            } while (await _teacherRepository.IsUsernameExistsAsync(username));

            return username;
        }

        public async Task UpdateTeacherAsync(int id, TeacherDetailDto teacherDto)
        {
            var teacher = _mapper.Map<Teacher>(teacherDto);
            teacher.TeacherId = id; // Giữ nguyên ID khi update
            await _teacherRepository.UpdateAsync(teacher);
        }

        public async Task<bool> DeleteTeacherAsync(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null)
                return false; // Trả về false nếu không tìm thấy

            // Xử lý xóa liên quan (nếu có)
            var teacherSubjects = await _teacherRepository.GetTeacherSubjectsAsync(id);
            if (teacherSubjects?.Any() == true)
            {
                await _teacherRepository.DeleteTeacherSubjectsAsync(id);
            }

            await _teacherRepository.DeleteAsync(id);
            return true; // Trả về true nếu xóa thành công
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
                    string.IsNullOrEmpty(row["Ngày vào trường"]))
                {
                    throw new Exception("Thiếu thông tin bắt buộc. Vui lòng kiểm tra dữ liệu.");
                }
                // Kiểm tra email hoặc số điện thoại đã tồn tại
                if (await _teacherRepository.IsEmailOrPhoneExistsAsync(row["Email"], row["Số điện thoại"]))
                {
                    throw new Exception($"Email {row["Email"]} hoặc số điện thoại {row["Số điện thoại"]} đã tồn tại.");
                }

                // Tạo username không trùng
                var username = await GenerateUniqueUsernameAsync(row["Họ và tên"]);

                // Chuẩn hóa dữ liệu
                string idCardNumber = row["CMND/CCCD"].Trim();
                // Kiểm tra trùng lặp IDCardNumber hoặc InsuranceNumber
                if (await _teacherRepository.ExistsAsync(idCardNumber))
                {
                    throw new Exception($"Giáo viên với CMND/CCCD {idCardNumber} đã tồn tại.");
                }
                var user = new User
                {
                    Email = row["Email"],
                    PhoneNumber = row["Số điện thoại"],
                    RoleId = 2,  // Không fix cứng là 2 nữa
                    Username = username,
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
                    InsuranceNumber = row["Số sổ bảo hiểm"],
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