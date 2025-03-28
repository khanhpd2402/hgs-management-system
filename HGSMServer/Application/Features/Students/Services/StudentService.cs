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

            // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await _studentRepository.BeginTransactionAsync();
            try
            {
                // Kiểm tra trùng lặp IdcardNumber (nếu có)
                if (!string.IsNullOrWhiteSpace(createStudentDto.IdcardNumber))
                {
                    bool exists = await _studentRepository.ExistsAsync(createStudentDto.IdcardNumber);
                    if (exists)
                    {
                        Console.WriteLine($"IdcardNumber {createStudentDto.IdcardNumber} already exists.");
                        throw new Exception($"Số CMND/CCCD {createStudentDto.IdcardNumber} đã tồn tại.");
                    }
                }

                // Kiểm tra ClassId có tồn tại không
                Console.WriteLine($"Checking if ClassId {createStudentDto.ClassId} exists...");
                if (!await _classRepository.ExistsAsync(createStudentDto.ClassId))
                {
                    Console.WriteLine($"ClassId {createStudentDto.ClassId} does not exist.");
                    throw new Exception($"ClassId {createStudentDto.ClassId} không tồn tại.");
                }

                // Tạo học sinh
                Console.WriteLine("Mapping CreateStudentDto to Student...");
                var student = _mapper.Map<Student>(createStudentDto);
                student.StudentClasses = new List<StudentClass>
        {
            new StudentClass
            {
                ClassId = createStudentDto.ClassId,
                AcademicYearId = 1 // Giả sử AcademicYearId = 1, bạn có thể thay đổi logic để lấy giá trị thực tế
            }
        };

                Console.WriteLine("Adding student to database...");
                await _studentRepository.AddAsync(student);
                Console.WriteLine($"Student added with ID: {student.StudentId}");

                // Xử lý thông tin cha
                if (!string.IsNullOrEmpty(createStudentDto.FullNameFather) &&
                    (!string.IsNullOrEmpty(createStudentDto.PhoneNumberFather) || !string.IsNullOrEmpty(createStudentDto.EmailFather)))
                {
                    Console.WriteLine("Processing father information...");
                    await CreateParentAndLinkAsync(student.StudentId, createStudentDto.FullNameFather,
                        createStudentDto.YearOfBirthFather, createStudentDto.OccupationFather,
                        createStudentDto.PhoneNumberFather, createStudentDto.EmailFather,
                        createStudentDto.IdcardNumberFather, "Father");
                }
                else
                {
                    Console.WriteLine($"Skipping father for StudentID {student.StudentId}: Insufficient data (FullName, PhoneNumber, or Email missing).");
                }

                // Xử lý thông tin mẹ
                if (!string.IsNullOrEmpty(createStudentDto.FullNameMother) &&
                    (!string.IsNullOrEmpty(createStudentDto.PhoneNumberMother) || !string.IsNullOrEmpty(createStudentDto.EmailMother)))
                {
                    Console.WriteLine("Processing mother information...");
                    await CreateParentAndLinkAsync(student.StudentId, createStudentDto.FullNameMother,
                        createStudentDto.YearOfBirthMother, createStudentDto.OccupationMother,
                        createStudentDto.PhoneNumberMother, createStudentDto.EmailMother,
                        createStudentDto.IdcardNumberMother, "Mother");
                }
                else
                {
                    Console.WriteLine($"Skipping mother for StudentID {student.StudentId}: Insufficient data (FullName, PhoneNumber, or Email missing).");
                }

                // Xử lý thông tin người bảo hộ
                if (!string.IsNullOrEmpty(createStudentDto.FullNameGuardian) &&
                    (!string.IsNullOrEmpty(createStudentDto.PhoneNumberGuardian) || !string.IsNullOrEmpty(createStudentDto.EmailGuardian)))
                {
                    Console.WriteLine("Processing guardian information...");
                    await CreateParentAndLinkAsync(student.StudentId, createStudentDto.FullNameGuardian,
                        createStudentDto.YearOfBirthGuardian, createStudentDto.OccupationGuardian,
                        createStudentDto.PhoneNumberGuardian, createStudentDto.EmailGuardian,
                        createStudentDto.IdcardNumberGuardian, "Guardian");
                }
                else
                {
                    Console.WriteLine($"Skipping guardian for StudentID {student.StudentId}: Insufficient data (FullName, PhoneNumber, or Email missing).");
                }

                // Commit transaction nếu tất cả thành công
                await transaction.CommitAsync();
                Console.WriteLine("Transaction committed successfully.");
                return student.StudentId; // Trả về ID sau khi thêm
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back due to error: {ex.Message}");
                throw; // Ném lại ngoại lệ để API trả về lỗi cho client
            }
        }

        private async Task CreateParentAndLinkAsync(int studentId, string fullName, DateOnly? yearOfBirth, string occupation,
    string phoneNumber, string email, string idcardNumber, string relationship)
        {
            Console.WriteLine($"Creating parent for StudentID {studentId}, Relationship: {relationship}, FullName: {fullName}, PhoneNumber: {phoneNumber}, Email: {email}");

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(fullName) || (string.IsNullOrEmpty(phoneNumber) && string.IsNullOrEmpty(email)))
            {
                Console.WriteLine($"Invalid parent data for StudentID {studentId}. Skipping...");
                return;
            }

            // Kiểm tra định dạng email (nếu có)
            if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
            {
                Console.WriteLine($"Invalid email format for parent of StudentID {studentId}: {email}. Skipping...");
                return;
            }

            // Kiểm tra định dạng số điện thoại (nếu có)
            if (!string.IsNullOrEmpty(phoneNumber) && !IsValidPhoneNumber(phoneNumber))
            {
                Console.WriteLine($"Invalid phone number format for parent of StudentID {studentId}: {phoneNumber}. Skipping...");
                return;
            }

            // Kiểm tra xem phụ huynh đã tồn tại chưa (dựa trên tất cả các trường)
            Console.WriteLine($"Checking if parent exists with FullName: {fullName}, PhoneNumber: {phoneNumber}, Email: {email}...");
            var existingParent = await _parentRepository.GetParentByDetailsAsync(fullName, yearOfBirth, phoneNumber, email, idcardNumber);
            if (existingParent != null)
            {
                Console.WriteLine($"Parent already exists for StudentID {studentId} with matching details. Using existing ParentID {existingParent.ParentId}.");

                // Kiểm tra xem mối quan hệ đã tồn tại chưa
                var existingRelationship = await _parentRepository.GetStudentParentAsync(studentId, existingParent.ParentId);
                if (existingRelationship != null)
                {
                    Console.WriteLine($"Relationship between StudentID {studentId} and ParentID {existingParent.ParentId} already exists. Skipping...");
                    return;
                }

                // Thêm mối quan hệ vào StudentParents
                var studentParent = new StudentParent
                {
                    StudentId = studentId,
                    ParentId = existingParent.ParentId,
                    Relationship = relationship
                };

                Console.WriteLine($"Adding relationship for StudentID {studentId} and ParentID {existingParent.ParentId}...");
                await _parentRepository.AddStudentParentAsync(studentParent);
                Console.WriteLine($"Added relationship for StudentID {studentId} and ParentID {existingParent.ParentId}.");
                return;
            }

            // Kiểm tra trùng lặp PhoneNumber và Email
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                Console.WriteLine($"Checking if PhoneNumber {phoneNumber} exists...");
                var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
                if (existingUserByPhone != null)
                {
                    Console.WriteLine($"PhoneNumber {phoneNumber} already exists for UserID {existingUserByPhone.UserId}.");
                    throw new Exception($"Số điện thoại {phoneNumber} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByPhone.UserId}).");
                }
            }
            if (!string.IsNullOrEmpty(email))
            {
                Console.WriteLine($"Checking if Email {email} exists...");
                var existingUserByEmail = await _userRepository.GetByEmailAsync(email);
                if (existingUserByEmail != null)
                {
                    Console.WriteLine($"Email {email} already exists for UserID {existingUserByEmail.UserId}.");
                    throw new Exception($"Email {email} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByEmail.UserId}).");
                }
            }

            // Tạo username tạm thời
            string lastName = ExtractLastName(fullName);
            Console.WriteLine($"Extracted lastName for {fullName}: {lastName}");
            string tempUsername = $"{lastName.ToLower()}temp"; // Username tạm thời

            // Tạo mới User với username tạm thời
            var user = new User
            {
                Username = tempUsername, // Gán username tạm thời để tránh lỗi NOT NULL
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"), // Mật khẩu mặc định là 1-8
                Email = email,
                PhoneNumber = phoneNumber,
                RoleId = 6, // RoleID của Parent
                Status = "Active"
            };

            Console.WriteLine("Adding new user to database with temporary username...");
            await _userRepository.AddAsync(user);
            Console.WriteLine($"Created new user with UserID: {user.UserId}");

            // Cập nhật username theo định dạng lastname + UserId
            string finalUsername = $"{lastName.ToLower()}{user.UserId}";
            Console.WriteLine($"Generated final username: {finalUsername}. Checking for duplicates...");
            // Kiểm tra trùng lặp username
            var existingUserByUsername = await _userRepository.GetByUsernameAsync(finalUsername);
            if (existingUserByUsername != null)
            {
                Console.WriteLine($"Username {finalUsername} already exists for UserID {existingUserByUsername.UserId}.");
                throw new Exception($"Username {finalUsername} đã tồn tại (UserID: {existingUserByUsername.UserId}). Vui lòng chọn tên khác.");
            }

            user.Username = finalUsername;
            Console.WriteLine($"Updating user with final Username: {user.Username}...");
            await _userRepository.UpdateAsync(user);
            Console.WriteLine($"Updated user with Username: {user.Username}");

            // Tạo mới Parent
            var parent = new Parent
            {
                UserId = user.UserId,
                FullName = fullName,
                Dob = yearOfBirth,
                Occupation = occupation ?? "Unknown",
                IdcardNumber = idcardNumber
            };

            Console.WriteLine("Adding new parent to database...");
            await _parentRepository.AddAsync(parent);
            Console.WriteLine($"Created new parent with ParentID: {parent.ParentId}");

            // Thêm mối quan hệ vào StudentParents
            var newStudentParent = new StudentParent
            {
                StudentId = studentId,
                ParentId = parent.ParentId,
                Relationship = relationship
            };

            Console.WriteLine($"Adding relationship for StudentID {studentId} and ParentID {parent.ParentId}...");
            await _parentRepository.AddStudentParentAsync(newStudentParent);
            Console.WriteLine($"Added relationship for StudentID {studentId} and ParentID {parent.ParentId}.");
        }

        private string ExtractLastName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return "unknown";

            // Tách tên (phần cuối cùng) từ FullName
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? RemoveDiacritics(parts[^1]) : "unknown"; // Lấy phần cuối cùng (tên)
        }

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

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
            // Kiểm tra số điện thoại có 10 chữ số và bắt đầu bằng 0
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^0\d{9}$");
        }

        public async Task UpdateStudentAsync(UpdateStudentDto updateStudentDto)
        {
            Console.WriteLine($"Updating student with StudentID: {updateStudentDto.StudentId}");

            // Kiểm tra học sinh có tồn tại không
            var student = await _studentRepository.GetByIdAsync(updateStudentDto.StudentId);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {updateStudentDto.StudentId} not found");
            }

            // Kiểm tra trùng lặp IdcardNumber (nếu có và thay đổi)
            if (!string.IsNullOrWhiteSpace(updateStudentDto.IdcardNumber) && updateStudentDto.IdcardNumber != student.IdcardNumber)
            {
                bool exists = await _studentRepository.ExistsAsync(updateStudentDto.IdcardNumber);
                if (exists)
                {
                    throw new Exception($"Số CMND/CCCD {updateStudentDto.IdcardNumber} đã tồn tại.");
                }
            }

            // Kiểm tra ClassId có tồn tại không (nếu thay đổi)
            var currentClass = student.StudentClasses?.FirstOrDefault(sc => sc.StudentId == student.StudentId);
            if (currentClass == null || currentClass.ClassId != updateStudentDto.ClassId)
            {
                if (!await _classRepository.ExistsAsync(updateStudentDto.ClassId))
                {
                    throw new Exception($"ClassId {updateStudentDto.ClassId} không tồn tại.");
                }

                // Cập nhật ClassId trong StudentClasses
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
                            AcademicYearId = 1 // Giả sử AcademicYearId = 1
                        }
                    };
                }
            }

            // Cập nhật thông tin học sinh
            _mapper.Map(updateStudentDto, student);
            await _studentRepository.UpdateAsync(student);
            Console.WriteLine($"Updated student with StudentID: {student.StudentId}");

            // Xử lý thông tin cha
            if (!string.IsNullOrEmpty(updateStudentDto.FullNameFather) &&
                (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberFather) || !string.IsNullOrEmpty(updateStudentDto.EmailFather)))
            {
                await UpdateOrCreateParentAsync(student.StudentId, updateStudentDto.FullNameFather,
                    updateStudentDto.YearOfBirthFather, updateStudentDto.OccupationFather,
                    updateStudentDto.PhoneNumberFather, updateStudentDto.EmailFather,
                    updateStudentDto.IdcardNumberFather, "Father");
            }

            // Xử lý thông tin mẹ
            if (!string.IsNullOrEmpty(updateStudentDto.FullNameMother) &&
                (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberMother) || !string.IsNullOrEmpty(updateStudentDto.EmailMother)))
            {
                await UpdateOrCreateParentAsync(student.StudentId, updateStudentDto.FullNameMother,
                    updateStudentDto.YearOfBirthMother, updateStudentDto.OccupationMother,
                    updateStudentDto.PhoneNumberMother, updateStudentDto.EmailMother,
                    updateStudentDto.IdcardNumberMother, "Mother");
            }

            // Xử lý thông tin người bảo hộ
            if (!string.IsNullOrEmpty(updateStudentDto.FullNameGuardian) &&
                (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberGuardian) || !string.IsNullOrEmpty(updateStudentDto.EmailGuardian)))
            {
                await UpdateOrCreateParentAsync(student.StudentId, updateStudentDto.FullNameGuardian,
                    updateStudentDto.YearOfBirthGuardian, updateStudentDto.OccupationGuardian,
                    updateStudentDto.PhoneNumberGuardian, updateStudentDto.EmailGuardian,
                    updateStudentDto.IdcardNumberGuardian, "Guardian");
            }
        }

        private async Task UpdateOrCreateParentAsync(int studentId, string fullName, DateOnly? yearOfBirth, string occupation,
            string phoneNumber, string email, string idcardNumber, string relationship)
        {
            Console.WriteLine($"Updating/Creating parent for StudentID {studentId}, Relationship: {relationship}, FullName: {fullName}");

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(fullName) || (string.IsNullOrEmpty(phoneNumber) && string.IsNullOrEmpty(email)))
            {
                Console.WriteLine($"Invalid parent data for StudentID {studentId}. Skipping...");
                return;
            }

            // Kiểm tra định dạng email (nếu có)
            if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
            {
                Console.WriteLine($"Invalid email format for parent of StudentID {studentId}: {email}. Skipping...");
                return;
            }

            // Kiểm tra định dạng số điện thoại (nếu có)
            if (!string.IsNullOrEmpty(phoneNumber) && !IsValidPhoneNumber(phoneNumber))
            {
                Console.WriteLine($"Invalid phone number format for parent of StudentID {studentId}: {phoneNumber}. Skipping...");
                return;
            }

            // Kiểm tra xem học sinh đã có phụ huynh với mối quan hệ này chưa
            var existingStudentParent = await _parentRepository.GetStudentParentByRelationshipAsync(studentId, relationship);
            if (existingStudentParent != null)
            {
                // Nếu đã có phụ huynh, cập nhật thông tin phụ huynh
                var parent = await _parentRepository.GetByIdAsync(existingStudentParent.ParentId);
                if (parent != null)
                {
                    // Kiểm tra xem FullName có thay đổi không
                    bool fullNameChanged = parent.FullName != fullName;

                    // Kiểm tra xem PhoneNumber hoặc Email có thay đổi không
                    var user = await _userRepository.GetByIdAsync(parent.UserId);
                    if (user != null)
                    {
                        bool phoneNumberChanged = user.PhoneNumber != phoneNumber;
                        bool emailChanged = user.Email != email;

                        // Kiểm tra trùng lặp PhoneNumber (nếu thay đổi)
                        if (phoneNumberChanged && !string.IsNullOrEmpty(phoneNumber))
                        {
                            var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
                            if (existingUserByPhone != null && existingUserByPhone.UserId != user.UserId)
                            {
                                throw new Exception($"Số điện thoại {phoneNumber} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByPhone.UserId}).");
                            }
                        }

                        // Kiểm tra trùng lặp Email (nếu thay đổi)
                        if (emailChanged && !string.IsNullOrEmpty(email))
                        {
                            var existingUserByEmail = await _userRepository.GetByEmailAsync(email);
                            if (existingUserByEmail != null && existingUserByEmail.UserId != user.UserId)
                            {
                                throw new Exception($"Email {email} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByEmail.UserId}).");
                            }
                        }

                        // Cập nhật thông tin phụ huynh
                        parent.FullName = fullName;
                        parent.Dob = yearOfBirth;
                        parent.Occupation = occupation ?? "Unknown";
                        parent.IdcardNumber = idcardNumber;

                        await _parentRepository.UpdateAsync(parent);
                        Console.WriteLine($"Updated parent with ParentID: {parent.ParentId}");

                        // Cập nhật thông tin User
                        user.PhoneNumber = phoneNumber;
                        user.Email = email;

                        // Nếu FullName thay đổi, cập nhật username theo định dạng lastname + UserId
                        if (fullNameChanged)
                        {
                            string lastName = ExtractLastName(fullName);
                            string newUsername = $"{lastName.ToLower()}{user.UserId}";
                            // Kiểm tra trùng lặp username
                            var existingUserByUsername = await _userRepository.GetByUsernameAsync(newUsername);
                            if (existingUserByUsername != null && existingUserByUsername.UserId != user.UserId)
                            {
                                throw new Exception($"Username {newUsername} đã tồn tại (UserID: {existingUserByUsername.UserId}). Vui lòng chọn tên khác.");
                            }
                            user.Username = newUsername;
                            Console.WriteLine($"Updated username for UserID {user.UserId} to {newUsername} due to FullName change.");
                        }

                        await _userRepository.UpdateAsync(user);
                        Console.WriteLine($"Updated user with UserID: {user.UserId}");
                    }
                }
                return;
            }

            // Nếu chưa có phụ huynh, tạo mới
            await CreateParentAndLinkAsync(studentId, fullName, yearOfBirth, occupation, phoneNumber, email, idcardNumber, relationship);
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

            foreach (var row in data)
            {
                string idCardNumber = row["Số CMND/CCCD"]?.ToString().Trim();
                if (string.IsNullOrWhiteSpace(idCardNumber))
                {
                    throw new Exception("Số CMND/CCCD không được để trống.");
                }

                // Kiểm tra trùng lặp trước khi insert
                bool exists = await _studentRepository.ExistsAsync(idCardNumber);
                if (exists)
                {
                    throw new Exception($"Số CMND/CCCD {idCardNumber} đã tồn tại.");
                }

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
                    IdcardNumber = idCardNumber, // Đã kiểm tra trùng lặp và null trước khi gán
                    Status = row["Trạng thái"]?.ToString().Trim() ?? "Đang học",
                    StudentClasses = new List<StudentClass>
                    {
                        new StudentClass
                        {
                            ClassId = 1, // Lớp cố định, có thể thay bằng logic động
                            AcademicYearId = 1 // Năm học cố định, cần thay bằng giá trị thực tế
                        }
                    }
                };
                students.Add(student);
            }

            await _studentRepository.AddRangeAsync(students);
            Console.WriteLine($"Imported {students.Count} students from Excel file");
        }
    }
}