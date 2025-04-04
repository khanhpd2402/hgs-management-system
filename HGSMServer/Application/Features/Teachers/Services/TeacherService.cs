using Application.Features.Subjects.DTOs;
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
        // Lấy tất cả giáo viên cùng thông tin User
        var teachers = await _teacherRepository.GetAllWithUserAsync();
        var teacherList = new List<TeacherListDto>();

        foreach (var teacher in teachers)
        {
            // Lấy danh sách môn học từ bảng TeacherSubjects
            var teacherSubjects = await _teacherRepository.GetTeacherSubjectsAsync(teacher.TeacherId);

            // Ánh xạ sang TeacherListDto
            var teacherDto = _mapper.Map<TeacherListDto>(teacher);

            // Gán thông tin từ bảng Users
            teacherDto.Email = teacher.User?.Email;
            teacherDto.PhoneNumber = teacher.User?.PhoneNumber;

            // Gán danh sách môn học
            teacherDto.Subjects = teacherSubjects?.Select(ts => new SubjectDto
            {
                SubjectId = ts.Subject.SubjectId,
                SubjectName = ts.Subject.SubjectName,
                IsMainSubject = ts.IsMainSubject ?? false
            }).ToList() ?? new List<SubjectDto>();

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

        
        teacherDto.Subjects = teacherSubjects?.Select(ts => new SubjectDto
        {
            SubjectId = ts.Subject.SubjectId,
            SubjectName = ts.Subject.SubjectName,
            IsMainSubject = ts.IsMainSubject ?? false
        }).ToList() ?? new List<SubjectDto>();

        return teacherDto;
    }

    public async Task AddTeacherAsync(TeacherListDto teacherDto)
    {
        // Kiểm tra thông tin bắt buộc
        if (string.IsNullOrEmpty(teacherDto.FullName) || teacherDto.Dob == default ||
            string.IsNullOrEmpty(teacherDto.Gender) || string.IsNullOrEmpty(teacherDto.IdcardNumber) ||
            string.IsNullOrEmpty(teacherDto.InsuranceNumber) || string.IsNullOrEmpty(teacherDto.Department) ||
            teacherDto.SchoolJoinDate == default)
        {
            throw new Exception("Thiếu thông tin bắt buộc. Vui lòng kiểm tra dữ liệu.");
        }

        // Kiểm tra trùng lặp IdcardNumber
        if (await _teacherRepository.ExistsAsync(teacherDto.IdcardNumber))
        {
            throw new Exception($"Giáo viên với CMND/CCCD {teacherDto.IdcardNumber} đã tồn tại.");
        }

        // Kiểm tra trùng lặp Email hoặc PhoneNumber
        if (await _teacherRepository.IsEmailOrPhoneExistsAsync(teacherDto.Email, teacherDto.PhoneNumber))
        {
            throw new Exception($"Email {teacherDto.Email} hoặc số điện thoại {teacherDto.PhoneNumber} đã tồn tại.");
        }

        // Ánh xạ DTO sang Teacher, chưa gán username ngay
        var teacher = _mapper.Map<Teacher>(teacherDto);
        teacher.User = new User
        {
            Email = teacherDto.Email,
            PhoneNumber = teacherDto.PhoneNumber,
            RoleId = 4,
            Username = "tempuser", // Username tạm thời
            PasswordHash = PasswordHasher.HashPassword("12345678"),
            Status = "Active"
        };

        // Thêm giáo viên vào DB để lấy UserId
        await _teacherRepository.AddAsync(teacher);

        // Sinh username dựa trên UserId vừa tạo
        var username = await GenerateUniqueUsernameAsync(teacherDto.FullName);
        teacher.User.Username = username;
        await _teacherRepository.UpdateUserAsync(teacher.User);

        // Xử lý danh sách môn học (nếu có)
        var subjects = teacherDto.Subjects ?? new List<SubjectDto>();
        if (subjects.Any())
        {
            var teacherSubjects = new List<TeacherSubject>();
            foreach (var subjectDto in subjects)
            {
                // Kiểm tra môn học có tồn tại không
                var subject = await _subjectRepository.GetByIdAsync(subjectDto.SubjectId);
                if (subject == null)
                {
                    throw new Exception($"Môn học với ID {subjectDto.SubjectId} không tồn tại.");
                }

                teacherSubjects.Add(new TeacherSubject
                {
                    TeacherId = teacher.TeacherId,
                    SubjectId = subjectDto.SubjectId,
                    IsMainSubject = subjectDto.IsMainSubject
                });
            }

            // Thêm tất cả TeacherSubjects
            await _teacherRepository.AddTeacherSubjectsRangeAsync(teacherSubjects);
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
        // Kiểm tra giáo viên có tồn tại không
        var existingTeacher = await _teacherRepository.GetByIdWithUserAsync(id);
        if (existingTeacher == null)
        {
            throw new Exception($"Không tìm thấy giáo viên với ID {id}.");
        }

        // Kiểm tra TeacherId trong DTO có khớp với URL không
        if (teacherDto.TeacherId != id)
        {
            throw new Exception($"TeacherId trong DTO ({teacherDto.TeacherId}) không khớp với TeacherId trong URL ({id}).");
        }

        // Cập nhật thông tin bảng Teachers trực tiếp trên existingTeacher
        _mapper.Map(teacherDto, existingTeacher); // Ánh xạ từ DTO sang entity đã có
        existingTeacher.TeacherId = id; // Đảm bảo giữ nguyên TeacherId
        await _teacherRepository.UpdateAsync(existingTeacher);

        // Cập nhật thông tin bảng Users (email, phone)
        if (existingTeacher.User != null)
        {
            existingTeacher.User.Email = teacherDto.Email;
            existingTeacher.User.PhoneNumber = teacherDto.PhoneNumber;
            await _teacherRepository.UpdateUserAsync(existingTeacher.User);
        }

        // Xử lý danh sách môn học (TeacherSubjects)
        var existingSubjects = await _teacherRepository.GetTeacherSubjectsAsync(id);
        var newSubjects = teacherDto.Subjects ?? new List<SubjectDto>();

        // Xóa các môn học cũ không còn trong danh sách mới
        var subjectsToRemove = existingSubjects
            .Where(es => !newSubjects.Any(ns => ns.SubjectId == es.SubjectId))
            .ToList();
        if (subjectsToRemove.Any())
        {
            await _teacherRepository.DeleteTeacherSubjectsRangeAsync(subjectsToRemove);
        }

        // Thêm hoặc cập nhật các môn học mới
        foreach (var subjectDto in newSubjects)
        {
            var existingSubject = existingSubjects.FirstOrDefault(es => es.SubjectId == subjectDto.SubjectId);
            if (existingSubject == null)
            {
                // Kiểm tra xem SubjectId có tồn tại trong bảng Subjects không
                var subject = await _subjectRepository.GetByIdAsync(subjectDto.SubjectId);
                if (subject == null)
                {
                    throw new Exception($"Môn học với ID {subjectDto.SubjectId} không tồn tại.");
                }

                // Thêm mới TeacherSubject
                var teacherSubject = new TeacherSubject
                {
                    TeacherId = id,
                    SubjectId = subjectDto.SubjectId,
                    IsMainSubject = subjectDto.IsMainSubject
                };
                await _teacherRepository.AddTeacherSubjectAsync(teacherSubject);
            }
            else if (existingSubject.IsMainSubject != subjectDto.IsMainSubject)
            {
                // Cập nhật IsMainSubject nếu thay đổi
                existingSubject.IsMainSubject = subjectDto.IsMainSubject;
                await _teacherRepository.UpdateTeacherSubjectAsync(existingSubject);
            }
        }
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

    public async Task<(bool Success, List<string> Errors)> ImportTeachersFromExcelAsync(IFormFile file)
    {
        var errors = new List<string>();

        if (file == null || file.Length == 0)
        {
            errors.Add("Vui lòng chọn file Excel!");
            return (false, errors);
        }

        var data = ExcelImportHelper.ReadExcelData(file);
        var teachers = new Dictionary<string, Teacher>(); // Lưu giáo viên theo IdCardNumber
        var teacherSubjects = new List<(string IdCardNumber, TeacherSubject TeacherSubject)>(); // Lưu môn học kèm IdCardNumber

        foreach (var row in data)
        {
            // Kiểm tra thông tin bắt buộc
            if (string.IsNullOrEmpty(row["Họ và tên"]) || string.IsNullOrEmpty(row["Ngày sinh"]) ||
                string.IsNullOrEmpty(row["Giới tính"]) || string.IsNullOrEmpty(row["CMND/CCCD"]) ||
                string.IsNullOrEmpty(row["Ngày vào trường"]))
            {
                errors.Add($"Dòng dữ liệu cho giáo viên với CMND/CCCD {row["CMND/CCCD"]} thiếu thông tin bắt buộc.");
                continue;
            }

            var idCardNumber = row["CMND/CCCD"].Trim();

            // Nếu giáo viên chưa tồn tại trong Dictionary, tạo mới
            if (!teachers.TryGetValue(idCardNumber, out var teacher))
            {
                if (await _teacherRepository.ExistsAsync(idCardNumber))
                {
                    errors.Add($"Giáo viên với CMND/CCCD {idCardNumber} đã tồn tại trong hệ thống.");
                    continue;
                }

                if (await _teacherRepository.IsEmailOrPhoneExistsAsync(row["Email"], row["Số điện thoại"]))
                {
                    errors.Add($"Email {row["Email"]} hoặc số điện thoại {row["Số điện thoại"]} đã tồn tại.");
                    continue;
                }

                var username = await GenerateUniqueUsernameAsync(row["Họ và tên"]);
                var user = new User
                {
                    Email = row["Email"],
                    PhoneNumber = row["Số điện thoại"],
                    RoleId = 4,
                    Username = username,
                    PasswordHash = PasswordHasher.HashPassword("12345678"),
                    Status = "Active"
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
                    bool isMainSubject = subjectName.Contains("*");
                    if (isMainSubject)
                    {
                        subjectName = subjectName.Replace("*", "").Trim();
                    }

                    var subject = await _subjectRepository.GetByNameAsync(subjectName);
                    if (subject == null)
                    {
                        errors.Add($"Môn học '{subjectName}' không tồn tại trong bảng Subjects cho giáo viên với CMND/CCCD {idCardNumber}.");
                        continue;
                    }

                    // Lưu môn học kèm IdCardNumber để ánh xạ TeacherId sau
                    teacherSubjects.Add((idCardNumber, new TeacherSubject
                    {
                        TeacherId = 0, // Sẽ cập nhật sau
                        SubjectId = subject.SubjectId,
                        IsMainSubject = isMainSubject
                    }));
                }
            }
        }

        // Nếu có lỗi, trả về ngay
        if (errors.Any())
        {
            return (false, errors);
        }

        // Thêm tất cả giáo viên vào bảng Teachers
        await _teacherRepository.AddRangeAsync(teachers.Values);

        // Cập nhật TeacherId cho teacherSubjects
        foreach (var teacherSubject in teacherSubjects)
        {
            if (teachers.TryGetValue(teacherSubject.IdCardNumber, out var teacher))
            {
                teacherSubject.TeacherSubject.TeacherId = teacher.TeacherId;
            }
        }

        // Thêm tất cả TeacherSubjects
        if (teacherSubjects.Any())
        {
            await _teacherRepository.AddTeacherSubjectsRangeAsync(teacherSubjects.Select(ts => ts.TeacherSubject).ToList());
        }

        return (true, errors);
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