using Application.Features.Subjects.DTOs;
using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;
using AutoMapper;
using Common.Utils;
using Domain.Models;
using Infrastructure.Repositories.Implementtations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly HgsdbContext _context;

    public TeacherService(
        ITeacherRepository teacherRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ISubjectRepository subjectRepository,
        IRoleRepository roleRepository,
        HgsdbContext context)
    {
        _teacherRepository = teacherRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _subjectRepository = subjectRepository;
        _roleRepository = roleRepository;
        _context = context;
    }

    public async Task<TeacherListResponseDto> GetAllTeachersAsync()
    {
        var teachers = await _teacherRepository.GetAllWithUserAsync();
        var teacherList = new List<TeacherListDto>();

        foreach (var teacher in teachers)
        {
            var teacherSubjects = await _teacherRepository.GetTeacherSubjectsAsync(teacher.TeacherId);
            var teacherDto = _mapper.Map<TeacherListDto>(teacher);
            teacherDto.Email = teacher.User?.Email;
            teacherDto.PhoneNumber = teacher.User?.PhoneNumber;
            teacherDto.Subjects = teacherSubjects?.Select(ts => new SubjectTeacherDto
            {
                SubjectName = ts.Subject.SubjectName,
                IsMainSubject = ts.IsMainSubject ?? false
            }).ToList() ?? new List<SubjectTeacherDto>();

            teacherList.Add(teacherDto);
        }

        return new TeacherListResponseDto
        {
            Teachers = teacherList,
            TotalCount = teacherList.Count
        };
    }

    public async Task<TeacherDetailDto?> GetTeacherByIdAsync(int id)
    {
        var teacher = await _teacherRepository.GetByIdWithUserAsync(id);
        if (teacher == null) return null;

        var teacherSubjects = await _teacherRepository.GetTeacherSubjectsAsync(id);
        var teacherDto = _mapper.Map<TeacherDetailDto>(teacher);
        teacherDto.Email = teacher.User?.Email;
        teacherDto.PhoneNumber = teacher.User?.PhoneNumber;
        teacherDto.Subjects = teacherSubjects?.Select(ts => new SubjectTeacherDto
        {
            SubjectName = ts.Subject.SubjectName,
            IsMainSubject = ts.IsMainSubject ?? false
        }).ToList() ?? new List<SubjectTeacherDto>();

        return teacherDto;
    }

    public async Task<string?> GetEmailByTeacherIdAsync(int teacherId)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherId);
        if (teacher == null)
        {
            throw new KeyNotFoundException("Không tìm thấy giáo viên.");
        }

        if (!teacher.UserId.HasValue)
        {
            throw new InvalidOperationException("Giáo viên không có UserId liên kết.");
        }

        var user = await _userRepository.GetByIdAsync(teacher.UserId.Value);
        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy user liên kết với giáo viên.");
        }

        return user.Email;
    }

    public async Task AddTeacherAsync(TeacherListDto teacherDto)
    {
        if (string.IsNullOrEmpty(teacherDto.FullName) || teacherDto.Dob == default ||
            string.IsNullOrEmpty(teacherDto.Gender) || string.IsNullOrEmpty(teacherDto.IdcardNumber) ||
            string.IsNullOrEmpty(teacherDto.InsuranceNumber) || string.IsNullOrEmpty(teacherDto.Department) ||
            teacherDto.SchoolJoinDate == default)
        {
            throw new ArgumentException("Thiếu thông tin bắt buộc. Vui lòng kiểm tra dữ liệu.");
        }

        if (await _teacherRepository.ExistsAsync(teacherDto.IdcardNumber))
        {
            throw new ArgumentException("Giáo viên với CMND/CCCD đã tồn tại.");
        }

        if (await _teacherRepository.IsEmailOrPhoneExistsAsync(teacherDto.Email, teacherDto.PhoneNumber))
        {
            throw new ArgumentException("Email hoặc số điện thoại đã tồn tại.");
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var teacherRole = await _roleRepository.GetRoleByNameAsync(
                teacherDto.IsHeadOfDepartment == true ? "Trưởng bộ môn" : "Giáo viên");
            if (teacherRole == null)
            {
                throw new InvalidOperationException("Vai trò không tồn tại trong hệ thống.");
            }

            var teacher = _mapper.Map<Teacher>(teacherDto);
            teacher.User = new User
            {
                Email = teacherDto.Email,
                PhoneNumber = teacherDto.PhoneNumber,
                RoleId = teacherRole.RoleId,
                Username = "tempuser",
                PasswordHash = PasswordHasher.HashPassword("12345678"),
                Status = "Hoạt động"
            };

            await _teacherRepository.AddAsync(teacher);

            var username = await GenerateUniqueUsernameAsync(teacherDto.FullName);
            teacher.User.Username = username;
            await _teacherRepository.UpdateUserAsync(teacher.User);

            var subjects = teacherDto.Subjects ?? new List<SubjectTeacherDto>();
            if (subjects.Any())
            {
                var teacherSubjects = new List<TeacherSubject>();
                foreach (var subjectDto in subjects)
                {
                    if (string.IsNullOrEmpty(subjectDto.SubjectName))
                    {
                        throw new ArgumentException("Tên môn học không được để trống.");
                    }

                    var subject = await _subjectRepository.GetByNameAsync(subjectDto.SubjectName);
                    if (subject == null)
                    {
                        throw new ArgumentException("Môn học không tồn tại.");
                    }

                    teacherSubjects.Add(new TeacherSubject
                    {
                        TeacherId = teacher.TeacherId,
                        SubjectId = subject.SubjectId,
                        IsMainSubject = subjectDto.IsMainSubject
                    });
                }
                await _teacherRepository.AddTeacherSubjectsRangeAsync(teacherSubjects);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
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
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var existingTeacher = await _teacherRepository.GetByIdWithUserAsync(id);
            if (existingTeacher == null)
            {
                throw new KeyNotFoundException("Không tìm thấy giáo viên.");
            }

            if (teacherDto.TeacherId != id)
            {
                throw new ArgumentException("TeacherId không khớp.");
            }

            _mapper.Map(teacherDto, existingTeacher);
            existingTeacher.TeacherId = id;
            await _teacherRepository.UpdateAsync(existingTeacher);

            if (existingTeacher.User != null)
            {
                var teacherRole = await _roleRepository.GetRoleByNameAsync(
                    teacherDto.IsHeadOfDepartment == true ? "Trưởng bộ môn" : "Giáo viên");
                if (teacherRole == null)
                {
                    throw new InvalidOperationException("Vai trò không tồn tại trong hệ thống.");
                }

                existingTeacher.User.Email = teacherDto.Email;
                existingTeacher.User.PhoneNumber = teacherDto.PhoneNumber;
                existingTeacher.User.RoleId = teacherRole.RoleId;
                await _teacherRepository.UpdateUserAsync(existingTeacher.User);
            }

            var existingSubjects = await _teacherRepository.GetTeacherSubjectsAsync(id);
            var newSubjects = teacherDto.Subjects ?? new List<SubjectTeacherDto>();

            var subjectsToRemove = existingSubjects
                .Where(es => !newSubjects.Any(ns => ns.SubjectName == es.Subject.SubjectName))
                .ToList();
            if (subjectsToRemove.Any())
            {
                await _teacherRepository.DeleteTeacherSubjectsRangeAsync(subjectsToRemove);
            }

            foreach (var subjectDto in newSubjects)
            {
                if (string.IsNullOrEmpty(subjectDto.SubjectName))
                {
                    throw new ArgumentException("Tên môn học không được để trống.");
                }

                var subject = await _subjectRepository.GetByNameAsync(subjectDto.SubjectName);
                if (subject == null)
                {
                    throw new ArgumentException("Môn học không tồn tại.");
                }

                var existingSubject = existingSubjects.FirstOrDefault(es => es.Subject.SubjectName == subjectDto.SubjectName);
                if (existingSubject == null)
                {
                    var teacherSubject = new TeacherSubject
                    {
                        TeacherId = id,
                        SubjectId = subject.SubjectId,
                        IsMainSubject = subjectDto.IsMainSubject
                    };
                    await _teacherRepository.AddTeacherSubjectAsync(teacherSubject);
                }
                else if (existingSubject.IsMainSubject != subjectDto.IsMainSubject)
                {
                    existingSubject.IsMainSubject = subjectDto.IsMainSubject;
                    await _teacherRepository.UpdateTeacherSubjectAsync(existingSubject);
                }
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteTeacherAsync(int id)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            Console.WriteLine("Attempting to delete teacher...");

            var teacher = await _teacherRepository.GetByIdWithUserAsync(id);
            if (teacher == null)
            {
                Console.WriteLine("Teacher not found.");
                return false;
            }

            var teacherSubjects = await _teacherRepository.GetTeacherSubjectsAsync(id);
            if (teacherSubjects?.Any() == true)
            {
                Console.WriteLine("Deleting teacher subjects...");
                await _teacherRepository.DeleteTeacherSubjectsRangeAsync(teacherSubjects);
            }

            Console.WriteLine("Deleting teacher...");
            await _teacherRepository.DeleteAsync(id);

            if (teacher.UserId.HasValue)
            {
                Console.WriteLine("Deleting associated user...");
                await _teacherRepository.DeleteUserAsync(teacher.UserId.Value);
            }

            await transaction.CommitAsync();
            Console.WriteLine("Teacher deleted successfully.");
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error deleting teacher: {ex.Message}");
            throw new Exception("Lỗi khi xóa giáo viên.");
        }
    }

    public async Task<(bool Success, List<string> Errors)> ImportTeachersFromExcelAsync(IFormFile file)
    {
        var errors = new List<string>();

        if (file == null || file.Length == 0)
        {
            errors.Add("Vui lòng chọn file Excel!");
            return (false, errors);
        }

        var data = ExcelImportHelper.ReadExcelData(file);
        var teachers = new Dictionary<string, Teacher>();
        var teacherSubjects = new List<(string IdCardNumber, TeacherSubject TeacherSubject)>();

        // Lấy vai trò "Giáo viên"
        var teacherRole = await _roleRepository.GetRoleByNameAsync("Giáo viên");
        if (teacherRole == null)
        {
            errors.Add("Vai trò 'Giáo viên' không tồn tại trong hệ thống.");
            return (false, errors);
        }

        // Lấy vai trò "Trưởng bộ môn"
        var headOfDepartmentRole = await _roleRepository.GetRoleByNameAsync("Trưởng bộ môn");
        if (headOfDepartmentRole == null)
        {
            errors.Add("Vai trò 'Trưởng bộ môn' không tồn tại trong hệ thống.");
            return (false, errors);
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            foreach (var row in data)
            {
                if (string.IsNullOrEmpty(row["Họ và tên"]) || string.IsNullOrEmpty(row["Ngày sinh"]) ||
                    string.IsNullOrEmpty(row["Giới tính"]) || string.IsNullOrEmpty(row["CMND/CCCD"]) ||
                    string.IsNullOrEmpty(row["Ngày vào trường"]))
                {
                    errors.Add("Dòng dữ liệu thiếu thông tin bắt buộc.");
                    continue;
                }

                var idCardNumber = row["CMND/CCCD"].Trim();

                if (!teachers.TryGetValue(idCardNumber, out var teacher))
                {
                    if (await _teacherRepository.ExistsAsync(idCardNumber))
                    {
                        errors.Add("Giáo viên với CMND/CCCD đã tồn tại trong hệ thống.");
                    }

                    if (await _teacherRepository.IsEmailOrPhoneExistsAsync(row["Email"], row["Số điện thoại"]))
                    {
                        errors.Add("Email hoặc số điện thoại đã tồn tại.");
                    }

                    var username = await GenerateUniqueUsernameAsync(row["Họ và tên"]);

                    // Kiểm tra cột "Là tổ trưởng" để gán vai trò phù hợp
                    bool isHeadOfDepartment = row.TryGetValue("Là tổ trưởng", out var isHead) && isHead == "Có";
                    var roleId = isHeadOfDepartment ? headOfDepartmentRole.RoleId : teacherRole.RoleId;

                    var user = new User
                    {
                        Email = row["Email"],
                        PhoneNumber = row["Số điện thoại"],
                        RoleId = roleId, // Gán RoleId dựa trên điều kiện
                        Username = username,
                        PasswordHash = PasswordHasher.HashPassword("12345678"),
                        Status = "Hoạt động"
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
                        IsHeadOfDepartment = isHeadOfDepartment,
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

                var subjectsInput = row.TryGetValue("Môn dạy", out var subjects) ? subjects : null;
                if (!string.IsNullOrEmpty(subjectsInput))
                {
                    var subjectParts = subjectsInput.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in subjectParts)
                    {
                        var subjectName = part.Trim();
                        bool isMainSubject = subjectName.Contains("*");
                        if (isMainSubject)
                        {
                            subjectName = subjectName.Replace("*", "").Trim();
                        }

                        var subject = await _subjectRepository.GetByNameAsync(subjectName);
                        if (subject == null)
                        {
                            errors.Add("Môn học không tồn tại trong hệ thống.");
                            continue;
                        }

                        teacherSubjects.Add((idCardNumber, new TeacherSubject
                        {
                            TeacherId = 0,
                            SubjectId = subject.SubjectId,
                            IsMainSubject = isMainSubject
                        }));
                    }
                }
            }

            if (errors.Any())
            {
                return (false, errors);
            }

            await _teacherRepository.AddRangeAsync(teachers.Values);

            foreach (var teacherSubject in teacherSubjects)
            {
                if (teachers.TryGetValue(teacherSubject.IdCardNumber, out var teacher))
                {
                    teacherSubject.TeacherSubject.TeacherId = teacher.TeacherId;
                }
            }

            if (teacherSubjects.Any())
            {
                await _teacherRepository.AddTeacherSubjectsRangeAsync(teacherSubjects.Select(ts => ts.TeacherSubject).ToList());
            }

            await transaction.CommitAsync();
            return (true, errors);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}