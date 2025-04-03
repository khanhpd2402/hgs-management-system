using Application.Features.Parents.DTOs;
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

        public async Task<List<string>> ImportStudentsFromExcelAsync(IFormFile file)
        {
            Console.WriteLine("Importing students from Excel file");

            if (file == null || file.Length == 0)
                throw new ArgumentException("Vui lòng chọn file Excel!");

            var data = ExcelImportHelper.ReadExcelData(file);
            var students = new List<Student>();
            var importResults = new List<string>();

            var currentDate = DateOnly.FromDateTime(DateTime.Now); // 30/03/2025
            var currentAcademicYear = await _studentRepository.GetCurrentAcademicYearAsync(currentDate);
            if (currentAcademicYear == null || currentAcademicYear.YearName != "2024-2025")
                throw new Exception("Không tìm thấy năm học hiện tại (2024-2025).");

            // Định nghĩa culture để parse ngày tháng định dạng "dd-MM-yyyy"
            var culture = new System.Globalization.CultureInfo("vi-VN");
            var dateFormat = "dd-MM-yyyy";

            using var transaction = await _studentRepository.BeginTransactionAsync();
            try
            {
                foreach (var row in data)
                {
                    try
                    {
                        // Kiểm tra các trường bắt buộc
                        if (!row.TryGetValue("Họ và tên", out var fullName) || string.IsNullOrEmpty(fullName) ||
                            !row.TryGetValue("Ngày sinh", out var dobStr) || string.IsNullOrEmpty(dobStr) ||
                            !row.TryGetValue("Giới tính", out var gender) || string.IsNullOrEmpty(gender) ||
                            !row.TryGetValue("Ngày nhập học", out var admissionDateStr) || string.IsNullOrEmpty(admissionDateStr) ||
                            !row.TryGetValue("Số CMND/CCCD", out var idCardNumber) || string.IsNullOrEmpty(idCardNumber) ||
                            !row.TryGetValue("Tên lớp", out var className) || string.IsNullOrEmpty(className))
                        {
                            importResults.Add($"Dòng dữ liệu thiếu thông tin bắt buộc: {string.Join(", ", row.Values)}");
                            continue;
                        }

                        // Parse dữ liệu ngày tháng với định dạng "dd-MM-yyyy"
                        if (!DateOnly.TryParseExact(dobStr, dateFormat, culture, System.Globalization.DateTimeStyles.None, out var dob))
                            throw new Exception($"Ngày sinh '{dobStr}' không hợp lệ. Định dạng phải là 'dd-MM-yyyy'.");
                        if (!DateOnly.TryParseExact(admissionDateStr, dateFormat, culture, System.Globalization.DateTimeStyles.None, out var admissionDate))
                            throw new Exception($"Ngày nhập học '{admissionDateStr}' không hợp lệ. Định dạng phải là 'dd-MM-yyyy'.");

                        // Kiểm tra tên lớp
                        var validClasses = new[] { "6A", "6B", "7A", "7B", "8A", "8B", "9A", "9B" };
                        if (!validClasses.Contains(className.ToUpper()))
                            throw new Exception($"Tên lớp '{className}' không hợp lệ. Phải là một trong: {string.Join(", ", validClasses)}");

                        var classEntity = await _classRepository.GetClassByNameAsync(className.ToUpper());
                        if (classEntity == null)
                            throw new Exception($"Không tìm thấy lớp '{className}' trong hệ thống.");

                        // Kiểm tra trùng lặp IdcardNumber
                        if (await _studentRepository.ExistsAsync(idCardNumber.Trim()))
                            throw new Exception($"Số CMND/CCCD {idCardNumber} đã tồn tại.");

                        if (!IsValidIdCardNumber(idCardNumber))
                            throw new Exception($"Số CMND/CCCD {idCardNumber} không hợp lệ.");

                        // Tạo Student
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
                            IdcardNumber = idCardNumber.Trim(),
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

                        // Kiểm tra tính hợp lệ
                        if (student.Dob > currentDate)
                            throw new ArgumentException($"Ngày sinh của {student.FullName} không hợp lệ (vượt quá ngày hiện tại).");
                        if (!new[] { "Nam", "Nữ", "Khác" }.Contains(student.Gender))
                            throw new ArgumentException($"Giới tính của {student.FullName} không hợp lệ. Phải là 'Nam', 'Nữ' hoặc 'Khác'.");

                        // Xử lý thông tin cha mẹ
                        bool hasParentInfo = row.Any(kvp => kvp.Key.Contains("cha") || kvp.Key.Contains("mẹ") || kvp.Key.Contains("người bảo hộ"));
                        if (hasParentInfo)
                        {
                            Console.WriteLine($"Processing parent information for student {student.FullName}...");

                            var parentInfoDto = new ParentInfoDto
                            {
                                FullNameFather = row.TryGetValue("Họ và tên cha", out var fatherName) ? fatherName.Trim() : null,
                                YearOfBirthFather = row.TryGetValue("Ngày sinh cha", out var fatherDobStr) && DateOnly.TryParseExact(fatherDobStr, dateFormat, culture, System.Globalization.DateTimeStyles.None, out var fatherDob) ? fatherDob : null,
                                OccupationFather = row.TryGetValue("Nghề nghiệp cha", out var fatherOcc) ? fatherOcc.Trim() : null,
                                PhoneNumberFather = row.TryGetValue("SĐT cha", out var fatherPhone) ? fatherPhone.Trim() : null,
                                EmailFather = row.TryGetValue("Email cha", out var fatherEmail) ? fatherEmail.Trim() : null,
                                IdcardNumberFather = row.TryGetValue("Số CCCD cha", out var fatherIdCard) ? fatherIdCard.Trim() : null,
                                FullNameMother = row.TryGetValue("Họ và tên mẹ", out var motherName) ? motherName.Trim() : null,
                                YearOfBirthMother = row.TryGetValue("Ngày sinh mẹ", out var motherDobStr) && DateOnly.TryParseExact(motherDobStr, dateFormat, culture, System.Globalization.DateTimeStyles.None, out var motherDob) ? motherDob : null,
                                OccupationMother = row.TryGetValue("Nghề nghiệp mẹ", out var motherOcc) ? motherOcc.Trim() : null,
                                PhoneNumberMother = row.TryGetValue("SĐT mẹ", out var motherPhone) ? motherPhone.Trim() : null,
                                EmailMother = row.TryGetValue("Email mẹ", out var motherEmail) ? motherEmail.Trim() : null,
                                IdcardNumberMother = row.TryGetValue("Số CCCD mẹ", out var motherIdCard) ? motherIdCard.Trim() : null,
                                FullNameGuardian = row.TryGetValue("Họ và tên người bảo hộ", out var guardianName) ? guardianName.Trim() : null,
                                YearOfBirthGuardian = row.TryGetValue("Ngày sinh người bảo hộ", out var guardianDobStr) && DateOnly.TryParseExact(guardianDobStr, dateFormat, culture, System.Globalization.DateTimeStyles.None, out var guardianDob) ? guardianDob : null,
                                OccupationGuardian = row.TryGetValue("Nghề nghiệp người bảo hộ", out var guardianOcc) ? guardianOcc.Trim() : null,
                                PhoneNumberGuardian = row.TryGetValue("SĐT người bảo hộ", out var guardianPhone) ? guardianPhone.Trim() : null,
                                EmailGuardian = row.TryGetValue("Email người bảo hộ", out var guardianEmail) ? guardianEmail.Trim() : null,
                                IdcardNumberGuardian = row.TryGetValue("Số CCCD người bảo hộ", out var guardianIdCard) ? guardianIdCard.Trim() : null
                            };

                            ValidateParentInfoDto(parentInfoDto);

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

                                if (!string.IsNullOrEmpty(user.PhoneNumber) && await _userRepository.GetByPhoneNumberAsync(user.PhoneNumber) != null)
                                    throw new Exception($"Số điện thoại {user.PhoneNumber} đã tồn tại.");
                                if (!string.IsNullOrEmpty(user.Email) && await _userRepository.GetByEmailAsync(user.Email) != null)
                                    throw new Exception($"Email {user.Email} đã tồn tại.");

                                await _userRepository.AddAsync(user);
                                user.Username = $"parent{user.UserId}";
                                await _userRepository.UpdateAsync(user);

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
                                student.ParentId = parent.ParentId;
                            }
                        }

                        students.Add(student);
                        importResults.Add($"Đã thêm học sinh: {student.FullName} (CMND/CCCD: {student.IdcardNumber}, Lớp: {className})");
                    }
                    catch (Exception ex)
                    {
                        importResults.Add($"Lỗi khi xử lý dòng: {string.Join(", ", row.Values)}. Chi tiết: {ex.Message}");
                    }
                }

                if (students.Any())
                {
                    await _studentRepository.AddRangeAsync(students);
                }

                await transaction.CommitAsync();
                Console.WriteLine($"Imported {students.Count} students from Excel file");
                return importResults;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back due to error: {ex.Message}");
                throw new Exception($"Lỗi khi import học sinh: {ex.Message}");
            }
        }
    }
}