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
        private readonly IRoleRepository _roleRepository; 
        private readonly IMapper _mapper;
        private readonly HgsdbContext _context;

        public StudentService(
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            IParentRepository parentRepository,
            IClassRepository classRepository,
            IRoleRepository roleRepository, 
            IMapper mapper,
            HgsdbContext context)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _parentRepository = parentRepository ?? throw new ArgumentNullException(nameof(parentRepository));
            _classRepository = classRepository ?? throw new ArgumentNullException(nameof(classRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository)); 
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
                // Kiểm tra CMND/CCCD học sinh
                if (!string.IsNullOrWhiteSpace(createStudentDto.IdcardNumber))
                {
                    bool exists = await _studentRepository.ExistsAsync(createStudentDto.IdcardNumber);
                    if (exists)
                    {
                        Console.WriteLine($"IdcardNumber {createStudentDto.IdcardNumber} already exists.");
                        throw new Exception($"Số CMND/CCCD {createStudentDto.IdcardNumber} đã tồn tại.");
                    }
                }

                // Kiểm tra lớp học
                Console.WriteLine($"Checking if ClassId {createStudentDto.ClassId} exists...");
                if (!await _classRepository.ExistsAsync(createStudentDto.ClassId))
                {
                    Console.WriteLine($"ClassId {createStudentDto.ClassId} does not exist.");
                    throw new Exception($"ClassId {createStudentDto.ClassId} không tồn tại.");
                }

                // Map DTO sang Student
                Console.WriteLine("Mapping CreateStudentDto to Student...");
                var student = _mapper.Map<Student>(createStudentDto);
                var currentAcademicYearId = createStudentDto.AcademicYearId ?? await GetCurrentAcademicYearIdAsync();
                student.StudentClasses = new List<Domain.Models.StudentClass>
        {
            new Domain.Models.StudentClass
            {
                ClassId = createStudentDto.ClassId,
                AcademicYearId = currentAcademicYearId
            }
        };

                Console.WriteLine("Adding student to database...");
                await _studentRepository.AddAsync(student);
                Console.WriteLine($"Student added with ID: {student.StudentId}");

                // Xử lý phụ huynh
                if (createStudentDto.ParentId.HasValue)
                {
                    // Nếu có ParentId, cập nhật thông tin phụ huynh
                    Console.WriteLine($"Updating parent with ParentID: {createStudentDto.ParentId}");
                    var parent = await _parentRepository.GetByIdAsync(createStudentDto.ParentId.Value);
                    if (parent == null)
                    {
                        throw new Exception($"Phụ huynh với ID {createStudentDto.ParentId.Value} không tồn tại.");
                    }

                    // Cập nhật thông tin phụ huynh nếu có dữ liệu mới
                    if (HasParentInfoToUpdate(createStudentDto))
                    {
                        Console.WriteLine("Updating parent information...");
                        await UpdateParentAsync(student, createStudentDto);
                    }
                    else
                    {
                        // Nếu không có thông tin mới, chỉ liên kết với ParentId
                        student.ParentId = createStudentDto.ParentId.Value;
                        Console.WriteLine($"Linking student to existing ParentID: {student.ParentId}");
                    }
                }
                else if (HasCompleteParentInfo(createStudentDto))
                {
                    // Nếu không có ParentId nhưng có thông tin đầy đủ, tạo phụ huynh mới
                    Console.WriteLine("Creating new parent...");
                    await CreateParentAndLinkAsync(student, createStudentDto);
                }
                else
                {
                    Console.WriteLine($"Skipping parent creation/update for StudentID {student.StudentId}: No ParentId or complete parent data provided.");
                }

                await _studentRepository.UpdateAsync(student); // Cập nhật student với ParentId (nếu có)
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

        // Hàm kiểm tra xem có thông tin phụ huynh nào để cập nhật không
        private bool HasParentInfoToUpdate(IParentInfoDto dto)
        {
            return !string.IsNullOrEmpty(dto.FullNameFather) ||
                   dto.YearOfBirthFather.HasValue ||
                   !string.IsNullOrEmpty(dto.OccupationFather) ||
                   !string.IsNullOrEmpty(dto.PhoneNumberFather) ||
                   !string.IsNullOrEmpty(dto.EmailFather) ||
                   !string.IsNullOrEmpty(dto.IdcardNumberFather) ||
                   !string.IsNullOrEmpty(dto.FullNameMother) ||
                   dto.YearOfBirthMother.HasValue ||
                   !string.IsNullOrEmpty(dto.OccupationMother) ||
                   !string.IsNullOrEmpty(dto.PhoneNumberMother) ||
                   !string.IsNullOrEmpty(dto.EmailMother) ||
                   !string.IsNullOrEmpty(dto.IdcardNumberMother) ||
                   !string.IsNullOrEmpty(dto.FullNameGuardian) ||
                   dto.YearOfBirthGuardian.HasValue ||
                   !string.IsNullOrEmpty(dto.OccupationGuardian) ||
                   !string.IsNullOrEmpty(dto.PhoneNumberGuardian) ||
                   !string.IsNullOrEmpty(dto.EmailGuardian) ||
                   !string.IsNullOrEmpty(dto.IdcardNumberGuardian);
        }
        private async Task UpdateParentAsync(Student student, CreateStudentDto createStudentDto)
        {
            if (!createStudentDto.ParentId.HasValue)
                throw new ArgumentException("ParentId is required for updating parent.");

            var parent = await _parentRepository.GetByIdAsync(createStudentDto.ParentId.Value);
            if (parent == null)
                throw new Exception($"Parent with ID {createStudentDto.ParentId.Value} not found.");

            ValidateParentInfoDto(createStudentDto);

            // Cập nhật thông tin phụ huynh
            var user = await _userRepository.GetByIdAsync(parent.UserId);
            if (user == null)
                throw new Exception($"User associated with ParentID {parent.ParentId} not found.");

            // Cập nhật thông tin user (nếu có thay đổi phone hoặc email)
            string newPhoneNumber = createStudentDto.PhoneNumberFather ?? createStudentDto.PhoneNumberMother ?? createStudentDto.PhoneNumberGuardian;
            string newEmail = createStudentDto.EmailFather ?? createStudentDto.EmailMother ?? createStudentDto.EmailGuardian;

            if (!string.IsNullOrEmpty(newPhoneNumber) && newPhoneNumber != user.PhoneNumber)
            {
                var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(newPhoneNumber);
                if (existingUserByPhone != null && existingUserByPhone.UserId != user.UserId)
                    throw new Exception($"Số điện thoại {newPhoneNumber} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByPhone.UserId}).");
                user.PhoneNumber = newPhoneNumber;
            }

            if (!string.IsNullOrEmpty(newEmail) && newEmail != user.Email)
            {
                var existingUserByEmail = await _userRepository.GetByEmailAsync(newEmail);
                if (existingUserByEmail != null && existingUserByEmail.UserId != user.UserId)
                    throw new Exception($"Email {newEmail} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByEmail.UserId}).");
                user.Email = newEmail;
            }

            // Cập nhật username nếu có thay đổi tên
            string fullNameForUsername = createStudentDto.FullNameFather ?? createStudentDto.FullNameMother ?? createStudentDto.FullNameGuardian;
            if (!string.IsNullOrEmpty(fullNameForUsername))
            {
                string newUsername = FormatUserName.GenerateUsername(fullNameForUsername, user.UserId);
                if (user.Username != newUsername)
                {
                    var existingUserByUsername = await _userRepository.GetByUsernameAsync(newUsername);
                    if (existingUserByUsername != null && existingUserByUsername.UserId != user.UserId)
                        newUsername += $"_{user.UserId}";
                    user.Username = newUsername;
                }
            }

            await _userRepository.UpdateAsync(user);
            Console.WriteLine($"Updated user with UserID: {user.UserId}");

            // Cập nhật thông tin phụ huynh
            parent.FullNameFather = createStudentDto.FullNameFather ?? parent.FullNameFather;
            parent.YearOfBirthFather = createStudentDto.YearOfBirthFather ?? parent.YearOfBirthFather;
            parent.OccupationFather = createStudentDto.OccupationFather ?? parent.OccupationFather;
            parent.PhoneNumberFather = createStudentDto.PhoneNumberFather ?? parent.PhoneNumberFather;
            parent.EmailFather = createStudentDto.EmailFather ?? parent.EmailFather;
            parent.IdcardNumberFather = createStudentDto.IdcardNumberFather ?? parent.IdcardNumberFather;

            parent.FullNameMother = createStudentDto.FullNameMother ?? parent.FullNameMother;
            parent.YearOfBirthMother = createStudentDto.YearOfBirthMother ?? parent.YearOfBirthMother;
            parent.OccupationMother = createStudentDto.OccupationMother ?? parent.OccupationMother;
            parent.PhoneNumberMother = createStudentDto.PhoneNumberMother ?? parent.PhoneNumberMother;
            parent.EmailMother = createStudentDto.EmailMother ?? parent.EmailMother;
            parent.IdcardNumberMother = createStudentDto.IdcardNumberMother ?? parent.IdcardNumberMother;

            parent.FullNameGuardian = createStudentDto.FullNameGuardian ?? parent.FullNameGuardian;
            parent.YearOfBirthGuardian = createStudentDto.YearOfBirthGuardian ?? parent.YearOfBirthGuardian;
            parent.OccupationGuardian = createStudentDto.OccupationGuardian ?? parent.OccupationGuardian;
            parent.PhoneNumberGuardian = createStudentDto.PhoneNumberGuardian ?? parent.PhoneNumberGuardian;
            parent.EmailGuardian = createStudentDto.EmailGuardian ?? parent.EmailGuardian;
            parent.IdcardNumberGuardian = createStudentDto.IdcardNumberGuardian ?? parent.IdcardNumberGuardian;

            await _parentRepository.UpdateAsync(parent);
            Console.WriteLine($"Updated parent with ParentID: {parent.ParentId}");

            student.ParentId = parent.ParentId;
        }
        private async Task CreateParentAndLinkAsync(Student student, IParentInfoDto parentInfoDto)
        {
            Console.WriteLine($"Creating parent for StudentID {student.StudentId}");

            ValidateParentInfoDto(parentInfoDto);

            // Kiểm tra thông tin đầy đủ cho cha, mẹ, hoặc người bảo hộ
            bool hasCompleteFatherInfo = !string.IsNullOrEmpty(parentInfoDto.FullNameFather) &&
                                        parentInfoDto.YearOfBirthFather.HasValue &&
                                        !string.IsNullOrEmpty(parentInfoDto.PhoneNumberFather) &&
                                        !string.IsNullOrEmpty(parentInfoDto.IdcardNumberFather);

            bool hasCompleteMotherInfo = !string.IsNullOrEmpty(parentInfoDto.FullNameMother) &&
                                        parentInfoDto.YearOfBirthMother.HasValue &&
                                        !string.IsNullOrEmpty(parentInfoDto.PhoneNumberMother) &&
                                        !string.IsNullOrEmpty(parentInfoDto.IdcardNumberMother);

            bool hasCompleteGuardianInfo = !string.IsNullOrEmpty(parentInfoDto.FullNameGuardian) &&
                                          parentInfoDto.YearOfBirthGuardian.HasValue &&
                                          !string.IsNullOrEmpty(parentInfoDto.PhoneNumberGuardian) &&
                                          !string.IsNullOrEmpty(parentInfoDto.IdcardNumberGuardian);

            // Log để kiểm tra giá trị đầu vào
            Console.WriteLine($"Father Info - FullName: {parentInfoDto.FullNameFather}, YearOfBirth: {parentInfoDto.YearOfBirthFather}, Phone: {parentInfoDto.PhoneNumberFather}, IDCard: {parentInfoDto.IdcardNumberFather}");
            Console.WriteLine($"Mother Info - FullName: {parentInfoDto.FullNameMother}, YearOfBirth: {parentInfoDto.YearOfBirthMother}, Phone: {parentInfoDto.PhoneNumberMother}, IDCard: {parentInfoDto.IdcardNumberMother}");
            Console.WriteLine($"Guardian Info - FullName: {parentInfoDto.FullNameGuardian}, YearOfBirth: {parentInfoDto.YearOfBirthGuardian}, Phone: {parentInfoDto.PhoneNumberGuardian}, IDCard: {parentInfoDto.IdcardNumberGuardian}");
            Console.WriteLine($"HasCompleteFatherInfo: {hasCompleteFatherInfo}, HasCompleteMotherInfo: {hasCompleteMotherInfo}, HasCompleteGuardianInfo: {hasCompleteGuardianInfo}");

            if (!hasCompleteFatherInfo && !hasCompleteMotherInfo && !hasCompleteGuardianInfo)
            {
                Console.WriteLine($"Không đủ thông tin để tạo phụ huynh cho StudentID {student.StudentId}. Bỏ qua.");
                return;
            }

            // Kiểm tra trùng lặp CCCD
            if (hasCompleteFatherInfo && !string.IsNullOrEmpty(parentInfoDto.IdcardNumberFather))
            {
                var existingParentByFatherIdCard = await _parentRepository.GetParentByIdCardAsync(parentInfoDto.IdcardNumberFather);
                if (existingParentByFatherIdCard != null)
                    throw new Exception($"Số CCCD cha {parentInfoDto.IdcardNumberFather} đã tồn tại (ParentID: {existingParentByFatherIdCard.ParentId}).");
            }

            if (hasCompleteMotherInfo && !string.IsNullOrEmpty(parentInfoDto.IdcardNumberMother))
            {
                var existingParentByMotherIdCard = await _parentRepository.GetParentByIdCardAsync(parentInfoDto.IdcardNumberMother);
                if (existingParentByMotherIdCard != null)
                    throw new Exception($"Số CCCD mẹ {parentInfoDto.IdcardNumberMother} đã tồn tại (ParentID: {existingParentByMotherIdCard.ParentId}).");
            }

            if (hasCompleteGuardianInfo && !string.IsNullOrEmpty(parentInfoDto.IdcardNumberGuardian))
            {
                var existingParentByGuardianIdCard = await _parentRepository.GetParentByIdCardAsync(parentInfoDto.IdcardNumberGuardian);
                if (existingParentByGuardianIdCard != null)
                    throw new Exception($"Số CCCD người bảo hộ {parentInfoDto.IdcardNumberGuardian} đã tồn tại (ParentID: {existingParentByGuardianIdCard.ParentId}).");
            }

            string primaryFullName = null;
            string primaryPhoneNumber = null;
            string primaryEmail = null;
            string primaryIdCard = null;

            // Chọn thông tin chính để tạo user
            if (hasCompleteFatherInfo)
            {
                primaryFullName = parentInfoDto.FullNameFather;
                primaryPhoneNumber = parentInfoDto.PhoneNumberFather;
                primaryEmail = parentInfoDto.EmailFather;
                primaryIdCard = parentInfoDto.IdcardNumberFather;
            }
            else if (hasCompleteMotherInfo)
            {
                primaryFullName = parentInfoDto.FullNameMother;
                primaryPhoneNumber = parentInfoDto.PhoneNumberMother;
                primaryEmail = parentInfoDto.EmailMother;
                primaryIdCard = parentInfoDto.IdcardNumberMother;
            }
            else if (hasCompleteGuardianInfo)
            {
                primaryFullName = parentInfoDto.FullNameGuardian;
                primaryPhoneNumber = parentInfoDto.PhoneNumberGuardian;
                primaryEmail = parentInfoDto.EmailGuardian;
                primaryIdCard = parentInfoDto.IdcardNumberGuardian;
            }

            if (string.IsNullOrEmpty(primaryFullName))
            {
                Console.WriteLine($"Không tìm thấy primaryFullName hợp lệ cho StudentID {student.StudentId}. Bỏ qua việc tạo phụ huynh.");
                return;
            }

            // Lấy RoleId của "Phụ huynh" từ IRoleRepository
            var parentRole = await _roleRepository.GetRoleByNameAsync("Phụ huynh");
            if (parentRole == null)
            {
                throw new Exception("Vai trò 'Phụ huynh' không tồn tại trong hệ thống.");
            }

            // Tạo user mới với RoleId động
            var user = new User
            {
                Username = $"temp_{Guid.NewGuid().ToString().Substring(0, 8)}",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"),
                Email = primaryEmail,
                PhoneNumber = primaryPhoneNumber,
                RoleId = parentRole.RoleId,
                Status = "Hoạt động"
            };  

            // Kiểm tra trùng lặp PhoneNumber và Email
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

            // Tạo username dựa trên primaryFullName
            string finalUsername = FormatUserName.GenerateUsername(primaryFullName, user.UserId);
            var existingUserByUsername = await _userRepository.GetByUsernameAsync(finalUsername);
            if (existingUserByUsername != null)
            {
                finalUsername += $"_{user.UserId}";
            }
            user.Username = finalUsername;
            await _userRepository.UpdateAsync(user);
            Console.WriteLine($"Cập nhật user với Username: {user.Username}");

            // Tạo parent mới
            var parent = new Parent
            {
                UserId = user.UserId,
                FullNameFather = parentInfoDto.FullNameFather,
                YearOfBirthFather = parentInfoDto.YearOfBirthFather,
                OccupationFather = parentInfoDto.OccupationFather,
                PhoneNumberFather = parentInfoDto.PhoneNumberFather,
                EmailFather = parentInfoDto.EmailFather,
                IdcardNumberFather = parentInfoDto.IdcardNumberFather,
                FullNameMother = parentInfoDto.FullNameMother,
                YearOfBirthMother = parentInfoDto.YearOfBirthMother,
                OccupationMother = parentInfoDto.OccupationMother,
                PhoneNumberMother = parentInfoDto.PhoneNumberMother,
                EmailMother = parentInfoDto.EmailMother,
                IdcardNumberMother = parentInfoDto.IdcardNumberMother,
                FullNameGuardian = parentInfoDto.FullNameGuardian,
                YearOfBirthGuardian = parentInfoDto.YearOfBirthGuardian,
                OccupationGuardian = parentInfoDto.OccupationGuardian,
                PhoneNumberGuardian = parentInfoDto.PhoneNumberGuardian,
                EmailGuardian = parentInfoDto.EmailGuardian,
                IdcardNumberGuardian = parentInfoDto.IdcardNumberGuardian
            };

            await _parentRepository.AddAsync(parent);
            Console.WriteLine($"Tạo phụ huynh mới với ParentID: {parent.ParentId}");

            student.ParentId = parent.ParentId;
            Console.WriteLine($"Liên kết StudentID với ParentID {parent.ParentId}");
        }
        private bool HasCompleteParentInfo(IParentInfoDto dto)
        {
            return (!string.IsNullOrEmpty(dto.FullNameFather) && dto.YearOfBirthFather.HasValue && !string.IsNullOrEmpty(dto.PhoneNumberFather) && !string.IsNullOrEmpty(dto.IdcardNumberFather)) ||
                   (!string.IsNullOrEmpty(dto.FullNameMother) && dto.YearOfBirthMother.HasValue && !string.IsNullOrEmpty(dto.PhoneNumberMother) && !string.IsNullOrEmpty(dto.IdcardNumberMother)) ||
                   (!string.IsNullOrEmpty(dto.FullNameGuardian) && dto.YearOfBirthGuardian.HasValue && !string.IsNullOrEmpty(dto.PhoneNumberGuardian) && !string.IsNullOrEmpty(dto.IdcardNumberGuardian));
        }
        private async Task<int> GetCurrentAcademicYearIdAsync()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var currentAcademicYear = await _studentRepository.GetCurrentAcademicYearAsync(currentDate);
            if (currentAcademicYear == null)
                throw new Exception("Không tìm thấy năm học hiện tại.");
            return currentAcademicYear.AcademicYearId;
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

        public async Task UpdateStudentAsync(int id, UpdateStudentDto updateStudentDto)
        {
            Console.WriteLine($"Updating student with StudentID: {id}");

            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found");
            }

            using var transaction = await _studentRepository.BeginTransactionAsync();
            try
            {
                // Kiểm tra và cập nhật chỉ các trường hợp lệ
                if (!string.IsNullOrWhiteSpace(updateStudentDto.FullName) && updateStudentDto.FullName != "string")
                    student.FullName = updateStudentDto.FullName;

                if (updateStudentDto.Dob != null && updateStudentDto.Dob <= DateOnly.FromDateTime(DateTime.Now))
                    student.Dob = updateStudentDto.Dob;

                if (!string.IsNullOrWhiteSpace(updateStudentDto.Gender) && updateStudentDto.Gender != "string" && new[] { "Nam", "Nữ", "Khác" }.Contains(updateStudentDto.Gender))
                    student.Gender = updateStudentDto.Gender;

                if (updateStudentDto.ClassId != null && updateStudentDto.ClassId != 0)
                {
                    if (!await _classRepository.ExistsAsync(updateStudentDto.ClassId))
                        throw new Exception($"ClassId {updateStudentDto.ClassId} không tồn tại.");

                    var currentClass = student.StudentClasses?.FirstOrDefault();
                    if (currentClass != null)
                        currentClass.ClassId = updateStudentDto.ClassId;
                    else
                        student.StudentClasses = new List<Domain.Models.StudentClass>
                {
                    new Domain.Models.StudentClass
                    {
                        StudentId = student.StudentId,
                        ClassId = updateStudentDto.ClassId,
                        AcademicYearId = 1 // Giả sử AcademicYearId mặc định
                    }
                };
                }

                if (updateStudentDto.AdmissionDate != null && updateStudentDto.AdmissionDate <= DateOnly.FromDateTime(DateTime.Now))
                    student.AdmissionDate = updateStudentDto.AdmissionDate;

                if (!string.IsNullOrWhiteSpace(updateStudentDto.EnrollmentType) && updateStudentDto.EnrollmentType != "string")
                    student.EnrollmentType = updateStudentDto.EnrollmentType;

                if (!string.IsNullOrWhiteSpace(updateStudentDto.Ethnicity) && updateStudentDto.Ethnicity != "string")
                    student.Ethnicity = updateStudentDto.Ethnicity;

                if (!string.IsNullOrWhiteSpace(updateStudentDto.PermanentAddress) && updateStudentDto.PermanentAddress != "string")
                    student.PermanentAddress = updateStudentDto.PermanentAddress;

                if (!string.IsNullOrWhiteSpace(updateStudentDto.BirthPlace) && updateStudentDto.BirthPlace != "string")
                    student.BirthPlace = updateStudentDto.BirthPlace;

                if (!string.IsNullOrWhiteSpace(updateStudentDto.Religion) && updateStudentDto.Religion != "string")
                    student.Religion = updateStudentDto.Religion;

                if (updateStudentDto.RepeatingYear != null)
                    student.RepeatingYear = updateStudentDto.RepeatingYear.Value;

                if (!string.IsNullOrWhiteSpace(updateStudentDto.IdcardNumber) && updateStudentDto.IdcardNumber != "string")
                {
                    if (updateStudentDto.IdcardNumber != student.IdcardNumber)
                    {
                        bool exists = await _studentRepository.ExistsAsync(updateStudentDto.IdcardNumber);
                        if (exists)
                            throw new Exception($"Số CMND/CCCD {updateStudentDto.IdcardNumber} đã tồn tại.");
                    }
                    student.IdcardNumber = updateStudentDto.IdcardNumber;
                }

                if (!string.IsNullOrWhiteSpace(updateStudentDto.Status) && updateStudentDto.Status != "string")
                    student.Status = updateStudentDto.Status;

                // Kiểm tra xem có thông tin phụ huynh hợp lệ nào để cập nhật không
                bool hasValidParentInfo =
                    (!string.IsNullOrEmpty(updateStudentDto.FullNameFather) && updateStudentDto.FullNameFather != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.FullNameMother) && updateStudentDto.FullNameMother != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.FullNameGuardian) && updateStudentDto.FullNameGuardian != "string") ||
                    (updateStudentDto.YearOfBirthFather != null && updateStudentDto.YearOfBirthFather <= DateOnly.FromDateTime(DateTime.Now)) ||
                    (updateStudentDto.YearOfBirthMother != null && updateStudentDto.YearOfBirthMother <= DateOnly.FromDateTime(DateTime.Now)) ||
                    (updateStudentDto.YearOfBirthGuardian != null && updateStudentDto.YearOfBirthGuardian <= DateOnly.FromDateTime(DateTime.Now)) ||
                    (!string.IsNullOrEmpty(updateStudentDto.OccupationFather) && updateStudentDto.OccupationFather != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.OccupationMother) && updateStudentDto.OccupationMother != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.OccupationGuardian) && updateStudentDto.OccupationGuardian != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberFather) && updateStudentDto.PhoneNumberFather != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberMother) && updateStudentDto.PhoneNumberMother != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberGuardian) && updateStudentDto.PhoneNumberGuardian != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.EmailFather) && updateStudentDto.EmailFather != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.EmailMother) && updateStudentDto.EmailMother != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.EmailGuardian) && updateStudentDto.EmailGuardian != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.IdcardNumberFather) && updateStudentDto.IdcardNumberFather != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.IdcardNumberMother) && updateStudentDto.IdcardNumberMother != "string") ||
                    (!string.IsNullOrEmpty(updateStudentDto.IdcardNumberGuardian) && updateStudentDto.IdcardNumberGuardian != "string");

                if (hasValidParentInfo)
                {
                    await UpdateOrCreateParentAsync(student, updateStudentDto);
                }
                else
                {
                    Console.WriteLine("No valid parent information provided. Skipping parent update.");
                }

                await _studentRepository.UpdateAsync(student);
                Console.WriteLine($"Updated student with StudentID: {student.StudentId}");

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
                        // Lấy thông tin mới từ DTO
                        string newPhoneNumber = null;
                        if (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberFather) && updateStudentDto.PhoneNumberFather != "string")
                            newPhoneNumber = updateStudentDto.PhoneNumberFather;
                        else if (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberMother) && updateStudentDto.PhoneNumberMother != "string")
                            newPhoneNumber = updateStudentDto.PhoneNumberMother;
                        else if (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberGuardian) && updateStudentDto.PhoneNumberGuardian != "string")
                            newPhoneNumber = updateStudentDto.PhoneNumberGuardian;

                        string newEmail = null;
                        if (!string.IsNullOrEmpty(updateStudentDto.EmailFather) && updateStudentDto.EmailFather != "string")
                            newEmail = updateStudentDto.EmailFather;
                        else if (!string.IsNullOrEmpty(updateStudentDto.EmailMother) && updateStudentDto.EmailMother != "string")
                            newEmail = updateStudentDto.EmailMother;
                        else if (!string.IsNullOrEmpty(updateStudentDto.EmailGuardian) && updateStudentDto.EmailGuardian != "string")
                            newEmail = updateStudentDto.EmailGuardian;

                        // Kiểm tra xem thông tin cha mẹ mới có hoàn toàn giống với thông tin cha mẹ đã tồn tại nào không
                        Parent existingParent = null;
                        if (newPhoneNumber != null || newEmail != null)
                        {
                            existingParent = await _parentRepository.GetParentByDetailsAsync(
                                updateStudentDto.FullNameFather ?? parent.FullNameFather,
                                updateStudentDto.YearOfBirthFather ?? parent.YearOfBirthFather,
                                newPhoneNumber ?? user.PhoneNumber,
                                newEmail ?? user.Email,
                                updateStudentDto.IdcardNumberFather ?? parent.IdcardNumberFather
                            ) ?? await _parentRepository.GetParentByDetailsAsync(
                                updateStudentDto.FullNameMother ?? parent.FullNameMother,
                                updateStudentDto.YearOfBirthMother ?? parent.YearOfBirthMother,
                                newPhoneNumber ?? user.PhoneNumber,
                                newEmail ?? user.Email,
                                updateStudentDto.IdcardNumberMother ?? parent.IdcardNumberMother
                            ) ?? await _parentRepository.GetParentByDetailsAsync(
                                updateStudentDto.FullNameGuardian ?? parent.FullNameGuardian,
                                updateStudentDto.YearOfBirthGuardian ?? parent.YearOfBirthGuardian,
                                newPhoneNumber ?? user.PhoneNumber,
                                newEmail ?? user.Email,
                                updateStudentDto.IdcardNumberGuardian ?? parent.IdcardNumberGuardian
                            );
                        }

                        if (existingParent != null && existingParent.ParentId != parent.ParentId)
                        {
                            // Nếu tìm thấy phụ huynh giống hệt nhưng khác ParentId, liên kết với phụ huynh đó
                            Console.WriteLine($"Found existing parent with ParentID: {existingParent.ParentId}. Linking to student...");
                            student.ParentId = existingParent.ParentId;
                            await _studentRepository.UpdateAsync(student);
                            return;
                        }

                        // Kiểm tra trùng lặp số điện thoại
                        if (!string.IsNullOrEmpty(newPhoneNumber) && newPhoneNumber != user.PhoneNumber)
                        {
                            var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(newPhoneNumber);
                            if (existingUserByPhone != null && existingUserByPhone.UserId != user.UserId)
                                throw new Exception($"Số điện thoại {newPhoneNumber} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByPhone.UserId}).");
                            user.PhoneNumber = newPhoneNumber;
                        }

                        // Kiểm tra trùng lặp email
                        if (!string.IsNullOrEmpty(newEmail) && newEmail != user.Email)
                        {
                            var existingUserByEmail = await _userRepository.GetByEmailAsync(newEmail);
                            if (existingUserByEmail != null && existingUserByEmail.UserId != user.UserId)
                                throw new Exception($"Email {newEmail} đã được sử dụng bởi một người dùng khác (UserID: {existingUserByEmail.UserId}).");
                            user.Email = newEmail;
                        }

                        // Cập nhật username nếu có thay đổi tên
                        string fullNameForUsername = null;
                        if (!string.IsNullOrEmpty(updateStudentDto.FullNameFather) && updateStudentDto.FullNameFather != "string")
                            fullNameForUsername = updateStudentDto.FullNameFather;
                        else if (!string.IsNullOrEmpty(updateStudentDto.FullNameMother) && updateStudentDto.FullNameMother != "string")
                            fullNameForUsername = updateStudentDto.FullNameMother;
                        else if (!string.IsNullOrEmpty(updateStudentDto.FullNameGuardian) && updateStudentDto.FullNameGuardian != "string")
                            fullNameForUsername = updateStudentDto.FullNameGuardian;

                        if (!string.IsNullOrEmpty(fullNameForUsername))
                        {
                            string newUsername = FormatUserName.GenerateUsername(fullNameForUsername, user.UserId);
                            if (user.Username != newUsername)
                            {
                                var existingUserByUsername = await _userRepository.GetByUsernameAsync(newUsername);
                                if (existingUserByUsername != null && existingUserByUsername.UserId != user.UserId)
                                    newUsername += $"_{user.UserId}";
                                user.Username = newUsername;
                            }
                        }

                        await _userRepository.UpdateAsync(user);
                        Console.WriteLine($"Updated user with UserID: {user.UserId}");

                        // Cập nhật thông tin cha
                        if (!string.IsNullOrEmpty(updateStudentDto.FullNameFather) && updateStudentDto.FullNameFather != "string")
                            parent.FullNameFather = updateStudentDto.FullNameFather;
                        if (updateStudentDto.YearOfBirthFather != null && updateStudentDto.YearOfBirthFather <= DateOnly.FromDateTime(DateTime.Now))
                            parent.YearOfBirthFather = updateStudentDto.YearOfBirthFather;
                        if (!string.IsNullOrEmpty(updateStudentDto.OccupationFather) && updateStudentDto.OccupationFather != "string")
                            parent.OccupationFather = updateStudentDto.OccupationFather;
                        if (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberFather) && updateStudentDto.PhoneNumberFather != "string")
                            parent.PhoneNumberFather = updateStudentDto.PhoneNumberFather;
                        if (!string.IsNullOrEmpty(updateStudentDto.EmailFather) && updateStudentDto.EmailFather != "string")
                            parent.EmailFather = updateStudentDto.EmailFather;
                        if (!string.IsNullOrEmpty(updateStudentDto.IdcardNumberFather) && updateStudentDto.IdcardNumberFather != "string")
                            parent.IdcardNumberFather = updateStudentDto.IdcardNumberFather;

                        // Cập nhật thông tin mẹ
                        if (!string.IsNullOrEmpty(updateStudentDto.FullNameMother) && updateStudentDto.FullNameMother != "string")
                            parent.FullNameMother = updateStudentDto.FullNameMother;
                        if (updateStudentDto.YearOfBirthMother != null && updateStudentDto.YearOfBirthMother <= DateOnly.FromDateTime(DateTime.Now))
                            parent.YearOfBirthMother = updateStudentDto.YearOfBirthMother;
                        if (!string.IsNullOrEmpty(updateStudentDto.OccupationMother) && updateStudentDto.OccupationMother != "string")
                            parent.OccupationMother = updateStudentDto.OccupationMother;
                        if (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberMother) && updateStudentDto.PhoneNumberMother != "string")
                            parent.PhoneNumberMother = updateStudentDto.PhoneNumberMother;
                        if (!string.IsNullOrEmpty(updateStudentDto.EmailMother) && updateStudentDto.EmailMother != "string")
                            parent.EmailMother = updateStudentDto.EmailMother;
                        if (!string.IsNullOrEmpty(updateStudentDto.IdcardNumberMother) && updateStudentDto.IdcardNumberMother != "string")
                            parent.IdcardNumberMother = updateStudentDto.IdcardNumberMother;

                        // Cập nhật thông tin người bảo hộ
                        if (!string.IsNullOrEmpty(updateStudentDto.FullNameGuardian) && updateStudentDto.FullNameGuardian != "string")
                            parent.FullNameGuardian = updateStudentDto.FullNameGuardian;
                        if (updateStudentDto.YearOfBirthGuardian != null && updateStudentDto.YearOfBirthGuardian <= DateOnly.FromDateTime(DateTime.Now))
                            parent.YearOfBirthGuardian = updateStudentDto.YearOfBirthGuardian;
                        if (!string.IsNullOrEmpty(updateStudentDto.OccupationGuardian) && updateStudentDto.OccupationGuardian != "string")
                            parent.OccupationGuardian = updateStudentDto.OccupationGuardian;
                        if (!string.IsNullOrEmpty(updateStudentDto.PhoneNumberGuardian) && updateStudentDto.PhoneNumberGuardian != "string")
                            parent.PhoneNumberGuardian = updateStudentDto.PhoneNumberGuardian;
                        if (!string.IsNullOrEmpty(updateStudentDto.EmailGuardian) && updateStudentDto.EmailGuardian != "string")
                            parent.EmailGuardian = updateStudentDto.EmailGuardian;
                        if (!string.IsNullOrEmpty(updateStudentDto.IdcardNumberGuardian) && updateStudentDto.IdcardNumberGuardian != "string")
                            parent.IdcardNumberGuardian = updateStudentDto.IdcardNumberGuardian;

                        await _parentRepository.UpdateAsync(parent);
                        Console.WriteLine($"Updated parent with ParentID: {parent.ParentId}");
                    }
                }
                else
                {
                    await CreateParentAndLinkAsync(student, updateStudentDto);
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
                            StudentClasses = new List<Domain.Models.StudentClass>
                    {
                        new Domain.Models.StudentClass
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

                            // Kiểm tra thông tin đầy đủ cho cha, mẹ, người bảo hộ trước khi gán vào DTO
                            string fatherName = row.TryGetValue("Họ và tên cha", out var fnFather) ? fnFather.Trim() : null;
                            DateOnly? fatherDob = row.TryGetValue("Ngày sinh cha", out var fatherDobStr) && !string.IsNullOrEmpty(fatherDobStr) ? DateHelper.ParseDate(fatherDobStr) : null;
                            string fatherPhone = row.TryGetValue("SĐT cha", out var fatherPh) ? fatherPh.Trim().Replace(".0", "") : null; // Xử lý .0
                            string fatherIdCard = row.TryGetValue("Số CCCD cha", out var fatherId) ? fatherId.Trim().Replace(".0", "") : null; // Xử lý .0
                            string fatherOccupation = row.TryGetValue("Nghề nghiệp cha", out var fatherOcc) ? fatherOcc.Trim() : null;
                            string fatherEmail = row.TryGetValue("Email cha", out var fatherEm) ? fatherEm.Trim() : null;

                            string motherName = row.TryGetValue("Họ và tên mẹ", out var fnMother) ? fnMother.Trim() : null;
                            DateOnly? motherDob = row.TryGetValue("Ngày sinh mẹ", out var motherDobStr) && !string.IsNullOrEmpty(motherDobStr) ? DateHelper.ParseDate(motherDobStr) : null;
                            string motherPhone = row.TryGetValue("SĐT mẹ", out var motherPh) ? motherPh.Trim().Replace(".0", "") : null; // Xử lý .0
                            string motherIdCard = row.TryGetValue("Số CCCD mẹ", out var motherId) ? motherId.Trim().Replace(".0", "") : null; // Xử lý .0
                            string motherOccupation = row.TryGetValue("Nghề nghiệp mẹ", out var motherOcc) ? motherOcc.Trim() : null;
                            string motherEmail = row.TryGetValue("Email mẹ", out var motherEm) ? motherEm.Trim() : null;

                            string guardianName = row.TryGetValue("Họ và tên người bảo hộ", out var fnGuardian) ? fnGuardian.Trim() : null;
                            DateOnly? guardianDob = row.TryGetValue("Ngày sinh người bảo hộ", out var guardianDobStr) && !string.IsNullOrEmpty(guardianDobStr) ? DateHelper.ParseDate(guardianDobStr) : null;
                            string guardianPhone = row.TryGetValue("SĐT người bảo hộ", out var guardianPh) ? guardianPh.Trim().Replace(".0", "") : null; // Xử lý .0
                            string guardianIdCard = row.TryGetValue("Số CCCD người bảo hộ", out var guardianId) ? guardianId.Trim().Replace(".0", "") : null; // Xử lý .0
                            string guardianOccupation = row.TryGetValue("Nghề nghiệp người bảo hộ", out var guardianOcc) ? guardianOcc.Trim() : null;
                            string guardianEmail = row.TryGetValue("Email người bảo hộ", out var guardianEm) ? guardianEm.Trim() : null;

                            // Log dữ liệu thô từ Excel
                            Console.WriteLine($"Raw Parent Data for {student.FullName} - Father: {fatherName}, Phone: {fatherPhone}, IDCard: {fatherIdCard}, DOB: {fatherDob}");
                            Console.WriteLine($"Raw Parent Data for {student.FullName} - Mother: {motherName}, Phone: {motherPhone}, IDCard: {motherIdCard}, DOB: {motherDob}");
                            Console.WriteLine($"Raw Parent Data for {student.FullName} - Guardian: {guardianName}, Phone: {guardianPhone}, IDCard: {guardianIdCard}, DOB: {guardianDob}");

                            // Kiểm tra tính đầy đủ của thông tin cha, mẹ, người bảo hộ
                            bool hasCompleteFatherInfo = !string.IsNullOrEmpty(fatherName) &&
                                                        fatherDob.HasValue &&
                                                        !string.IsNullOrEmpty(fatherPhone) &&
                                                        !string.IsNullOrEmpty(fatherIdCard);

                            bool hasCompleteMotherInfo = !string.IsNullOrEmpty(motherName) &&
                                                        motherDob.HasValue &&
                                                        !string.IsNullOrEmpty(motherPhone) &&
                                                        !string.IsNullOrEmpty(motherIdCard);

                            bool hasCompleteGuardianInfo = !string.IsNullOrEmpty(guardianName) &&
                                                          guardianDob.HasValue &&
                                                          !string.IsNullOrEmpty(guardianPhone) &&
                                                          !string.IsNullOrEmpty(guardianIdCard);

                            // Log kết quả kiểm tra
                            Console.WriteLine($"Parent Completeness for {student.FullName} - Father: {hasCompleteFatherInfo}, Mother: {hasCompleteMotherInfo}, Guardian: {hasCompleteGuardianInfo}");

                            // Nếu thông tin không đầy đủ, đặt các trường tương ứng thành null
                            var parentInfoDto = new ParentInfoDto
                            {
                                FullNameFather = hasCompleteFatherInfo ? fatherName : null,
                                YearOfBirthFather = hasCompleteFatherInfo ? fatherDob : null,
                                OccupationFather = hasCompleteFatherInfo ? fatherOccupation : null,
                                PhoneNumberFather = hasCompleteFatherInfo ? fatherPhone : null,
                                EmailFather = hasCompleteFatherInfo ? fatherEmail : null,
                                IdcardNumberFather = hasCompleteFatherInfo ? fatherIdCard : null,

                                FullNameMother = hasCompleteMotherInfo ? motherName : null,
                                YearOfBirthMother = hasCompleteMotherInfo ? motherDob : null,
                                OccupationMother = hasCompleteMotherInfo ? motherOccupation : null,
                                PhoneNumberMother = hasCompleteMotherInfo ? motherPhone : null,
                                EmailMother = hasCompleteMotherInfo ? motherEmail : null,
                                IdcardNumberMother = hasCompleteMotherInfo ? motherIdCard : null,

                                FullNameGuardian = hasCompleteGuardianInfo ? guardianName : null,
                                YearOfBirthGuardian = hasCompleteGuardianInfo ? guardianDob : null,
                                OccupationGuardian = hasCompleteGuardianInfo ? guardianOccupation : null,
                                PhoneNumberGuardian = hasCompleteGuardianInfo ? guardianPhone : null,
                                EmailGuardian = hasCompleteGuardianInfo ? guardianEmail : null,
                                IdcardNumberGuardian = hasCompleteGuardianInfo ? guardianIdCard : null
                            };

                            // Log giá trị của parentInfoDto
                            Console.WriteLine($"ParentInfoDto for {student.FullName} - Father: {parentInfoDto.FullNameFather}, Mother: {parentInfoDto.FullNameMother}, Guardian: {parentInfoDto.FullNameGuardian}");
                            Console.WriteLine($"Mother Details - YearOfBirth: {parentInfoDto.YearOfBirthMother}, Phone: {parentInfoDto.PhoneNumberMother}, IDCard: {parentInfoDto.IdcardNumberMother}");

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


