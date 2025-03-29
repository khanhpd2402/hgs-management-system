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
using System.Threading.Tasks;

namespace Application.Features.Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IParentRepository _parentRepository;
        private readonly IMapper _mapper;

        public StudentService(
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            IParentRepository parentRepository,
            IMapper mapper)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _parentRepository = parentRepository ?? throw new ArgumentNullException(nameof(parentRepository));
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

            // Kiểm tra trùng lặp IdcardNumber (nếu có)
            if (!string.IsNullOrWhiteSpace(createStudentDto.IdcardNumber))
            {
                bool exists = await _studentRepository.ExistsAsync(createStudentDto.IdcardNumber);
                if (exists)
                {
                    throw new Exception($"Số CMND/CCCD {createStudentDto.IdcardNumber} đã tồn tại.");
                }
            }

            // Tạo học sinh
            var student = _mapper.Map<Student>(createStudentDto);
            student.StudentClasses = new List<StudentClass>
            {
                new StudentClass
                {
                    ClassId = createStudentDto.ClassId,
                    AcademicYearId = 1 // Giả sử AcademicYearId = 1, bạn có thể thay đổi logic để lấy giá trị thực tế
                }
            };

            await _studentRepository.AddAsync(student);

            // Xử lý thông tin cha
            if (!string.IsNullOrEmpty(createStudentDto.FullNameFather) &&
                (!string.IsNullOrEmpty(createStudentDto.PhoneNumberFather) || !string.IsNullOrEmpty(createStudentDto.EmailFather)))
            {
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
                await CreateParentAndLinkAsync(student.StudentId, createStudentDto.FullNameGuardian,
                    createStudentDto.YearOfBirthGuardian, createStudentDto.OccupationGuardian,
                    createStudentDto.PhoneNumberGuardian, createStudentDto.EmailGuardian,
                    createStudentDto.IdcardNumberGuardian, "Guardian");
            }
            else
            {
                Console.WriteLine($"Skipping guardian for StudentID {student.StudentId}: Insufficient data (FullName, PhoneNumber, or Email missing).");
            }

            return student.StudentId; // Trả về ID sau khi thêm
        }

        private async Task CreateParentAndLinkAsync(int studentId, string fullName, DateOnly? yearOfBirth, string occupation,
            string phoneNumber, string email, string idcardNumber, string relationship)
        {
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
            var existingParent = await _parentRepository.GetParentByDetailsAsync(fullName, yearOfBirth, phoneNumber, email, idcardNumber);
            if (existingParent != null)
            {
                // Nếu phụ huynh đã tồn tại với tất cả thông tin trùng khớp, sử dụng phụ huynh hiện có
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

                await _parentRepository.AddStudentParentAsync(studentParent);
                return;
            }

            // Nếu không tìm thấy phụ huynh trùng khớp, tạo mới User và Parent
            User user = null;
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
            }
            if (user == null && !string.IsNullOrEmpty(email))
            {
                user = await _userRepository.GetByEmailAsync(email);
            }

            if (user == null)
            {
                // Tạo username theo định dạng lastname+stt
                string lastName = ExtractLastName(fullName); // Lấy phần tên (phần cuối cùng)
                Console.WriteLine($"Extracted lastName for {fullName}: {lastName}");

                int maxUserId = await _userRepository.GetMaxUserIdAsync(); // Lấy UserID lớn nhất hiện tại
                int stt = maxUserId + 1; // Số thứ tự sẽ khớp với UserID của bản ghi mới
                string username = $"{lastName.ToLower()}{stt}";

                // Kiểm tra xem username đã tồn tại chưa, nếu có thì tăng stt cho đến khi tìm được username chưa tồn tại
                while (await _userRepository.GetByUsernameAsync(username) != null)
                {
                    stt++;
                    username = $"{lastName.ToLower()}{stt}";
                }

                Console.WriteLine($"Creating new user with Username: {username}, stt: {stt}");

                // Tạo mới User
                user = new User
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"), // Mật khẩu mặc định là 1-8
                    Email = email,
                    PhoneNumber = phoneNumber,
                    RoleId = 6, // RoleID của Parent
                    Status = "Active"
                };

                await _userRepository.AddAsync(user);
                Console.WriteLine($"Created new user with UserID: {user.UserId}, Username: {user.Username}");
            }
            else
            {
                // Nếu User đã tồn tại nhưng thông tin khác (ví dụ: Email hoặc PhoneNumber khác), tạo User mới
                string lastName = ExtractLastName(fullName);
                Console.WriteLine($"Extracted lastName for {fullName}: {lastName}");

                int maxUserId = await _userRepository.GetMaxUserIdAsync();
                int stt = maxUserId + 1;
                string username = $"{lastName.ToLower()}{stt}";

                // Kiểm tra xem username đã tồn tại chưa
                while (await _userRepository.GetByUsernameAsync(username) != null)
                {
                    stt++;
                    username = $"{lastName.ToLower()}{stt}";
                }

                Console.WriteLine($"Creating new user with Username: {username}, stt: {stt}");

                user = new User
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"), // Mật khẩu mặc định là 1-8
                    Email = email,
                    PhoneNumber = phoneNumber,
                    RoleId = 6,
                    Status = "Active"
                };

                await _userRepository.AddAsync(user);
                Console.WriteLine($"Created new user with UserID: {user.UserId}, Username: {user.Username}");
            }

            // Tạo mới Parent
            var parent = new Parent
            {
                UserId = user.UserId,
                FullName = fullName,
                Dob = yearOfBirth,
                Occupation = occupation ?? "Unknown",
                IdcardNumber = idcardNumber
            };

            await _parentRepository.AddAsync(parent);

            // Thêm mối quan hệ vào StudentParents
            var newStudentParent = new StudentParent
            {
                StudentId = studentId,
                ParentId = parent.ParentId,
                Relationship = relationship
            };

            await _parentRepository.AddStudentParentAsync(newStudentParent);
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
            var student = await _studentRepository.GetByIdAsync(updateStudentDto.StudentId);
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found");
            }

            _mapper.Map(updateStudentDto, student); // Cập nhật từ DTO vào entity
            await _studentRepository.UpdateAsync(student);
        }

        public async Task DeleteStudentAsync(int id)
        {
            await _studentRepository.DeleteAsync(id);
        }

        public async Task ImportStudentsFromExcelAsync(IFormFile file)
        {
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
        }
    }
}