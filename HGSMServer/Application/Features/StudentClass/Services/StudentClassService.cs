using Application.Features.AcademicYears.DTOs;
using Application.Features.Classes.DTOs;
using Application.Features.Semesters.DTOs;
using Application.Features.StudentClass.DTOs;
using Application.Features.StudentClass.Interfaces;
using Application.Features.Students.DTOs;
using Domain.Models;
using Infrastructure.Repositories.Implementtations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClassDto = Application.Features.StudentClass.DTOs.ClassDto;

namespace Application.Features.StudentClass.Services
{
    public class StudentClassService : IStudentClassService
    {
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IAcademicYearRepository _academicYearRepository;
        private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly IConductRepository _conductRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HgsdbContext _context;

        public StudentClassService(
            IStudentClassRepository studentClassRepository,
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            ITeachingAssignmentRepository teachingAssignmentRepository,
            ISemesterRepository semesterRepository,
            IAcademicYearRepository academicYearRepository,
            IGradeRepository gradeRepository,
            IConductRepository conductRepository,
            IHttpContextAccessor httpContextAccessor,
            HgsdbContext context)
        {
            _studentClassRepository = studentClassRepository;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _teachingAssignmentRepository = teachingAssignmentRepository;
            _academicYearRepository = academicYearRepository;
            _semesterRepository = semesterRepository;
            _gradeRepository = gradeRepository;
            _conductRepository = conductRepository;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private async Task<bool> HasPermissionAsync()
        {
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            var allowedRoles = new[] { "Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư" };
            return allowedRoles.Contains(userRole);
        }
        private async Task<bool> HasReadPermissionAsync()
        {
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            var allowedRoles = new[] { "Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư", "Giáo viên", "Phụ huynh","Trưởng bộ môn" };
            return allowedRoles.Contains(userRole);
        }


        private async Task ValidateAcademicYearAsync(int academicYearId, bool mustBeActive = true)
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
            if (academicYear == null)
            {
                throw new ArgumentException($"Năm học với Id {academicYearId} không tồn tại.");
            }

            if (mustBeActive && (academicYear.StartDate > DateOnly.FromDateTime(DateTime.Now) || academicYear.EndDate < DateOnly.FromDateTime(DateTime.Now)))
            {
                throw new InvalidOperationException($"Năm học {academicYear.YearName} không hoạt động.");
            }
        }

        private async Task<AcademicYear> GetCurrentAcademicYearAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var academicYears = await _academicYearRepository.GetAllAsync();
            var currentAcademicYear = academicYears.FirstOrDefault(ay => ay.StartDate <= today && ay.EndDate >= today);

            if (currentAcademicYear == null)
            {
                throw new InvalidOperationException("Không tìm thấy năm học đang hoạt động cho ngày hiện tại.");
            }

            return currentAcademicYear;
        }

        private async Task<bool> IsNextAcademicYearAsync(int currentAcademicYearId, int targetAcademicYearId)
        {
            var currentYear = await _academicYearRepository.GetByIdAsync(currentAcademicYearId);
            var targetYear = await _academicYearRepository.GetByIdAsync(targetAcademicYearId);

            if (currentYear == null || targetYear == null)
            {
                return false;
            }
            // Kiểm tra xem targetYear có phải là năm ngay sau currentYear không
            var academicYears = await _academicYearRepository.GetAllAsync();
            var sortedAcademicYears = academicYears.OrderBy(ay => ay.StartDate).ToList();
            var currentIndex = sortedAcademicYears.FindIndex(ay => ay.AcademicYearId == currentAcademicYearId);

            if (currentIndex == -1 || currentIndex + 1 >= sortedAcademicYears.Count)
            {
                return false; 
            }

            return sortedAcademicYears[currentIndex + 1].AcademicYearId == targetAcademicYearId;
        }

        private async Task<bool> IsPreviousAcademicYearAsync(int academicYearId)
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
            if (academicYear == null)
            {
                return false;
            }

