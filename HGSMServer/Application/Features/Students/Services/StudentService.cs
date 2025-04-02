using Application.Features.Parents.DTOs;
using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using AutoMapper;
using Common.Utils;
using DocumentFormat.OpenXml.InkML;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUserRepository _userRepository;
        private readonly IParentRepository _parentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IMapper _mapper;
        private readonly HgsdbContext _context;

        public StudentService(
    IStudentRepository studentRepository,
    IUserRepository userRepository,
    IParentRepository parentRepository,
    IClassRepository classRepository,
    IMapper mapper,
    HgsdbContext context)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _parentRepository = parentRepository ?? throw new ArgumentNullException(nameof(parentRepository));
            _classRepository = classRepository ?? throw new ArgumentNullException(nameof(classRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<StudentListResponseDto> GetAllStudentsWithParentsAsync(int academicYearId)
        {
            var students = await _studentRepository.GetAllWithParentsAsync(academicYearId);
            var studentDtos = new List<StudentDto>();

            foreach (var student in students)
            {
                var studentDto = _mapper.Map<StudentDto>(student);
                if (student.ParentId.HasValue)
                {
                    var parent = await _parentRepository.GetByIdAsync(student.ParentId.Value);
                    if (parent != null)
                    {
                        studentDto.Parent = _mapper.Map<ParentDto>(parent);
                    }
                }
                studentDtos.Add(studentDto);
            }

            return new StudentListResponseDto
            {
                Students = studentDtos,
                TotalCount = studentDtos.Count
            };
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id, int academicYearId)
        {
            var student = await _studentRepository.GetByIdWithParentsAsync(id, academicYearId);
            if (student == null)
                return null;

            var studentDto = _mapper.Map<StudentDto>(student);
            if (student.ParentId.HasValue)
            {
                var parent = await _parentRepository.GetByIdAsync(student.ParentId.Value);
                if (parent != null)
                {
                    studentDto.Parent = _mapper.Map<ParentDto>(parent);
                }
            }

            return studentDto;
        }

        public async Task<int> AddStudentAsync(CreateStudentDto createStudentDto)
        {
            if (createStudentDto == null)
                throw new ArgumentNullException(nameof(createStudentDto));

            Console.WriteLine($"Adding student: {createStudentDto.FullName}, IdcardNumber: {createStudentDto.IdcardNumber}");

            ValidateStudentDto(createStudentDto);

            using var transaction = await _studentRepository.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrWhiteSpace(createStudentDto.IdcardNumber))
                {
                    bool exists = await _studentRepository.ExistsAsync(createStudentDto.IdcardNumber);
                    if (exists)
                    {
                        Console.WriteLine($"IdcardNumber {createStudentDto.IdcardNumber} already exists.");
                        throw new Exception($"Số CMND/CCCD {createStudentDto.IdcardNumber} đã tồn tại.");
                    }
                }

                Console.WriteLine($"Checking if ClassId {createStudentDto.ClassId} exists...");
                if (!await _classRepository.ExistsAsync(createStudentDto.ClassId))
                {
                    Console.WriteLine($"ClassId {createStudentDto.ClassId} does not exist.");
                    throw new Exception($"ClassId {createStudentDto.ClassId} không tồn tại.");
                }

                Console.WriteLine("Mapping CreateStudentDto to Student...");
                var student = _mapper.Map<Student>(createStudentDto);
                student.StudentClasses = new List<StudentClass>
                {
                    new StudentClass
                    {
                        ClassId = createStudentDto.ClassId,
                        AcademicYearId = 1 // Giả sử AcademicYearId = 1
                    }
                };

                Console.WriteLine("Adding student to database...");
                await _studentRepository.AddAsync(student);
                Console.WriteLine($"Student added with ID: {student.StudentId}");

                if (!string.IsNullOrEmpty(createStudentDto.FullNameFather) ||
                    !string.IsNullOrEmpty(createStudentDto.FullNameMother) ||
                    !string.IsNullOrEmpty(createStudentDto.FullNameGuardian))
                {
                    Console.WriteLine("Processing parent information...");
                    await CreateParentAndLinkAsync(student, createStudentDto);
                }
                else
                {
                    Console.WriteLine($"Skipping parent creation for StudentID {student.StudentId}: No parent data provided.");
                }

                await transaction.CommitAsync();
                Console.WriteLine("Transaction committed successfully.");
                return student.StudentId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back due to error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                throw new Exception($"Lỗi khi thêm học sinh: {ex.Message}. Chi tiết: {ex.InnerException?.Message}");
            }
        }

        private async Task CreateParentAndLinkAsync(Student student, IParentInfoDto parentInfoDto)
        {
            Console.WriteLine($"Creating parent for StudentID {student.StudentId}");

            ValidateParentInfoDto(parentInfoDto);

            // Kiểm tra xem có thông tin liên lạc nào hợp lệ không
            bool hasFatherContact = !string.IsNullOrEmpty(parentInfoDto.PhoneNumberFather) || !string.IsNullOrEmpty(parentInfoDto.EmailFather);
            bool hasMotherContact = !string.IsNullOrEmpty(parentInfoDto.PhoneNumberMother) || !string.IsNullOrEmpty(parentInfoDto.EmailMother);
            bool hasGuardianContact = !string.IsNullOrEmpty(parentInfoDto.PhoneNumberGuardian) || !string.IsNullOrEmpty(parentInfoDto.EmailGuardian);

            if (!hasFatherContact && !hasMotherContact && !hasGuardianContact)
            {
                Console.WriteLine($"Không có thông tin liên lạc hợp lệ cho phụ huynh của StudentID {student.StudentId}. Bỏ qua việc tạo phụ huynh.");
                return;
            }

            // Tìm phụ huynh hiện có
            Parent existingParent = null;
            if (hasFatherContact)
            {
                existingParent = await _parentRepository.GetParentByDetailsAsync(
                    parentInfoDto.FullNameFather, parentInfoDto.YearOfBirthFather,
                    parentInfoDto.PhoneNumberFather, parentInfoDto.EmailFather, parentInfoDto.IdcardNumberFather);
            }
            else if (hasMotherContact)
            {
                existingParent = await _parentRepository.GetParentByDetailsAsync(
                    parentInfoDto.FullNameMother, parentInfoDto.YearOfBirthMother,
                    parentInfoDto.PhoneNumberMother, parentInfoDto.EmailMother, parentInfoDto.IdcardNumberMother);
            }
            else if (hasGuardianContact)
            {
                existingParent = await _parentRepository.GetParentByDetailsAsync(
                    parentInfoDto.FullNameGuardian, parentInfoDto.YearOfBirthGuardian,
                    parentInfoDto.PhoneNumberGuardian, parentInfoDto.EmailGuardian, parentInfoDto.IdcardNumberGuardian);
            }

            if (existingParent != null)
            {
                Console.WriteLine($"Phụ huynh đã tồn tại với ParentID: {existingParent.ParentId}. Liên kết với học sinh...");
                student.ParentId = existingParent.ParentId;
                return;
            }

            // Tạo user và parent mới
            string phoneNumber = parentInfoDto.PhoneNumberFather ?? parentInfoDto.PhoneNumberMother ?? parentInfoDto.PhoneNumberGuardian;
            string email = parentInfoDto.EmailFather ?? parentInfoDto.EmailMother ?? parentInfoDto.EmailGuardian;

            var user = new User
            {
                Username = $"temp_{Guid.NewGuid().ToString().Substring(0, 8)}",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"),
                Email = email,
                PhoneNumber = phoneNumber,
                RoleId = 6,
                Status = "Active"
            };

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(user.PhoneNumber);
                if (existingUserByPhone != null)
                {
                    throw new Exception($"Số điện thoại {user.PhoneNumber} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByPhone.UserId}).");
                }
            }
            if (!string.IsNullOrEmpty(user.Email))
            {
                var existingUserByEmail = await _userRepository.GetByEmailAsync(user.Email);
                if (existingUserByEmail != null)
                {
                    throw new Exception($"Email {user.Email} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByEmail.UserId}).");
                }
            }

            await _userRepository.AddAsync(user);
            Console.WriteLine($"Tạo user mới với UserID: {user.UserId}");

            string fullNameForUsername = parentInfoDto.FullNameFather ?? parentInfoDto.FullNameMother ?? parentInfoDto.FullNameGuardian ?? "user";
            string finalUsername = FormatUserName.GenerateUsername(fullNameForUsername, user.UserId);
            var existingUserByUsername = await _userRepository.GetByUsernameAsync(finalUsername);
            if (existingUserByUsername != null)
            {
                finalUsername += $"_{user.UserId}";
            }
            user.Username = finalUsername;
            await _userRepository.UpdateAsync(user);
            Console.WriteLine($"Cập nhật user với Username: {user.Username}");

            var parent = new Parent
            {
                UserId = user.UserId,
                FullNameFather = parentInfoDto.FullNameFather,
                YearOfBirthFather = parentInfoDto.YearOfBirthFather,
                OccupationFather = parentInfoDto.OccupationFather ?? "Unknown",
                PhoneNumberFather = parentInfoDto.PhoneNumberFather,
                EmailFather = parentInfoDto.EmailFather,
                IdcardNumberFather = parentInfoDto.IdcardNumberFather,
                FullNameMother = parentInfoDto.FullNameMother,
                YearOfBirthMother = parentInfoDto.YearOfBirthMother,
                OccupationMother = parentInfoDto.OccupationMother ?? "Unknown",
                PhoneNumberMother = parentInfoDto.PhoneNumberMother,
                EmailMother = parentInfoDto.EmailMother,
                IdcardNumberMother = parentInfoDto.IdcardNumberMother,
                FullNameGuardian = parentInfoDto.FullNameGuardian,
                YearOfBirthGuardian = parentInfoDto.YearOfBirthGuardian,
                OccupationGuardian = parentInfoDto.OccupationGuardian ?? "Unknown",
                PhoneNumberGuardian = parentInfoDto.PhoneNumberGuardian,
                EmailGuardian = parentInfoDto.EmailGuardian,
                IdcardNumberGuardian = parentInfoDto.IdcardNumberGuardian
            };

            await _parentRepository.AddAsync(parent);
            Console.WriteLine($"Tạo phụ huynh mới với ParentID: {parent.ParentId}");

            student.ParentId = parent.ParentId; // Chỉ gán ParentId, không thêm hoặc cập nhật student
            Console.WriteLine($"Liên kết StudentID với ParentID {parent.ParentId}");
        }

        private void ValidateStudentDto(CreateStudentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentException("Họ và tên không được để trống.");
            if (dto.Dob == default || dto.Dob > DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("Ngày sinh không hợp lệ.");
            if (string.IsNullOrWhiteSpace(dto.Gender) || !new[] { "Nam", "Nữ", "Khác" }.Contains(dto.Gender))
                throw new ArgumentException("Giới tính không hợp lệ. Phải là 'Nam', 'Nữ' hoặc 'Khác'.");
            if (dto.AdmissionDate == default || dto.AdmissionDate > DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("Ngày nhập học không hợp lệ.");
            if (!string.IsNullOrWhiteSpace(dto.IdcardNumber) && !IsValidIdCardNumber(dto.IdcardNumber))
                throw new ArgumentException("Số CMND/CCCD không hợp lệ.");
        }

        private void ValidateParentInfoDto(IParentInfoDto dto)
        {
            if (!string.IsNullOrEmpty(dto.EmailFather) && !IsValidEmail(dto.EmailFather))
                throw new ArgumentException("EmailFather không hợp lệ.");
            if (!string.IsNullOrEmpty(dto.PhoneNumberFather) && !IsValidPhoneNumber(dto.PhoneNumberFather))
                throw new ArgumentException("PhoneNumberFather không hợp lệ.");
            if (!string.IsNullOrEmpty(dto.IdcardNumberFather) && !IsValidIdCardNumber(dto.IdcardNumberFather))
                throw new ArgumentException("IdcardNumberFather không hợp lệ.");

            if (!string.IsNullOrEmpty(dto.EmailMother) && !IsValidEmail(dto.EmailMother))
                throw new ArgumentException("EmailMother không hợp lệ.");
            if (!string.IsNullOrEmpty(dto.PhoneNumberMother) && !IsValidPhoneNumber(dto.PhoneNumberMother))
                throw new ArgumentException("PhoneNumberMother không hợp lệ.");
            if (!string.IsNullOrEmpty(dto.IdcardNumberMother) && !IsValidIdCardNumber(dto.IdcardNumberMother))
                throw new ArgumentException("IdcardNumberMother không hợp lệ.");

            if (!string.IsNullOrEmpty(dto.EmailGuardian) && !IsValidEmail(dto.EmailGuardian))
                throw new ArgumentException("EmailGuardian không hợp lệ.");
            if (!string.IsNullOrEmpty(dto.PhoneNumberGuardian) && !IsValidPhoneNumber(dto.PhoneNumberGuardian))
                throw new ArgumentException("PhoneNumberGuardian không hợp lệ.");
            if (!string.IsNullOrEmpty(dto.IdcardNumberGuardian) && !IsValidIdCardNumber(dto.IdcardNumberGuardian))
                throw new ArgumentException("IdcardNumberGuardian không hợp lệ.");
        }

        private bool IsValidIdCardNumber(string idCardNumber)
        {
            return string.IsNullOrEmpty(idCardNumber) || System.Text.RegularExpressions.Regex.IsMatch(idCardNumber, @"^\d{12}$");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^0\d{9}$");
        }

        public async Task UpdateStudentAsync(UpdateStudentDto updateStudentDto)
        {
            Console.WriteLine($"Updating student with StudentID: {updateStudentDto.StudentId}");

            var student = await _studentRepository.GetByIdAsync(updateStudentDto.StudentId);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {updateStudentDto.StudentId} not found");
            }

            using var transaction = await _studentRepository.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrWhiteSpace(updateStudentDto.IdcardNumber) && updateStudentDto.IdcardNumber != student.IdcardNumber)
                {
                    bool exists = await _studentRepository.ExistsAsync(updateStudentDto.IdcardNumber);
                    if (exists)
                    {
                        throw new Exception($"Số CMND/CCCD {updateStudentDto.IdcardNumber} đã tồn tại.");
                    }
                }

                var currentClass = student.StudentClasses?.FirstOrDefault();
                if (currentClass == null || currentClass.ClassId != updateStudentDto.ClassId)
                {
                    if (!await _classRepository.ExistsAsync(updateStudentDto.ClassId))
                    {
                        throw new Exception($"ClassId {updateStudentDto.ClassId} không tồn tại.");
                    }
                    if (currentClass != null)
                    {
                        currentClass.ClassId = updateStudentDto.ClassId;
                    }
                    else
                    {
                        student.StudentClasses = new List<StudentClass>
                        {
                            new StudentClass
                            {
                                StudentId = student.StudentId,
                                ClassId = updateStudentDto.ClassId,
                                AcademicYearId = 1
                            }
                        };
                    }
                }

                _mapper.Map(updateStudentDto, student);
                await _studentRepository.UpdateAsync(student);
                Console.WriteLine($"Updated student with StudentID: {student.StudentId}");

                if (!string.IsNullOrEmpty(updateStudentDto.FullNameFather) ||
                    !string.IsNullOrEmpty(updateStudentDto.FullNameMother) ||
                    !string.IsNullOrEmpty(updateStudentDto.FullNameGuardian))
                {
                    await UpdateOrCreateParentAsync(student, updateStudentDto);
                }

                await transaction.CommitAsync();
                Console.WriteLine("Transaction committed successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back due to error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                throw new Exception($"Lỗi khi cập nhật học sinh: {ex.Message}. Chi tiết: {ex.InnerException?.Message}");
            }
        }

        private async Task UpdateOrCreateParentAsync(Student student, UpdateStudentDto updateStudentDto)
        {
            Console.WriteLine($"Updating/Creating parent for StudentID {student.StudentId}");

            if (student.ParentId.HasValue)
            {
                var parent = await _parentRepository.GetByIdAsync(student.ParentId.Value);
                if (parent != null)
                {
                    var user = await _userRepository.GetByIdAsync(parent.UserId);
                    if (user != null)
                    {
                        string newPhoneNumber = updateStudentDto.PhoneNumberFather ?? updateStudentDto.PhoneNumberMother ?? updateStudentDto.PhoneNumberGuardian;
                        string newEmail = updateStudentDto.EmailFather ?? updateStudentDto.EmailMother ?? updateStudentDto.EmailGuardian;

                        if (newPhoneNumber != user.PhoneNumber && !string.IsNullOrEmpty(newPhoneNumber))
                        {
                            var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(newPhoneNumber);
                            if (existingUserByPhone != null && existingUserByPhone.UserId != user.UserId)
                            {
                                throw new Exception($"Số điện thoại {newPhoneNumber} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByPhone.UserId}).");
                            }
                            user.PhoneNumber = newPhoneNumber;
                        }

                        if (newEmail != user.Email && !string.IsNullOrEmpty(newEmail))
                        {
                            var existingUserByEmail = await _userRepository.GetByEmailAsync(newEmail);
                            if (existingUserByEmail != null && existingUserByEmail.UserId != user.UserId)
                            {
                                throw new Exception($"Email {newEmail} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByEmail.UserId}).");
                            }
                            user.Email = newEmail;
                        }

                        string fullNameForUsername = updateStudentDto.FullNameFather ?? updateStudentDto.FullNameMother ?? updateStudentDto.FullNameGuardian ?? "user";
                        string newUsername = FormatUserName.GenerateUsername(fullNameForUsername, user.UserId);
                        if (user.Username != newUsername)
                        {
                            var existingUserByUsername = await _userRepository.GetByUsernameAsync(newUsername);
                            if (existingUserByUsername != null && existingUserByUsername.UserId != user.UserId)
                            {
                                newUsername += $"_{user.UserId}";
                            }
                            user.Username = newUsername;
                        }

                        await _userRepository.UpdateAsync(user);
                        Console.WriteLine($"Updated user with UserID: {user.UserId}");

                        parent.FullNameFather = updateStudentDto.FullNameFather;
                        parent.YearOfBirthFather = updateStudentDto.YearOfBirthFather;
                        parent.OccupationFather = updateStudentDto.OccupationFather ?? "Unknown";
                        parent.PhoneNumberFather = updateStudentDto.PhoneNumberFather;
                        parent.EmailFather = updateStudentDto.EmailFather;
                        parent.IdcardNumberFather = updateStudentDto.IdcardNumberFather;
                        parent.FullNameMother = updateStudentDto.FullNameMother;
                        parent.YearOfBirthMother = updateStudentDto.YearOfBirthMother;
                        parent.OccupationMother = updateStudentDto.OccupationMother ?? "Unknown";
                        parent.PhoneNumberMother = updateStudentDto.PhoneNumberMother;
                        parent.EmailMother = updateStudentDto.EmailMother;
                        parent.IdcardNumberMother = updateStudentDto.IdcardNumberMother;
                        parent.FullNameGuardian = updateStudentDto.FullNameGuardian;
                        parent.YearOfBirthGuardian = updateStudentDto.YearOfBirthGuardian;
                        parent.OccupationGuardian = updateStudentDto.OccupationGuardian ?? "Unknown";
                        parent.PhoneNumberGuardian = updateStudentDto.PhoneNumberGuardian;
                        parent.EmailGuardian = updateStudentDto.EmailGuardian;
                        parent.IdcardNumberGuardian = updateStudentDto.IdcardNumberGuardian;

                        await _parentRepository.UpdateAsync(parent);
                        Console.WriteLine($"Updated parent with ParentID: {parent.ParentId}");
                    }
                }
            }
            else
            {
                await CreateParentAndLinkAsync(student, updateStudentDto);
            }
        }

        public async Task DeleteStudentAsync(int id)
        {
            Console.WriteLine($"Deleting student with StudentID: {id}");
            await _studentRepository.DeleteAsync(id);
            Console.WriteLine($"Deleted student with StudentID: {id}");
        }

        public async Task<List<string>> ImportStudentsFromExcelAsync(IFormFile file)
        {
            Console.WriteLine("Importing students from Excel file");

            if (file == null || file.Length == 0)
                throw new ArgumentException("Vui lòng chọn file Excel!");

            var data = ExcelImportHelper.ReadExcelData(file);
            var students = new List<Student>();
            var importResults = new List<string>();

            Console.WriteLine("Dữ liệu đọc từ Excel:");
            foreach (var row in data)
            {
                Console.WriteLine(string.Join(", ", row.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
            }

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var currentAcademicYear = await _studentRepository.GetCurrentAcademicYearAsync(currentDate);
            if (currentAcademicYear == null)
                throw new Exception("Không tìm thấy năm học hiện tại.");

            using var transaction = await _studentRepository.BeginTransactionAsync();
            try
            {
                foreach (var row in data)
                {
                    string fullName = null;
                    try
                    {
                        if (!row.TryGetValue("Họ và tên", out fullName) || string.IsNullOrEmpty(fullName))
                        {
                            importResults.Add($"Lỗi: Thiếu hoặc rỗng 'Họ và tên' trong dòng: {string.Join(", ", row.Values)}");
                            continue;
                        }

                        if (!row.TryGetValue("Ngày sinh", out var dobStr) || string.IsNullOrEmpty(dobStr))
                        {
                            importResults.Add($"Lỗi: Thiếu hoặc rỗng 'Ngày sinh' trong dòng: {fullName}");
                            continue;
                        }

                        if (!row.TryGetValue("Giới tính", out var gender) || string.IsNullOrEmpty(gender))
                        {
                            importResults.Add($"Lỗi: Thiếu hoặc rỗng 'Giới tính' trong dòng: {fullName}");
                            continue;
                        }

                        if (!row.TryGetValue("Ngày nhập học", out var admissionDateStr) || string.IsNullOrEmpty(admissionDateStr))
                        {
                            importResults.Add($"Lỗi: Thiếu hoặc rỗng 'Ngày nhập học' trong dòng: {fullName}");
                            continue;
                        }

                        if (!row.TryGetValue("Tên lớp", out var className) || string.IsNullOrEmpty(className))
                        {
                            importResults.Add($"Lỗi: Thiếu hoặc rỗng 'Tên lớp' trong dòng: {fullName}");
                            continue;
                        }

                        string idCardNumber = null;
                        if (!row.TryGetValue("Số CMND/CCCD", out idCardNumber) && !row.TryGetValue("Số CMND", out idCardNumber))
                        {
                            idCardNumber = null;
                        }

                        // Parse ngày sinh và ngày nhập học
                        Console.WriteLine($"Parsing Dob for {fullName}: {dobStr}");
                        var dob = DateHelper.ParseDate(dobStr);

                        Console.WriteLine($"Parsing AdmissionDate for {fullName}: {admissionDateStr}");
                        var admissionDate = DateHelper.ParseDate(admissionDateStr);

                        var validClasses = new[] { "6A", "6B", "7A", "7B", "7C", "8A", "8B", "9A", "9B" };
                        if (!validClasses.Contains(className.ToUpper()))
                            throw new Exception($"Tên lớp '{className}' không hợp lệ. Phải là một trong: {string.Join(", ", validClasses)}");

                        var classEntity = await _classRepository.GetClassByNameAsync(className.ToUpper());
                        if (classEntity == null)
                            throw new Exception($"Không tìm thấy lớp '{className}' trong hệ thống.");

                        if (!string.IsNullOrEmpty(idCardNumber) && await _studentRepository.ExistsAsync(idCardNumber.Trim()))
                            throw new Exception($"Số CMND/CCCD {idCardNumber} đã tồn tại.");

                        if (!string.IsNullOrEmpty(idCardNumber) && !IsValidIdCardNumber(idCardNumber))
                            throw new Exception($"Số CMND/CCCD {idCardNumber} không hợp lệ (phải là 12 chữ số).");

                        var student = new Student
                        {
                            FullName = fullName.Trim(),
                            Dob = dob,
                            Gender = gender.Trim(),
                            AdmissionDate = admissionDate,
                            EnrollmentType = row.TryGetValue("Hình thức nhập học", out var enrollmentType) ? enrollmentType.Trim() : null,
                            Ethnicity = row.TryGetValue("Dân tộc", out var ethnicity) ? ethnicity.Trim() : null,
                            PermanentAddress = row.TryGetValue("Địa chỉ thường trú", out var permAddress) ? permAddress.Trim() : null,
                            BirthPlace = row.TryGetValue("Nơi sinh", out var birthPlace) ? birthPlace.Trim() : null,
                            Religion = row.TryGetValue("Tôn giáo", out var religion) ? religion.Trim() : null,
                            RepeatingYear = row.TryGetValue("Lưu ban", out var repeating) && repeating.Trim().ToLower() == "có",
                            IdcardNumber = idCardNumber?.Trim(),
                            Status = row.TryGetValue("Trạng thái", out var status) ? status.Trim() : "Đang học",
                            StudentClasses = new List<StudentClass>
                            {
                                new StudentClass
                                {
                                    ClassId = classEntity.ClassId,
                                    AcademicYearId = currentAcademicYear.AcademicYearId
                                }
                            }
                        };

                        bool hasParentInfo = row.Any(kvp => kvp.Key.Contains("cha") || kvp.Key.Contains("mẹ") || kvp.Key.Contains("người bảo hộ"));
                        if (hasParentInfo)
                        {
                            Console.WriteLine($"Processing parent information for student {student.FullName}...");

                            var parentInfoDto = new ParentInfoDto
                            {
                                FullNameFather = row.TryGetValue("Họ và tên cha", out var fatherName) ? fatherName.Trim() : null,
                                YearOfBirthFather = row.TryGetValue("Ngày sinh cha", out var fatherDobStr) && !string.IsNullOrEmpty(fatherDobStr) ? DateHelper.ParseDate(fatherDobStr) : null,
                                OccupationFather = row.TryGetValue("Nghề nghiệp cha", out var fatherOcc) ? fatherOcc.Trim() : null,
                                PhoneNumberFather = row.TryGetValue("SĐT cha", out var fatherPhone) ? fatherPhone.Trim() : null,
                                EmailFather = row.TryGetValue("Email cha", out var fatherEmail) ? fatherEmail.Trim() : null,
                                IdcardNumberFather = row.TryGetValue("Số CCCD cha", out var fatherIdCard) ? fatherIdCard.Trim() : null,
                                FullNameMother = row.TryGetValue("Họ và tên mẹ", out var motherName) ? motherName.Trim() : null,
                                YearOfBirthMother = row.TryGetValue("Ngày sinh mẹ", out var motherDobStr) && !string.IsNullOrEmpty(motherDobStr) ? DateHelper.ParseDate(motherDobStr) : null,
                                OccupationMother = row.TryGetValue("Nghề nghiệp mẹ", out var motherOcc) ? motherOcc.Trim() : null,
                                PhoneNumberMother = row.TryGetValue("SĐT mẹ", out var motherPhone) ? motherPhone.Trim() : null,
                                EmailMother = row.TryGetValue("Email mẹ", out var motherEmail) ? motherEmail.Trim() : null,
                                IdcardNumberMother = row.TryGetValue("Số CCCD mẹ", out var motherIdCard) ? motherIdCard.Trim() : null,
                                FullNameGuardian = row.TryGetValue("Họ và tên người bảo hộ", out var guardianName) ? guardianName.Trim() : null,
                                YearOfBirthGuardian = row.TryGetValue("Ngày sinh người bảo hộ", out var guardianDobStr) && !string.IsNullOrEmpty(guardianDobStr) ? DateHelper.ParseDate(guardianDobStr) : null,
                                OccupationGuardian = row.TryGetValue("Nghề nghiệp người bảo hộ", out var guardianOcc) ? guardianOcc.Trim() : null,
                                PhoneNumberGuardian = row.TryGetValue("SĐT người bảo hộ", out var guardianPhone) ? guardianPhone.Trim() : null,
                                EmailGuardian = row.TryGetValue("Email người bảo hộ", out var guardianEmail) ? guardianEmail.Trim() : null,
                                IdcardNumberGuardian = row.TryGetValue("Số CCCD người bảo hộ", out var guardianIdCard) ? guardianIdCard.Trim() : null
                            };

                            bool hasCompleteFatherInfo = !string.IsNullOrEmpty(parentInfoDto.FullNameFather) &&
                                                         parentInfoDto.YearOfBirthFather.HasValue &&
                                                         !string.IsNullOrEmpty(parentInfoDto.OccupationFather) &&
                                                         !string.IsNullOrEmpty(parentInfoDto.PhoneNumberFather) &&
                                                         !string.IsNullOrEmpty(parentInfoDto.IdcardNumberFather);

                            bool hasCompleteMotherInfo = !string.IsNullOrEmpty(parentInfoDto.FullNameMother) &&
                                                         parentInfoDto.YearOfBirthMother.HasValue &&
                                                         !string.IsNullOrEmpty(parentInfoDto.OccupationMother) &&
                                                         !string.IsNullOrEmpty(parentInfoDto.PhoneNumberMother) &&
                                                         !string.IsNullOrEmpty(parentInfoDto.IdcardNumberMother);

                            bool hasCompleteGuardianInfo = !string.IsNullOrEmpty(parentInfoDto.FullNameGuardian) &&
                                                           parentInfoDto.YearOfBirthGuardian.HasValue &&
                                                           !string.IsNullOrEmpty(parentInfoDto.OccupationGuardian) &&
                                                           !string.IsNullOrEmpty(parentInfoDto.PhoneNumberGuardian) &&
                                                           !string.IsNullOrEmpty(parentInfoDto.IdcardNumberGuardian);

                            if (hasCompleteFatherInfo || hasCompleteMotherInfo || hasCompleteGuardianInfo)
                            {
                                await CreateParentAndLinkAsync(student, parentInfoDto);
                            }
                            else
                            {
                                Console.WriteLine($"Không đủ thông tin để tạo phụ huynh cho {student.FullName}. Bỏ qua việc tạo phụ huynh.");
                            }
                        }

                        students.Add(student);
                        importResults.Add($"Đã thêm học sinh: {student.FullName} (CMND/CCCD: {student.IdcardNumber ?? "Không có"}, Lớp: {className})");
                    }
                    catch (Exception ex)
                    {
                        importResults.Add($"Lỗi khi xử lý dòng '{fullName ?? "Không xác định"}': {ex.Message}");
                    }
                }

                if (students.Any())
                {
                    Console.WriteLine($"Adding {students.Count} students to database...");
                    await _studentRepository.AddRangeAsync(students);
                }

                await transaction.CommitAsync();
                Console.WriteLine($"Imported {students.Count} students from Excel file");
                return importResults;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back due to error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                throw new Exception($"Lỗi khi import học sinh: {ex.Message}. Chi tiết: {ex.InnerException?.Message}");
            }
        }
    }
}


