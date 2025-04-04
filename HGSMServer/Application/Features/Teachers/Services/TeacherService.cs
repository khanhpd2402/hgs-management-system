using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;
using AutoMapper;
using Common.Utils;
using Domain.Models;
using Infrastructure.Repositories.Implementtations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly ITeacherClassRepository _teacherClassRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMapper _mapper;

    public TeacherService(ITeacherRepository teacherRepository, ITeacherClassRepository teacherClassRepository, IMapper mapper, ISubjectRepository subjectRepository)
    {
        _teacherRepository = teacherRepository;
        _teacherClassRepository = teacherClassRepository;
        _mapper = mapper;
        _subjectRepository = subjectRepository;
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
        if (string.IsNullOrEmpty(teacherDto.FullName) || teacherDto.Dob == default ||
            string.IsNullOrEmpty(teacherDto.Gender) || string.IsNullOrEmpty(teacherDto.IdcardNumber) ||
            string.IsNullOrEmpty(teacherDto.InsuranceNumber) || string.IsNullOrEmpty(teacherDto.Department) ||
            teacherDto.SchoolJoinDate == default)
        {
            throw new Exception("Thiếu thông tin bắt buộc. Vui lòng kiểm tra dữ liệu.");
        }

        if (await _teacherRepository.ExistsAsync(teacherDto.IdcardNumber))
        {
            throw new Exception($"Giáo viên với CMND/CCCD {teacherDto.IdcardNumber} đã tồn tại.");
        }

        if (await _teacherRepository.IsEmailOrPhoneExistsAsync(teacherDto.Email, teacherDto.PhoneNumber))
        {
            throw new Exception($"Email {teacherDto.Email} hoặc số điện thoại {teacherDto.PhoneNumber} đã tồn tại.");
        }

        var username = await GenerateUniqueUsernameAsync(teacherDto.FullName);
        var teacher = _mapper.Map<Teacher>(teacherDto);
        teacher.User = new User
        {
            Email = teacherDto.Email,
            PhoneNumber = teacherDto.PhoneNumber,
            RoleId = 2,
            Username = username,
            PasswordHash = PasswordHasher.HashPassword("DefaultPassword@123")
        };

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
        teacher.TeacherId = id;
        await _teacherRepository.UpdateAsync(teacher);
    }

    public async Task<bool> DeleteTeacherAsync(int id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher == null) return false;

        var teacherSubjects = await _teacherRepository.GetTeacherSubjectsAsync(id);
        if (teacherSubjects?.Any() == true)
        {
            await _teacherRepository.DeleteTeacherSubjectsAsync(id);
        }

        await _teacherRepository.DeleteAsync(id);
        return true;
    }

    public async Task ImportTeachersFromExcelAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Vui lòng chọn file Excel!");

        var data = ExcelImportHelper.ReadExcelData(file);
        var teachers = new Dictionary<string, Teacher>(); // Dùng Dictionary để gộp giáo viên trùng
        var teacherSubjects = new List<TeacherSubject>();

        foreach (var row in data)
        {
            if (string.IsNullOrEmpty(row["Họ và tên"]) || string.IsNullOrEmpty(row["Ngày sinh"]) ||
                string.IsNullOrEmpty(row["Giới tính"]) || string.IsNullOrEmpty(row["CMND/CCCD"]) ||
                string.IsNullOrEmpty(row["Ngày vào trường"]))
            {
                throw new Exception("Thiếu thông tin bắt buộc. Vui lòng kiểm tra dữ liệu.");
            }

            var idCardNumber = row["CMND/CCCD"].Trim();

            // Nếu giáo viên chưa tồn tại trong Dictionary, tạo mới
            if (!teachers.TryGetValue(idCardNumber, out var teacher))
            {
                if (await _teacherRepository.ExistsAsync(idCardNumber))
                {
                    throw new Exception($"Giáo viên với CMND/CCCD {idCardNumber} đã tồn tại trong hệ thống.");
                }

                if (await _teacherRepository.IsEmailOrPhoneExistsAsync(row["Email"], row["Số điện thoại"]))
                {
                    throw new Exception($"Email {row["Email"]} hoặc số điện thoại {row["Số điện thoại"]} đã tồn tại.");
                }

                var username = await GenerateUniqueUsernameAsync(row["Họ và tên"]);
                var user = new User
                {
                    Email = row["Email"],
                    PhoneNumber = row["Số điện thoại"],
                    RoleId = 2,
                    Username = username,
                    PasswordHash = PasswordHasher.HashPassword("DefaultPassword@123")
                };

                teacher = new Teacher
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
                    IsHeadOfDepartment = row.TryGetValue("Là tổ trưởng", out var isHead) && isHead == "Có",
                    EmploymentStatus = row.TryGetValue("Trạng thái cán bộ", out var status) ? status : "Đang làm việc",
                    RecruitmentAgency = row.TryGetValue("Cơ quan tuyển dụng", out var agency) ? agency : null,
                    HiringDate = row.TryGetValue("Ngày tuyển dụng", out var hiringDateStr) ? DateHelper.ParseDate(hiringDateStr) : null,
                    PermanentEmploymentDate = row.TryGetValue("Ngày vào biên chế", out var permDateStr) ? DateHelper.ParseDate(permDateStr) : null,
                    SchoolJoinDate = DateHelper.ParseDate(row["Ngày vào trường"]),
                    PermanentAddress = row.TryGetValue("Địa chỉ thường trú", out var permAddress) ? permAddress : null,
                    Hometown = row.TryGetValue("Quê quán", out var hometown) ? hometown : null,
                    User = user
                };

                teachers[idCardNumber] = teacher;
            }

            // Xử lý cột "Môn dạy"
            var subjectsInput = row.TryGetValue("Môn dạy", out var subjects) ? subjects : null;
            if (!string.IsNullOrEmpty(subjectsInput))
            {
                var subjectParts = subjectsInput.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in subjectParts)
                {
                    var subjectName = part.Trim();
                    bool isMainSubject = subjectName.Contains("*"); // Dùng "*" để xác định môn chính
                    if (isMainSubject)
                    {
                        subjectName = subjectName.Replace("*", "").Trim(); // Loại bỏ dấu "*"
                    }

                    var subject = await _subjectRepository.GetByNameAsync(subjectName);
                    if (subject == null)
                    {
                        throw new Exception($"Môn học '{subjectName}' không tồn tại trong bảng Subjects.");
                    }

                    // Thêm vào TeacherSubjects
                    teacherSubjects.Add(new TeacherSubject
                    {
                        TeacherId = 0, // Sẽ được cập nhật sau
                        SubjectId = subject.SubjectId,
                        IsMainSubject = isMainSubject
                    });
                }
            }
        }

        // Thêm tất cả giáo viên vào bảng Teachers
        await _teacherRepository.AddRangeAsync(teachers.Values);

        // Cập nhật TeacherId cho teacherSubjects
        foreach (var teacher in teachers.Values)
        {
            foreach (var ts in teacherSubjects.Where(ts => ts.TeacherId == 0))
            {
                ts.TeacherId = teacher.TeacherId;
            }
        }

        // Thêm tất cả TeacherSubjects
        if (teacherSubjects.Any())
        {
            await _teacherRepository.AddTeacherSubjectsRangeAsync(teacherSubjects);
        }
    }

    public async Task AssignHomeroomAsync(AssignHomeroomDto assignHomeroomDto)
    {
        if (assignHomeroomDto == null) throw new ArgumentNullException(nameof(assignHomeroomDto));

        if (assignHomeroomDto.TeacherId <= 0 || assignHomeroomDto.ClassId <= 0 ||
            assignHomeroomDto.AcademicYearId <= 0 || assignHomeroomDto.SemesterId <= 0)
        {
            throw new ArgumentException("All IDs must be positive.");
        }

        var isAssigned = await _teacherClassRepository.IsHomeroomAssignedAsync(
            assignHomeroomDto.TeacherId, assignHomeroomDto.ClassId, assignHomeroomDto.AcademicYearId);

        if (isAssigned)
        {
            throw new InvalidOperationException("This teacher is already assigned as homeroom teacher.");
        }

        await _teacherClassRepository.AssignHomeroomAsync(
            assignHomeroomDto.TeacherId, assignHomeroomDto.ClassId,
            assignHomeroomDto.AcademicYearId, assignHomeroomDto.SemesterId);
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