            var today = DateOnly.FromDateTime(DateTime.Now);
            return academicYear.EndDate < today;
        }
        private async Task<AcademicYear?> GetPreviousAcademicYearByIdAsync(int nextAcademicYearId)
        {
            var nextAcademicYear = await _academicYearRepository.GetByIdAsync(nextAcademicYearId);
            if (nextAcademicYear == null)
            {
                return null;
            }

            var academicYears = await _academicYearRepository.GetAllAsync();
            var previousAcademicYear = academicYears
                .Where(ay => ay.EndDate < nextAcademicYear.StartDate) // Năm học kết thúc trước khi năm học sau bắt đầu
                .OrderByDescending(ay => ay.EndDate) // Lấy năm học gần nhất (liền trước)
                .FirstOrDefault();

            return previousAcademicYear;
        }

        private async Task<bool> IsWithinSemester1OfAcademicYearAsync(int academicYearId)
        {
            var semestersInYear = await _semesterRepository.GetByAcademicYearIdAsync(academicYearId);
            var semester1 = semestersInYear.FirstOrDefault(s => s.SemesterName == "Học kỳ 1");

            if (semester1 == null)
            {
                return false;
            }

            var today = DateOnly.FromDateTime(DateTime.Now);
            return today >= semester1.StartDate && today <= semester1.EndDate;
        }
        private async Task ValidateTransferTimeWindowAsync(int sourceAcademicYearId, int targetAcademicYearId)
        {
            var sourceAcademicYear = await _academicYearRepository.GetByIdAsync(sourceAcademicYearId);
            var targetAcademicYear = await _academicYearRepository.GetByIdAsync(targetAcademicYearId);

            if (sourceAcademicYear == null || targetAcademicYear == null)
            {
                throw new ArgumentException($"Năm học nguồn (ID: {sourceAcademicYearId}) hoặc năm học đích (ID: {targetAcademicYearId}) không tồn tại.");
            }

            var semestersInTargetYear = await _semesterRepository.GetByAcademicYearIdAsync(targetAcademicYearId);
            var semester1 = semestersInTargetYear.FirstOrDefault(s => s.SemesterName == "Học kỳ 1");

            if (semester1 == null)
            {
                throw new InvalidOperationException($"Không tìm thấy Học kỳ 1 cho năm học đích {targetAcademicYear.YearName}.");
            }

            var today = DateOnly.FromDateTime(DateTime.Now);
            if (today < sourceAcademicYear.EndDate || today > semester1.EndDate)
            {
                throw new InvalidOperationException($"Chuyển lớp chỉ được thực hiện từ khi kết thúc năm học nguồn ({sourceAcademicYear.EndDate}) đến hết Học kỳ 1 của năm học đích ({semester1.EndDate}).");
            }
        }

        public async Task<IEnumerable<StudentClassResponseDto>> GetAllStudentClassesAsync()
        {
            if (!await HasReadPermissionAsync()) 
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập dữ liệu này.");
            }

            var studentClasses = await _studentClassRepository.GetAllAsync();

            var studentClassDtos = studentClasses.Select(sc => new StudentClassResponseDto
            {
                Id = sc.Id,
                StudentId = sc.StudentId,
                StudentName = sc.Student?.FullName ?? "N/A", 
                ClassId = sc.ClassId,
                ClassName = sc.Class?.ClassName ?? "N/A", 
                AcademicYearId = sc.AcademicYearId,
                YearName = sc.AcademicYear?.YearName ?? "N/A" 
            }).ToList();

            return studentClassDtos;
        }
        public async Task<IEnumerable<StudentClassResponseDto>> GetAllStudentClassByLastAcademicYearAsync(int currentAcademicYearId)
        {
            if (!await HasReadPermissionAsync()) 
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập dữ liệu này.");
            }

            // Tìm năm học liền trước năm học hiện tại được truyền vào
            var lastAcademicYear = await GetPreviousAcademicYearByIdAsync(currentAcademicYearId);

            if (lastAcademicYear == null)
            {
                return Enumerable.Empty<StudentClassResponseDto>();
            }
            var studentClassesInLastYear = await _studentClassRepository.GetByAcademicYearIdAsync(lastAcademicYear.AcademicYearId);

            if (!studentClassesInLastYear.Any())
            {
                return Enumerable.Empty<StudentClassResponseDto>();
            }

            var studentClassDtos = studentClassesInLastYear.Select(sc => new StudentClassResponseDto
            {
                Id = sc.Id,
                StudentId = sc.StudentId,
                StudentName = sc.Student?.FullName ?? "N/A",
                ClassId = sc.ClassId,
                ClassName = sc.Class?.ClassName ?? "N/A",
                AcademicYearId = sc.AcademicYearId,
                YearName = sc.AcademicYear?.YearName ?? "N/A"
            }).ToList();

            return studentClassDtos;
        }

        public async Task CreateStudentClassAsync(StudentClassAssignmentDto dto)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền phân công lớp.");
            }

            // Tự động lấy năm học hiện tại nếu AcademicYearId không được cung cấp
            int academicYearId = dto.AcademicYearId ?? (await GetCurrentAcademicYearAsync()).AcademicYearId;

            // Validate: Chỉ cho phép phân công mới trong thời gian của Học kỳ 1 của năm học đích
            if (!await IsWithinSemester1OfAcademicYearAsync(academicYearId))
            {
                var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
                throw new InvalidOperationException($"Chỉ có thể phân công lớp học sinh trong thời gian Học kỳ 1 của năm học {academicYear?.YearName}.");
            }

            var student = await _studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null)
            {
                throw new ArgumentException($"Học sinh với Id {dto.StudentId} không tồn tại.");
            }

            if (student.Status == "Tốt nghiệp")
            {
                throw new InvalidOperationException($"Học sinh {student.FullName} đã tốt nghiệp và không thể được phân công vào lớp.");
            }

            var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
            if (classEntity == null)
            {
                throw new ArgumentException($"Lớp với Id {dto.ClassId} không tồn tại.");
            }

            // Validate năm học đích tồn tại
            await ValidateAcademicYearAsync(academicYearId, mustBeActive: false);

            var existingAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, academicYearId);
            if (existingAssignment != null)
            {
                throw new InvalidOperationException($"Học sinh {student.FullName} đã được phân công vào lớp {existingAssignment.Class.ClassName} trong năm học {existingAssignment.AcademicYear.YearName}.");
            }

            var assignment = new Domain.Models.StudentClass
            {
                StudentId = dto.StudentId,
                ClassId = dto.ClassId,
                AcademicYearId = academicYearId
            };

            await _studentClassRepository.AddAsync(assignment);
        }


        public async Task UpdateStudentClassesAsync(List<StudentClassAssignmentDto> dtos)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật phân công lớp.");
            }

            if (!dtos.Any())
            {
                throw new ArgumentException("Danh sách phân công lớp không được rỗng.");
            }
            var distinctStudentIds = dtos.Select(d => d.StudentId).Distinct().Count();
            if (distinctStudentIds < dtos.Count)
            {
                throw new ArgumentException("Tìm thấy Id học sinh trùng lặp trong danh sách phân công lớp.");
            }

            var updatedAssignments = new List<Domain.Models.StudentClass>();

            using var transaction = await _studentClassRepository.BeginTransactionAsync();
            try
            {
                foreach (var dto in dtos)
                {
                    // Tự động lấy năm học hiện tại nếu AcademicYearId không được cung cấp
                    int academicYearId = dto.AcademicYearId ?? (await GetCurrentAcademicYearAsync()).AcademicYearId;

                    // Validate: Chỉ cho phép cập nhật trong thời gian của Học kỳ 1 của năm học đích
                    if (!await IsWithinSemester1OfAcademicYearAsync(academicYearId))
                    {
                        var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
                        throw new InvalidOperationException($"Chỉ có thể cập nhật phân công lớp trong thời gian Học kỳ 1 của năm học {academicYear?.YearName}.");
                    }

                    if (await IsPreviousAcademicYearAsync(academicYearId))
                    {
                        throw new InvalidOperationException($"Không thể cập nhật phân công lớp cho năm học trước (ID: {academicYearId}).");
                    }

                    var student = await _studentRepository.GetByIdAsync(dto.StudentId);
                    if (student == null)
                    {
                        throw new ArgumentException($"Học sinh với Id {dto.StudentId} không tồn tại.");
                    }

                    if (student.Status == "Tốt nghiệp")
                    {
                        throw new InvalidOperationException($"Học sinh {student.FullName} đã tốt nghiệp và không thể được phân công vào lớp.");
                    }

                    var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
                    if (classEntity == null)
                    {
                        throw new ArgumentException($"Lớp với Id {dto.ClassId} không tồn tại.");
                    }
                    if (classEntity.Status != "Hoạt động")
                    {
                        throw new InvalidOperationException($"Lớp {classEntity.ClassName} không hoạt động.");
                    }

                    // Validate năm học đích tồn tại
                    await ValidateAcademicYearAsync(academicYearId, mustBeActive: false);

                    var existingAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, academicYearId);
                    if (existingAssignment != null)
                    {
                        if (existingAssignment.ClassId == dto.ClassId)
                        {
                            throw new InvalidOperationException($"Học sinh {student.FullName} đã được phân công vào lớp {classEntity.ClassName} trong năm học này. Không thể chuyển sang lớp trùng với lớp hiện tại.");
                        }

                        await _studentClassRepository.DeleteAsync(existingAssignment.Id);
                    }

                    var newAssignment = new Domain.Models.StudentClass
                    {
                        StudentId = dto.StudentId,
                        ClassId = dto.ClassId,
                        AcademicYearId = academicYearId
                    };
                    updatedAssignments.Add(newAssignment);
                }

                if (updatedAssignments.Any())
                {
                    await _studentClassRepository.AddRangeAsync(updatedAssignments);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private async Task<int> GetNextAcademicYearIdAsync(int currentAcademicYearId)
        {
            var academicYears = await _academicYearRepository.GetAllAsync();
            var sortedAcademicYears = academicYears.OrderBy(ay => ay.StartDate).ToList();
            var currentIndex = sortedAcademicYears.FindIndex(ay => ay.AcademicYearId == currentAcademicYearId);
            if (currentIndex < sortedAcademicYears.Count - 1)
            {
                return sortedAcademicYears[currentIndex + 1].AcademicYearId;
            }
            throw new InvalidOperationException("Không tìm thấy năm học tiếp theo để chuyển lớp.");
        }
        public async Task<BulkTransferResultDto> BulkTransferClassAsync(BulkClassTransferDto dto)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền chuyển lớp hàng loạt.");
            }

            // Tự động lấy năm học hiện tại nếu AcademicYearId không được cung cấp
            int sourceAcademicYearId = dto.AcademicYearId ?? (await GetCurrentAcademicYearAsync()).AcademicYearId;

            // Tự động lấy năm học tiếp theo nếu TargetAcademicYearId không được cung cấp
            int targetAcademicYearId = dto.TargetAcademicYearId ?? await GetNextAcademicYearIdAsync(sourceAcademicYearId);

            // Validate năm học nguồn và đích
            await ValidateAcademicYearAsync(sourceAcademicYearId, mustBeActive: false);
            await ValidateAcademicYearAsync(targetAcademicYearId, mustBeActive: false);

            // Kiểm tra năm học đích có phải là năm tiếp theo của năm nguồn không
            bool isNextYearTransfer = await IsNextAcademicYearAsync(sourceAcademicYearId, targetAcademicYearId);
            if (!isNextYearTransfer)
            {
                throw new InvalidOperationException($"Chuyển lớp hàng loạt chỉ được thực hiện giữa hai năm học liền kề. Năm học đích (ID: {targetAcademicYearId}) không phải là năm học tiếp theo của năm học nguồn (ID: {sourceAcademicYearId}).");
            }

            // Validate thời gian chuyển lớp
            await ValidateTransferTimeWindowAsync(sourceAcademicYearId, targetAcademicYearId);

            var currentClass = await _classRepository.GetByIdAsync(dto.ClassId);
            if (currentClass == null)
            {
                throw new ArgumentException($"Lớp nguồn với Id {dto.ClassId} không tồn tại.");
            }

            if (currentClass.GradeLevelId == 4) // Khối 9
            {
                throw new InvalidOperationException($"Học sinh trong lớp {currentClass.ClassName} (Khối 9) cần được xử lý tốt nghiệp thay vì chuyển lớp.");
            }

            var targetClass = await _classRepository.GetByIdAsync(dto.TargetClassId);
            if (targetClass == null)
            {
                throw new ArgumentException($"Lớp đích với Id {dto.TargetClassId} không tồn tại.");
            }

            if (targetClass.Status != "Hoạt động")
            {
                throw new InvalidOperationException($"Lớp đích {targetClass.ClassName} không hoạt động.");
            }

            // Kiểm tra khối lớp đích có phải là khối tiếp theo của khối hiện tại không
            var currentGradeLevel = currentClass.GradeLevelId;
            if (targetClass.GradeLevelId != currentGradeLevel + 1)
            {
                throw new InvalidOperationException($"Học sinh chỉ có thể chuyển lên 1 khối. Không thể chuyển từ khối {currentGradeLevel} sang khối {targetClass.GradeLevelId}.");
            }

            var studentsInClass = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(dto.ClassId, sourceAcademicYearId);
            if (!studentsInClass.Any())
            {
                throw new InvalidOperationException($"Không tìm thấy học sinh trong lớp {currentClass.ClassName} cho năm học {sourceAcademicYearId}.");
            }

            var newAssignments = new List<Domain.Models.StudentClass>();
            var skippedStudents = new List<SkippedStudentDto>();
            var successfullyTransferredStudents = new List<StudentDto>();

            using var transaction = await _studentClassRepository.BeginTransactionAsync();
            try
            {
                foreach (var studentClass in studentsInClass)
                {
                    var student = await _studentRepository.GetByIdAsync(studentClass.StudentId);
                    if (student == null)
                    {
                        skippedStudents.Add(new SkippedStudentDto { StudentId = studentClass.StudentId, FullName = "N/A", Reason = "Không tìm thấy thông tin học sinh." });
                        continue;
                    }
                    if (student.Status == "Tốt nghiệp")
                    {
                        skippedStudents.Add(new SkippedStudentDto { StudentId = student.StudentId, FullName = student.FullName, Reason = "Học sinh đã tốt nghiệp." });
                        continue;
                    }

                    var existingAssignmentInTargetYear = await _studentClassRepository.GetByStudentAndAcademicYearAsync(student.StudentId, targetAcademicYearId);
                    if (existingAssignmentInTargetYear != null)
                    {
                        skippedStudents.Add(new SkippedStudentDto { StudentId = student.StudentId, FullName = student.FullName, Reason = $"Đã được phân công vào lớp {existingAssignmentInTargetYear.Class?.ClassName ?? "N/A"} trong năm học {existingAssignmentInTargetYear.AcademicYear?.YearName ?? "N/A"}." });
                        continue;
                    }

                    try
                    {
                        await CheckStudentPromotionEligibilityAsync(student.StudentId, sourceAcademicYearId);
                        newAssignments.Add(new Domain.Models.StudentClass
                        {
                            StudentId = student.StudentId,
                            ClassId = dto.TargetClassId,
                            AcademicYearId = targetAcademicYearId
                        });

                        successfullyTransferredStudents.Add(new StudentDto
                        {
                            StudentId = student.StudentId,
                            FullName = student.FullName,
                            Dob = student.Dob,
                            Gender = student.Gender,
                            AdmissionDate = student.AdmissionDate,
                            EnrollmentType = student.EnrollmentType,
                            Ethnicity = student.Ethnicity,
                            PermanentAddress = student.PermanentAddress,
                            BirthPlace = student.BirthPlace,
                            Religion = student.Religion,
                            RepeatingYear = student.RepeatingYear,
                            IdcardNumber = student.IdcardNumber,
                            Status = student.Status
                        });
                    }
                    catch (InvalidOperationException ex)
                    {
                        skippedStudents.Add(new SkippedStudentDto { StudentId = student.StudentId, FullName = student.FullName, Reason = ex.Message });
                        continue;
                    }
                    catch (Exception ex)
                    {
                        skippedStudents.Add(new SkippedStudentDto { StudentId = student.StudentId, FullName = student.FullName, Reason = $"Gặp lỗi khi kiểm tra điều kiện: {ex.Message}" });
                        continue;
                    }
                }

                if (newAssignments.Any())
                {
                    await _studentClassRepository.AddRangeAsync(newAssignments);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException($"Lỗi hệ thống trong quá trình chuyển lớp hàng loạt: {ex.Message}");
            }

            return new BulkTransferResultDto
            {
                TotalStudentsProcessed = studentsInClass.Count(),
                SuccessfulCount = successfullyTransferredStudents.Count,
                SkippedCount = skippedStudents.Count,
                SuccessfullyTransferredStudents = successfullyTransferredStudents,
                SkippedStudents = skippedStudents
            };
        }




        public async Task DeleteStudentClassAsync(int id)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa phân công lớp.");
            }

            var assignment = await _studentClassRepository.GetByIdAsync(id);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Phân công lớp với Id {id} không tồn tại.");
            }

            await _studentClassRepository.DeleteAsync(id);
        }
        public async Task<StudentClassFilterDataDto> GetFilterDataAsync(int? classId = null, int? semesterId = null)
        {
            if (!await HasReadPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập dữ liệu này.");
            }

            List<StudentFilterDto> students;

            if (classId.HasValue && semesterId.HasValue)
            {
                var semesterr = await _semesterRepository.GetByIdAsync(semesterId.Value);
                if (semesterr == null)
                {
                    throw new ArgumentException($"Học kỳ với Id {semesterId.Value} không tồn tại.");
                }

                var studentClasses = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(classId.Value, semesterr.AcademicYearId);
                students = studentClasses
                    .Select(sc => new StudentFilterDto
                    {
                        StudentClassId = sc.Id, // Gán StudentClassId từ bản ghi phân công
                        StudentId = sc.Student.StudentId,
                        FullName = sc.Student.FullName,
                        Status = sc.Student.Status
                    })
                    .ToList();
            }
            else
            {
                // Lấy năm học hiện tại
                var currentAcademicYear = await GetCurrentAcademicYearAsync();
                int currentAcademicYearId = currentAcademicYear.AcademicYearId;

                // Lấy tất cả học sinh đã được phân công lớp trong năm học hiện tại
                var studentClasses = await _studentClassRepository.GetByAcademicYearIdAsync(currentAcademicYearId);
                students = studentClasses
                    .Select(sc => new StudentFilterDto
                    {
                        StudentClassId = sc.Id, // Gán StudentClassId từ bản ghi phân công
                        StudentId = sc.Student.StudentId,
                        FullName = sc.Student.FullName,
                        Status = sc.Student.Status
                    })
                    .ToList();
            }

            var classes = classId.HasValue
                ? new List<Domain.Models.Class> { await _classRepository.GetByIdAsync(classId.Value) }
                : await _classRepository.GetAllAsync();

            if (classId.HasValue && classes.FirstOrDefault() == null)
            {
                throw new ArgumentException($"Lớp với Id {classId.Value} không tồn tại.");
            }

            var classDtos = new List<ClassDto>();

            foreach (var classEntity in classes.Where(c => c != null))
            {
                int studentCount = 0;
                var classDto = new ClassDto
                {
                    ClassId = classEntity.ClassId,
                    ClassName = classEntity.ClassName,
                    GradeLevelId = classEntity.GradeLevelId,
                    StudentCount = studentCount,
                    Status = classEntity.Status,
                    HomeroomTeachers = new List<HomeroomTeacherInfo>()
                };

                if (semesterId.HasValue)
                {
                    var semesterr = await _semesterRepository.GetByIdAsync(semesterId.Value);
                    if (semesterr != null)
                    {
                        var studentCountQuery = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(classEntity.ClassId, semesterr.AcademicYearId);
                        studentCount = studentCountQuery.Count();

                        var homeroomAssignment = await _context.HomeroomAssignments
                            .Where(ha => ha.ClassId == classEntity.ClassId && ha.SemesterId == semesterId.Value)
                            .Include(ha => ha.Teacher)
                            .FirstOrDefaultAsync();

                        classDto.HomeroomTeachers.Add(new HomeroomTeacherInfo
                        {
                            SemesterId = semesterr.SemesterId,
                            SemesterName = semesterr.SemesterName,
                            TeacherName = homeroomAssignment?.Teacher?.FullName ?? "Chưa có"
                        });

                        var otherSemesters = await _semesterRepository.GetByAcademicYearIdAsync(semesterr.AcademicYearId);
                        foreach (var otherSemester in otherSemesters.Where(s => s.SemesterId != semesterId.Value))
                        {
                            var otherHomeroomAssignment = await _context.HomeroomAssignments
                                .Where(ha => ha.ClassId == classEntity.ClassId && ha.SemesterId == otherSemester.SemesterId)
                                .Include(ha => ha.Teacher)
                                .FirstOrDefaultAsync();

                            classDto.HomeroomTeachers.Add(new HomeroomTeacherInfo
                            {
                                SemesterId = otherSemester.SemesterId,
                                SemesterName = otherSemester.SemesterName,
                                TeacherName = otherHomeroomAssignment?.Teacher?.FullName ?? "Chưa có"
                            });
                        }
                    }
                }
                else
                {
                    var studentCountQuery = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(classEntity.ClassId, 0);
                    studentCount = studentCountQuery.Count();
                }

                classDto.StudentCount = studentCount;
                classDtos.Add(classDto);
            }

            var semester = await _semesterRepository.GetByIdAsync(semesterId ?? 1);
            if (semester == null)
            {
                throw new ArgumentException($"Học kỳ với Id {(semesterId ?? 1)} không tồn tại.");
            }

            var semesterDto = new SemesterDto
            {
                SemesterID = semester.SemesterId,
                SemesterName = semester.SemesterName,
                AcademicYearID = semester.AcademicYearId,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate
            };

            var filterData = new StudentClassFilterDataDto
            {
                Students = students,
                Classes = classDtos.OrderBy(c => c.ClassName).ToList(),
                Semesters = new List<SemesterDto> { semesterDto }
            };

            return filterData;
        }

        public async Task<List<StudentClass.DTOs.ClassDto>> GetClassesWithStudentCountAsync(int? academicYearId = null)
        {
            if (!await HasReadPermissionAsync()) 
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập thông tin lớp.");
            }

            var classes = await _classRepository.GetAllAsync();
            var result = new List<ClassDto>();

            foreach (var classEntity in classes)
            {
                int studentCount = 0;
                if (academicYearId.HasValue)
                {
                    var studentClasses = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(classEntity.ClassId, academicYearId.Value);
                    studentCount = studentClasses.Count();
                }
                else
                {
                    var studentClasses = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(classEntity.ClassId, 0);
                    studentCount = studentClasses.Count();
                }

                var classDto = new StudentClass.DTOs.ClassDto
                {
                    ClassId = classEntity.ClassId,
                    ClassName = classEntity.ClassName,
                    GradeLevelId = classEntity.GradeLevelId,
                    StudentCount = studentCount,
                    Status = classEntity.Status,
                    HomeroomTeachers = new List<HomeroomTeacherInfo>()
                };

                if (academicYearId.HasValue)
                {
                    var semesters = await _semesterRepository.GetByAcademicYearIdAsync(academicYearId.Value);
                    foreach (var semester in semesters.OrderBy(s => s.SemesterName))
                    {
                        var homeroomAssignment = await _context.HomeroomAssignments
                            .Where(ha => ha.ClassId == classEntity.ClassId && ha.SemesterId == semester.SemesterId)
                            .Include(ha => ha.Teacher)
                            .FirstOrDefaultAsync();

                        classDto.HomeroomTeachers.Add(new HomeroomTeacherInfo
                        {
                            SemesterId = semester.SemesterId,
                            SemesterName = semester.SemesterName,
                            TeacherName = homeroomAssignment?.Teacher?.FullName ?? "Chưa có"
                        });
                    }
                }
                else
                {
                    classDto.HomeroomTeachers.Add(new HomeroomTeacherInfo
                    {
                        SemesterId = 0,
                        SemesterName = "N/A",
                        TeacherName = "N/A"
                    });
                }

                result.Add(classDto);
            }

            return result.OrderBy(c => c.ClassName).ToList();
        }

        public async Task ProcessGraduationAsync(int academicYearId)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xử lý tốt nghiệp.");
            }

            var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
            if (academicYear == null)
            {
                throw new ArgumentException($"Năm học với Id {academicYearId} không tồn tại.");
            }

            if (academicYear.EndDate > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new InvalidOperationException("Năm học chưa kết thúc.");
            }

            var grade9Students = await _studentClassRepository.GetByGradeLevelAndAcademicYearAsync(4, academicYearId);
            if (!grade9Students.Any())
            {
                return;
            }

            foreach (var studentClass in grade9Students)
            {
                var student = await _studentRepository.GetByIdAsync(studentClass.StudentId);
                if (student != null && student.Status != "Tốt nghiệp")
                {
                    try
                    {
                        await CheckStudentPromotionEligibilityAsync(studentClass.StudentId, academicYearId);
                        student.Status = "Tốt nghiệp";
                        await _studentRepository.UpdateAsync(student);
                    }
                    catch (InvalidOperationException)
                    {
                        // Bỏ qua học sinh không đủ điều kiện tốt nghiệp
                        continue;
                    }
                }
            }
        }


        public async Task CheckStudentPromotionEligibilityAsync(int studentId, int academicYearId)
        {
            if (!await HasReadPermissionAsync()) 
            {
                throw new UnauthorizedAccessException("Bạn không có quyền kiểm tra điều kiện lên lớp của học sinh.");
            }

            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
            {
                throw new ArgumentException($"Học sinh với Id {studentId} không tồn tại.");
            }

            var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
            if (academicYear == null)
            {
                throw new ArgumentException($"Năm học với Id {academicYearId} không tồn tại.");
            }
            // Kiểm tra năm học đã kết thúc chưa
            if (academicYear.EndDate > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new InvalidOperationException($"Học sinh {student.FullName} chưa thể xét điều kiện lên lớp vì năm học {academicYear.YearName} chưa kết thúc.");
            }


            var semesters = await _semesterRepository.GetByAcademicYearIdAsync(academicYearId);
            if (!semesters.Any())
            {
                throw new InvalidOperationException($"Không tìm thấy học kỳ nào cho năm học {academicYear.YearName}.");
            }

            var studentClass = await _studentClassRepository.GetByStudentAndAcademicYearAsync(studentId, academicYearId);
            if (studentClass == null)
            {
                throw new InvalidOperationException($"Học sinh {student.FullName} không được phân công vào bất kỳ lớp nào trong năm học {academicYear.YearName}.");
            }

            var reasons = new List<string>();

            foreach (var semester in semesters)
            {
                var grades = await _gradeRepository.GetGradesByStudentAsync(studentId, semester.SemesterId);
                var finalGrades = grades.Where(g => g.AssessmentsTypeName == "Final").ToList();

                foreach (var grade in finalGrades)
                {
                    // Kiểm tra TeachingAssignment và Subject có null không sau khi include
                    if (grade.Assignment?.Subject == null)
                    {
                        reasons.Add($"Không tìm thấy thông tin môn học cho điểm tổng kết trong học kỳ {semester.SemesterName} (AssignmentId: {grade.AssignmentId}).");
                        continue;
                    }

                    var subject = grade.Assignment.Subject;
                    bool isNumericScore = float.TryParse(grade.Score, out float score);

                    if (isNumericScore)
                    {
                        if (score < 5.0f)
                        {
                            reasons.Add($"Điểm tổng kết môn {subject.SubjectName} trong học kỳ {semester.SemesterName} là {score} (< 3.5).");
                        }
                        else if (score < 5.0f)
                        {
                            reasons.Add($"Điểm tổng kết môn {subject.SubjectName} trong học kỳ {semester.SemesterName} là {score} (< 5.0).");
                        }
                    }
                    else if (grade.Score != "Đạt") // Đối với môn nhận xét
                    {
                        reasons.Add($"Điểm tổng kết môn {subject.SubjectName} trong học kỳ {semester.SemesterName} là '{grade.Score}' (không phải 'Đạt').");
                    }
                }
            }

            // Kiểm tra hạnh kiểm
            foreach (var semester in semesters)
            {
                // GetByStudentAndSemesterAsync đã được sửa để trả về Conduct đầy đủ
                var conduct = await _conductRepository.GetByStudentAndSemesterAsync(studentId, semester.SemesterId);
                if (conduct == null)
                {
                    reasons.Add($"Không tìm thấy hồ sơ hạnh kiểm cho học sinh trong học kỳ {semester.SemesterName}.");
                    continue;
                }

                if (conduct.ConductType == "Yếu") // Giả định ConductType 'Yếu' là không đạt
                {
                    reasons.Add($"Hạnh kiểm trong học kỳ {semester.SemesterName} là '{conduct.ConductType}'.");
                }
            }

            // Điều kiện lên lớp: Không có môn nào dưới 3.5, không quá X môn dưới 5.0, hạnh kiểm từ Khá trở lên
            var fail3_5 = reasons.Any(r => r.Contains("< 3.5"));
            var below5_0 = reasons.Count(r => r.Contains("< 5.0"));
            var weakConduct = reasons.Any(r => r.Contains("Hạnh kiểm") && r.Contains("'Yếu'"));

            // Giả định điều kiện lên lớp: không có điểm < 3.5 VÀ không quá 1 môn < 5.0 VÀ hạnh kiểm không phải Yếu
            bool canPromote = !fail3_5 && below5_0 <= 1 && !weakConduct; // <-- Cần điều chỉnh logic này cho chính xác

            if (!canPromote)
            {
                student.RepeatingYear = true;
                await _studentRepository.UpdateAsync(student);
                throw new InvalidOperationException($"Học sinh {student.FullName} phải ở lại lớp vì: {string.Join("; ", reasons)}");
            }
            else
            {
                // Nếu đủ điều kiện, có thể đánh dấu là không lưu ban (nếu có trường đó) hoặc đơn giản là không ném exception
                student.RepeatingYear = false; // Giả định có trường RepeatingYear để đánh dấu
                await _studentRepository.UpdateAsync(student);
            }
        }
 
       
    }
}
