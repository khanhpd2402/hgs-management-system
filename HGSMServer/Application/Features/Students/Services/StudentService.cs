using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using AutoMapper;
using Common.Utils;
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

        public StudentService(
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            IParentRepository parentRepository,
            IClassRepository classRepository,
            IMapper mapper)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _parentRepository = parentRepository ?? throw new ArgumentNullException(nameof(parentRepository));
            _classRepository = classRepository ?? throw new ArgumentNullException(nameof(classRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            if (createStudentDto == null)
                throw new ArgumentNullException(nameof(createStudentDto));

            Console.WriteLine($"Adding student: {createStudentDto.FullName}, IdcardNumber: {createStudentDto.IdcardNumber}");

            // Kiểm tra tính hợp lệ của các trường
            ValidateStudentDto(createStudentDto);

            using var transaction = await _studentRepository.BeginTransactionAsync();
            try
            {
                // Kiểm tra trùng lặp IdcardNumber
                if (!string.IsNullOrWhiteSpace(createStudentDto.IdcardNumber))
                {
                    bool exists = await _studentRepository.ExistsAsync(createStudentDto.IdcardNumber);
                    if (exists)
                    {
                        Console.WriteLine($"IdcardNumber {createStudentDto.IdcardNumber} already exists.");
                        throw new Exception($"Số CMND/CCCD {createStudentDto.IdcardNumber} đã tồn tại.");
                    }
                }

                // Kiểm tra ClassId
                Console.WriteLine($"Checking if ClassId {createStudentDto.ClassId} exists...");
                if (!await _classRepository.ExistsAsync(createStudentDto.ClassId))
                {
                    Console.WriteLine($"ClassId {createStudentDto.ClassId} does not exist.");
                    throw new Exception($"ClassId {createStudentDto.ClassId} không tồn tại.");
                }

                // Tạo Student
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

                // Xử lý thông tin cha mẹ
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
                Console.WriteLine($"Transaction rolled back due to error: {ex.Message}");
                throw;
            }
        }

        private async Task CreateParentAndLinkAsync(Student student, IParentInfoDto parentInfoDto)
        {
            Console.WriteLine($"Creating parent for StudentID {student.StudentId}");

            // Kiểm tra tính hợp lệ của các trường cha mẹ
            ValidateParentInfoDto(parentInfoDto);

            // Kiểm tra xem cha mẹ đã tồn tại chưa (dựa trên Email hoặc PhoneNumber)
            Parent existingParent = null;
            if (!string.IsNullOrEmpty(parentInfoDto.EmailFather))
            {
                existingParent = await _parentRepository.GetParentByDetailsAsync(
                    parentInfoDto.FullNameFather, parentInfoDto.YearOfBirthFather,
                    parentInfoDto.PhoneNumberFather, parentInfoDto.EmailFather, parentInfoDto.IdcardNumberFather);
            }
            else if (!string.IsNullOrEmpty(parentInfoDto.EmailMother))
            {
                existingParent = await _parentRepository.GetParentByDetailsAsync(
                    parentInfoDto.FullNameMother, parentInfoDto.YearOfBirthMother,
                    parentInfoDto.PhoneNumberMother, parentInfoDto.EmailMother, parentInfoDto.IdcardNumberMother);
            }
            else if (!string.IsNullOrEmpty(parentInfoDto.EmailGuardian))
            {
                existingParent = await _parentRepository.GetParentByDetailsAsync(
                    parentInfoDto.FullNameGuardian, parentInfoDto.YearOfBirthGuardian,
                    parentInfoDto.PhoneNumberGuardian, parentInfoDto.EmailGuardian, parentInfoDto.IdcardNumberGuardian);
            }

            if (existingParent != null)
            {
                Console.WriteLine($"Parent already exists with ParentID: {existingParent.ParentId}. Linking to student...");
                student.ParentId = existingParent.ParentId;
                await _studentRepository.UpdateAsync(student);
                Console.WriteLine($"Linked StudentID {student.StudentId} with existing ParentID {existingParent.ParentId}");
                return;
            }

            // Nếu cha mẹ chưa tồn tại, tạo mới
            var user = new User
            {
                Username = "parenttemp", // Username tạm thời
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"),
                Email = parentInfoDto.EmailFather ?? parentInfoDto.EmailMother ?? parentInfoDto.EmailGuardian,
                PhoneNumber = parentInfoDto.PhoneNumberFather ?? parentInfoDto.PhoneNumberMother ?? parentInfoDto.PhoneNumberGuardian,
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

            Console.WriteLine("Adding new user with temporary username...");
            await _userRepository.AddAsync(user);
            Console.WriteLine($"Created new user with UserID: {user.UserId}");

            string roleName = "Parent";
            string finalUsername = $"{roleName.ToLower()}{user.UserId}";
            var existingUserByUsername = await _userRepository.GetByUsernameAsync(finalUsername);
            if (existingUserByUsername != null)
            {
                throw new Exception($"Username {finalUsername} đã tồn tại (UserID: {existingUserByUsername.UserId}).");
            }
            user.Username = finalUsername;
            await _userRepository.UpdateAsync(user);
            Console.WriteLine($"Updated user with Username: {user.Username}");

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

            Console.WriteLine("Adding new parent to database...");
            await _parentRepository.AddAsync(parent);
            Console.WriteLine($"Created new parent with ParentID: {parent.ParentId}");

            student.ParentId = parent.ParentId;
            await _studentRepository.UpdateAsync(student);
            Console.WriteLine($"Linked StudentID {student.StudentId} with ParentID {parent.ParentId}");
        }

        // Hàm kiểm tra tính hợp lệ của Student DTO
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

        // Hàm kiểm tra tính hợp lệ của Parent DTO
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

        // Hàm kiểm tra tính hợp lệ của ID card (ví dụ: 12 chữ số)
        private bool IsValidIdCardNumber(string idCardNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(idCardNumber, @"^\d{12}$");
        }

        private string ExtractLastName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return "";

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? RemoveDiacritics(parts[^1]) : "";
        }

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
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
                Console.WriteLine($"Transaction rolled back due to error: {ex.Message}");
                throw;
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

                        string lastNameFather = ExtractLastName(updateStudentDto.FullNameFather);
                        string lastNameMother = ExtractLastName(updateStudentDto.FullNameMother);
                        string lastNameGuardian = ExtractLastName(updateStudentDto.FullNameGuardian);
                        string combinedLastName = $"{lastNameFather}{lastNameMother}{lastNameGuardian}".ToLower();
                        string newUsername = $"{combinedLastName}{user.UserId}";
                        if (user.Username != newUsername)
                        {
                            var existingUserByUsername = await _userRepository.GetByUsernameAsync(newUsername);
                            if (existingUserByUsername != null && existingUserByUsername.UserId != user.UserId)
                            {
                                throw new Exception($"Username {newUsername} đã tồn tại (UserID: {existingUserByUsername.UserId}).");
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

        public async Task ImportStudentsFromExcelAsync(IFormFile file)
        {
            Console.WriteLine("Importing students from Excel file");

            var data = ExcelImportHelper.ReadExcelData(file);
            var students = new List<Student>();

            using var transaction = await _studentRepository.BeginTransactionAsync();
            try
            {
                foreach (var row in data)
                {
                    // Kiểm tra IdcardNumber của Student
                    string idCardNumber = row["Số CMND/CCCD"]?.ToString().Trim();
                    if (string.IsNullOrWhiteSpace(idCardNumber))
                    {
                        throw new Exception("Số CMND/CCCD không được để trống.");
                    }

                    bool exists = await _studentRepository.ExistsAsync(idCardNumber);
                    if (exists)
                    {
                        throw new Exception($"Số CMND/CCCD {idCardNumber} đã tồn tại.");
                    }

                    if (!IsValidIdCardNumber(idCardNumber))
                    {
                        throw new Exception($"Số CMND/CCCD {idCardNumber} không hợp lệ.");
                    }

                    // Tạo Student từ dữ liệu Excel
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
                        IdcardNumber = idCardNumber,
                        Status = row["Trạng thái"]?.ToString().Trim() ?? "Đang học",
                        StudentClasses = new List<StudentClass>
                {
                    new StudentClass
                    {
                        ClassId = 1,
                        AcademicYearId = 1
                    }
                }
                    };

                    // Kiểm tra tính hợp lệ của Student
                    if (student.Dob == default || student.Dob > DateOnly.FromDateTime(DateTime.Now))
                        throw new ArgumentException($"Ngày sinh của {student.FullName} không hợp lệ.");
                    if (!new[] { "Nam", "Nữ", "Khác" }.Contains(student.Gender))
                        throw new ArgumentException($"Giới tính của {student.FullName} không hợp lệ. Phải là 'Nam', 'Nữ' hoặc 'Khác'.");
                    if (student.AdmissionDate == default || student.AdmissionDate > DateOnly.FromDateTime(DateTime.Now))
                        throw new ArgumentException($"Ngày nhập học của {student.FullName} không hợp lệ.");

                    // Kiểm tra thông tin cha mẹ từ Excel
                    bool hasParentInfo = !string.IsNullOrEmpty(row["FullNameFather"]?.ToString().Trim()) ||
                                        !string.IsNullOrEmpty(row["FullNameMother"]?.ToString().Trim()) ||
                                        !string.IsNullOrEmpty(row["FullNameGuardian"]?.ToString().Trim());

                    if (hasParentInfo)
                    {
                        Console.WriteLine($"Processing parent information for student {student.FullName}...");

                        // Kiểm tra tính hợp lệ của thông tin cha mẹ
                        var parentInfoDto = new ParentInfoDto
                        {
                            FullNameFather = row["FullNameFather"]?.ToString().Trim(),
                            YearOfBirthFather = DateHelper.ParseDate(row["YearOfBirthFather"]?.ToString()),
                            OccupationFather = row["OccupationFather"]?.ToString().Trim(),
                            PhoneNumberFather = row["PhoneNumberFather"]?.ToString().Trim(),
                            EmailFather = row["EmailFather"]?.ToString().Trim(),
                            IdcardNumberFather = row["IdcardNumberFather"]?.ToString().Trim(),
                            FullNameMother = row["FullNameMother"]?.ToString().Trim(),
                            YearOfBirthMother = DateHelper.ParseDate(row["YearOfBirthMother"]?.ToString()),
                            OccupationMother = row["OccupationMother"]?.ToString().Trim(),
                            PhoneNumberMother = row["PhoneNumberMother"]?.ToString().Trim(),
                            EmailMother = row["EmailMother"]?.ToString().Trim(),
                            IdcardNumberMother = row["IdcardNumberMother"]?.ToString().Trim(),
                            FullNameGuardian = row["FullNameGuardian"]?.ToString().Trim(),
                            YearOfBirthGuardian = DateHelper.ParseDate(row["YearOfBirthGuardian"]?.ToString()),
                            OccupationGuardian = row["OccupationGuardian"]?.ToString().Trim(),
                            PhoneNumberGuardian = row["PhoneNumberGuardian"]?.ToString().Trim(),
                            EmailGuardian = row["EmailGuardian"]?.ToString().Trim(),
                            IdcardNumberGuardian = row["IdcardNumberGuardian"]?.ToString().Trim()
                        };

                        ValidateParentInfoDto(parentInfoDto);

                        // Kiểm tra xem cha mẹ đã tồn tại chưa
                        Parent existingParent = null;
                        if (!string.IsNullOrEmpty(parentInfoDto.EmailFather))
                        {
                            existingParent = await _parentRepository.GetParentByDetailsAsync(
                                parentInfoDto.FullNameFather, parentInfoDto.YearOfBirthFather,
                                parentInfoDto.PhoneNumberFather, parentInfoDto.EmailFather, parentInfoDto.IdcardNumberFather);
                        }
                        else if (!string.IsNullOrEmpty(parentInfoDto.EmailMother))
                        {
                            existingParent = await _parentRepository.GetParentByDetailsAsync(
                                parentInfoDto.FullNameMother, parentInfoDto.YearOfBirthMother,
                                parentInfoDto.PhoneNumberMother, parentInfoDto.EmailMother, parentInfoDto.IdcardNumberMother);
                        }
                        else if (!string.IsNullOrEmpty(parentInfoDto.EmailGuardian))
                        {
                            existingParent = await _parentRepository.GetParentByDetailsAsync(
                                parentInfoDto.FullNameGuardian, parentInfoDto.YearOfBirthGuardian,
                                parentInfoDto.PhoneNumberGuardian, parentInfoDto.EmailGuardian, parentInfoDto.IdcardNumberGuardian);
                        }

                        if (existingParent != null)
                        {
                            Console.WriteLine($"Parent already exists with ParentID: {existingParent.ParentId}. Linking to student...");
                            student.ParentId = existingParent.ParentId;
                        }
                        else
                        {
                            var user = new User
                            {
                                Username = "parenttemp",
                                PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"),
                                Email = parentInfoDto.EmailFather ?? parentInfoDto.EmailMother ?? parentInfoDto.EmailGuardian,
                                PhoneNumber = parentInfoDto.PhoneNumberFather ?? parentInfoDto.PhoneNumberMother ?? parentInfoDto.PhoneNumberGuardian,
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
                            Console.WriteLine($"Created new user with UserID: {user.UserId}");

                            string roleName = "Parent";
                            string finalUsername = $"{roleName.ToLower()}{user.UserId}";
                            var existingUserByUsername = await _userRepository.GetByUsernameAsync(finalUsername);
                            if (existingUserByUsername != null)
                            {
                                throw new Exception($"Username {finalUsername} đã tồn tại (UserID: {existingUserByUsername.UserId}).");
                            }
                            user.Username = finalUsername;
                            await _userRepository.UpdateAsync(user);
                            Console.WriteLine($"Updated user with Username: {user.Username}");

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
                            Console.WriteLine($"Created new parent with ParentID: {parent.ParentId}");
                            student.ParentId = parent.ParentId;
                        }
                    }

                    students.Add(student);
                }

                await _studentRepository.AddRangeAsync(students);
                await transaction.CommitAsync();
                Console.WriteLine($"Imported {students.Count} students from Excel file");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back due to error: {ex.Message}");
                throw;
            }
        }
    }
}