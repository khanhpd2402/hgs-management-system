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
            var allowedRoles = new[] { "Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư", "Giáo viên", "Phụ huynh", "Trưởng bộ môn" };
            return allowedRoles.Contains(userRole);
        }

        private async Task ValidateAcademicYearAsync(int academicYearId, bool mustBeActive = true)
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
            if (academicYear == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy năm học với ID {academicYearId}.");
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
                throw new KeyNotFoundException($"Không tìm thấy năm học với ID {nextAcademicYearId}.");
            }

            var academicYears = await _academicYearRepository.GetAllAsync();
            var previousAcademicYear = academicYears
                .Where(ay => ay.EndDate < nextAcademicYear.StartDate)
                .OrderByDescending(ay => ay.EndDate)
                .FirstOrDefault();

            if (previousAcademicYear == null)
            {
                throw new InvalidOperationException($"Không tìm thấy năm học trước năm học với ID {nextAcademicYearId}.");
            }

            return previousAcademicYear;
        }

        private async Task<bool> IsWithinSemester1OfAcademicYearAsync(int academicYearId)
        {
            var semestersInYear = await _semesterRepository.GetByAcademicYearIdAsync(academicYearId);
            var semester1 = semestersInYear.FirstOrDefault(s => s.SemesterName == "Học kỳ 1");

            if (semester1 == null)
            {
                throw new InvalidOperationException($"Không tìm thấy Học kỳ 1 cho năm học với ID {academicYearId}.");
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
                throw new KeyNotFoundException($"Năm học nguồn (ID: {sourceAcademicYearId}) hoặc năm học đích (ID: {targetAcademicYearId}) không tồn tại.");
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

            try
            {
                var studentClasses = await _studentClassRepository.GetAllAsync();

                var studentClassDtos = studentClasses.Select(sc => new StudentClassResponseDto
                {
                    Id = sc.Id,
                    StudentId = sc.StudentId,
                    StudentName = sc.Student?.FullName ?? "N/A",
                    ClassId = sc.ClassId,
                    ClassName = sc.Class?.ClassName ?? "N/A",
                    AcademicYearId = sc.AcademicYearId,
                    YearName = sc.AcademicYear?.YearName ?? "N/A",
                    RepeatingYear = sc.RepeatingYear
                }).ToList();

                return studentClassDtos;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách phân công lớp do lỗi hệ thống.", ex);
            }
        }

        public async Task<IEnumerable<StudentClassResponseDto>> GetAllStudentClassByLastAcademicYearAsync(int currentAcademicYearId)
        {
            if (!await HasReadPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập dữ liệu này.");
            }

            var lastAcademicYear = await GetPreviousAcademicYearByIdAsync(currentAcademicYearId);
            if (lastAcademicYear == null)
            {
                throw new InvalidOperationException($"Không tìm thấy năm học trước năm học với ID {currentAcademicYearId}.");
            }

            try
            {
                var studentClassesInLastYear = await _studentClassRepository.GetByAcademicYearIdAsync(lastAcademicYear.AcademicYearId);
                if (!studentClassesInLastYear.Any())
                {
                    throw new InvalidOperationException($"Không tìm thấy phân công lớp nào trong năm học trước (ID: {lastAcademicYear.AcademicYearId}).");
                }

                var studentClassDtos = studentClassesInLastYear.Select(sc => new StudentClassResponseDto
                {
                    Id = sc.Id,
                    StudentId = sc.StudentId,
                    StudentName = sc.Student?.FullName ?? "N/A",
                    ClassId = sc.ClassId,
                    ClassName = sc.Class?.ClassName ?? "N/A",
                    AcademicYearId = sc.AcademicYearId,
                    YearName = sc.AcademicYear?.YearName ?? "N/A",
                    RepeatingYear = sc.RepeatingYear
                }).ToList();

                return studentClassDtos;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách phân công lớp của năm học trước do lỗi hệ thống.", ex);
            }
        }

        public async Task CreateStudentClassAsync(StudentClassAssignmentDto dto)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền phân công lớp.");
            }

            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Thông tin phân công lớp không được để trống.");
            }

            int academicYearId = dto.AcademicYearId ?? (await GetCurrentAcademicYearAsync()).AcademicYearId;

            if (!await IsWithinSemester1OfAcademicYearAsync(academicYearId))
            {
                var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
                throw new InvalidOperationException($"Chỉ có thể phân công lớp học sinh trong thời gian Học kỳ 1 của năm học {academicYear?.YearName}.");
            }

            var student = await _studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy học sinh với ID {dto.StudentId}.");
            }

            if (student.Status == "Tốt nghiệp")
            {
                throw new InvalidOperationException($"Học sinh {student.FullName} đã tốt nghiệp và không thể được phân công vào lớp.");
            }

            var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
            if (classEntity == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lớp với ID {dto.ClassId}.");
            }

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
                AcademicYearId = academicYearId,
                RepeatingYear = false
            };

            try
            {
                await _studentClassRepository.AddAsync(assignment);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể phân công lớp do lỗi hệ thống.", ex);
            }
        }

        public async Task UpdateStudentClassesAsync(List<StudentClassAssignmentDto> dtos)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật phân công lớp.");
            }

            if (dtos == null || !dtos.Any())
            {
                throw new ArgumentException("Danh sách phân công lớp không được rỗng.");
            }

            var distinctStudentIds = dtos.Select(d => d.StudentId).Distinct().Count();
            if (distinctStudentIds < dtos.Count)
            {
                throw new ArgumentException("Tìm thấy ID học sinh trùng lặp trong danh sách phân công lớp.");
            }

            var updatedAssignments = new List<Domain.Models.StudentClass>();

            using var transaction = await _studentClassRepository.BeginTransactionAsync();
            try
            {
                foreach (var dto in dtos)
                {
                    int academicYearId = dto.AcademicYearId ?? (await GetCurrentAcademicYearAsync()).AcademicYearId;

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
                        throw new KeyNotFoundException($"Không tìm thấy học sinh với ID {dto.StudentId}.");
                    }

                    if (student.Status == "Tốt nghiệp")
                    {
                        throw new InvalidOperationException($"Học sinh {student.FullName} đã tốt nghiệp và không thể được phân công vào lớp.");
                    }

                    var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
                    if (classEntity == null)
                    {
                        throw new KeyNotFoundException($"Không tìm thấy lớp với ID {dto.ClassId}.");
                    }

                    if (classEntity.Status != "Hoạt động")
                    {
                        throw new InvalidOperationException($"Lớp {classEntity.ClassName} không hoạt động.");
                    }

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
                        AcademicYearId = academicYearId,
                        RepeatingYear = false
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
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Không thể cập nhật phân công lớp do lỗi hệ thống.", ex);
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

            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Thông tin chuyển lớp hàng loạt không được để trống.");
            }

            int sourceAcademicYearId = dto.AcademicYearId ?? (await GetCurrentAcademicYearAsync()).AcademicYearId;
            int targetAcademicYearId = dto.TargetAcademicYearId ?? await GetNextAcademicYearIdAsync(sourceAcademicYearId);

            await ValidateAcademicYearAsync(sourceAcademicYearId, mustBeActive: false);
            await ValidateAcademicYearAsync(targetAcademicYearId, mustBeActive: false);

            bool isNextYearTransfer = await IsNextAcademicYearAsync(sourceAcademicYearId, targetAcademicYearId);
            if (!isNextYearTransfer)
            {
                throw new InvalidOperationException($"Chuyển lớp hàng loạt chỉ được thực hiện giữa hai năm học liền kề. Năm học đích (ID: {targetAcademicYearId}) không phải là năm học tiếp theo của năm học nguồn (ID: {sourceAcademicYearId}).");
            }

            await ValidateTransferTimeWindowAsync(sourceAcademicYearId, targetAcademicYearId);

            var currentClass = await _classRepository.GetByIdAsync(dto.ClassId);
            if (currentClass == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lớp nguồn với ID {dto.ClassId}.");
            }

            if (currentClass.GradeLevelId == 4)
            {
                throw new InvalidOperationException($"Học sinh trong lớp {currentClass.ClassName} (Khối 9) cần được xử lý tốt nghiệp thay vì chuyển lớp.");
            }

            var targetClass = await _classRepository.GetByIdAsync(dto.TargetClassId);
            if (targetClass == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lớp đích với ID {dto.TargetClassId}.");
            }

            if (targetClass.Status != "Hoạt động")
            {
                throw new InvalidOperationException($"Lớp đích {targetClass.ClassName} không hoạt động.");
            }

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
                            AcademicYearId = targetAcademicYearId,
                            RepeatingYear = false
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
                            IdcardNumber = student.IdcardNumber,
                            Status = student.Status
                        });
                    }
                    catch (InvalidOperationException ex)
                    {
                        studentClass.RepeatingYear = true;
                        await _studentClassRepository.UpdateAsync(studentClass);
                        skippedStudents.Add(new SkippedStudentDto { StudentId = student.StudentId, FullName = student.FullName, Reason = ex.Message });
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
                throw new InvalidOperationException("Không thể chuyển lớp hàng loạt do lỗi hệ thống.", ex);
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
                throw new KeyNotFoundException($"Không tìm thấy phân công lớp với ID {id}.");
            }

            try
            {
                await _studentClassRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể xóa phân công lớp do lỗi hệ thống.", ex);
            }
        }

        public async Task<StudentClassFilterDataDto> GetFilterDataAsync(int? classId = null, int? semesterId = null)
        {
            if (!await HasReadPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập dữ liệu này.");
            }

            try
            {
                List<StudentFilterDto> students;

                if (classId.HasValue && semesterId.HasValue)
                {
                    var semester = await _semesterRepository.GetByIdAsync(semesterId.Value);
                    if (semester == null)
                    {
                        throw new KeyNotFoundException($"Không tìm thấy học kỳ với ID {semesterId.Value}.");
                    }

                    var studentClasses = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(classId.Value, semester.AcademicYearId);
                    students = studentClasses
                        .Select(sc => new StudentFilterDto
                        {
                            StudentClassId = sc.Id,
                            StudentId = sc.Student.StudentId,
                            FullName = sc.Student.FullName,
                            Status = sc.Student.Status
                        })
                        .ToList();
                }
                else
                {
                    var currentAcademicYear = await GetCurrentAcademicYearAsync();
                    int currentAcademicYearId = currentAcademicYear.AcademicYearId;

                    var studentClasses = await _studentClassRepository.GetByAcademicYearIdAsync(currentAcademicYearId);
                    students = studentClasses
                        .Select(sc => new StudentFilterDto
                        {
                            StudentClassId = sc.Id,
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
                    throw new KeyNotFoundException($"Không tìm thấy lớp với ID {classId.Value}.");
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
                        var semester = await _semesterRepository.GetByIdAsync(semesterId.Value);
                        if (semester != null)
                        {
                            var studentCountQuery = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(classEntity.ClassId, semester.AcademicYearId);
                            studentCount = studentCountQuery.Count();

                            var homeroomAssignment = await _context.HomeroomAssignments
                                .Where(ha => ha.ClassId == classEntity.ClassId && ha.SemesterId == semesterId.Value)
                                .Include(ha => ha.Teacher)
                                .FirstOrDefaultAsync();

                            classDto.HomeroomTeachers.Add(new HomeroomTeacherInfo
                            {
                                SemesterId = semester.SemesterId,
                                SemesterName = semester.SemesterName,
                                TeacherName = homeroomAssignment?.Teacher?.FullName ?? "Chưa có"
                            });

                            var otherSemesters = await _semesterRepository.GetByAcademicYearIdAsync(semester.AcademicYearId);
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

                var semesterDto = new SemesterDto();
                if (semesterId.HasValue)
                {
                    var semester = await _semesterRepository.GetByIdAsync(semesterId.Value);
                    if (semester == null)
                    {
                        throw new KeyNotFoundException($"Không tìm thấy học kỳ với ID {semesterId.Value}.");
                    }

                    semesterDto = new SemesterDto
                    {
                        SemesterID = semester.SemesterId,
                        SemesterName = semester.SemesterName,
                        AcademicYearID = semester.AcademicYearId,
                        StartDate = semester.StartDate,
                        EndDate = semester.EndDate
                    };
                }
                else
                {
                    var currentAcademicYear = await GetCurrentAcademicYearAsync();
                    var semesters = await _semesterRepository.GetByAcademicYearIdAsync(currentAcademicYear.AcademicYearId);
                    var semester = semesters.FirstOrDefault(s => s.StartDate <= DateOnly.FromDateTime(DateTime.Now) && s.EndDate >= DateOnly.FromDateTime(DateTime.Now));
                    if (semester == null)
                    {
                        throw new InvalidOperationException("Không tìm thấy học kỳ đang hoạt động.");
                    }

                    semesterDto = new SemesterDto
                    {
                        SemesterID = semester.SemesterId,
                        SemesterName = semester.SemesterName,
                        AcademicYearID = semester.AcademicYearId,
                        StartDate = semester.StartDate,
                        EndDate = semester.EndDate
                    };
                }

                var filterData = new StudentClassFilterDataDto
                {
                    Students = students,
                    Classes = classDtos.OrderBy(c => c.ClassName).ToList(),
                    Semesters = new List<SemesterDto> { semesterDto }
                };

                return filterData;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy dữ liệu lọc do lỗi hệ thống.", ex);
            }
        }

        public async Task<List<StudentClass.DTOs.ClassDto>> GetClassesWithStudentCountAsync(int? academicYearId = null)
        {
            if (!await HasReadPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập thông tin lớp.");
            }

            try
            {
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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy thông tin lớp do lỗi hệ thống.", ex);
            }
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
                throw new KeyNotFoundException($"Không tìm thấy năm học với ID {academicYearId}.");
            }

            if (academicYear.EndDate > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new InvalidOperationException("Năm học chưa kết thúc.");
            }

            var grade9Students = await _studentClassRepository.GetByGradeLevelAndAcademicYearAsync(4, academicYearId);
            if (!grade9Students.Any())
            {
                throw new InvalidOperationException($"Không tìm thấy học sinh lớp 9 trong năm học {academicYear.YearName} để xử lý tốt nghiệp.");
            }

            try
            {
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
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể xử lý tốt nghiệp do lỗi hệ thống.", ex);
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
                throw new KeyNotFoundException($"Không tìm thấy học sinh với ID {studentId}.");
            }

            var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
            if (academicYear == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy năm học với ID {academicYearId}.");
            }

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
                    if (grade.Assignment?.Subject == null)
                    {
                        reasons.Add($"Không tìm thấy thông tin môn học cho điểm tổng kết trong học kỳ {semester.SemesterName} (AssignmentId: {grade.AssignmentId}).");
                        continue;
                    }

                    var subject = grade.Assignment.Subject;
                    bool isNumericScore = float.TryParse(grade.Score, out float score);

                    if (isNumericScore)
                    {
                        if (score < 3.5f)
                        {
                            reasons.Add($"Điểm tổng kết môn {subject.SubjectName} trong học kỳ {semester.SemesterName} là {score} (< 3.5).");
                        }
                        else if (score < 5.0f)
                        {
                            reasons.Add($"Điểm tổng kết môn {subject.SubjectName} trong học kỳ {semester.SemesterName} là {score} (< 5.0).");
                        }
                    }
                    else if (grade.Score != "Đạt")
                    {
                        reasons.Add($"Điểm tổng kết môn {subject.SubjectName} trong học kỳ {semester.SemesterName} là '{grade.Score}' (không phải 'Đạt').");
                    }
                }
            }

            foreach (var semester in semesters)
            {
                var conduct = await _conductRepository.GetByStudentAndSemesterAsync(studentId, semester.SemesterId);
                if (conduct == null)
                {
                    reasons.Add($"Không tìm thấy hồ sơ hạnh kiểm cho học sinh trong học kỳ {semester.SemesterName}.");
                    continue;
                }

                if (conduct.ConductType == "Yếu")
                {
                    reasons.Add($"Hạnh kiểm trong học kỳ {semester.SemesterName} là '{conduct.ConductType}'.");
                }
            }

            var fail3_5 = reasons.Any(r => r.Contains("< 3.5"));
            var below5_0 = reasons.Count(r => r.Contains("< 5.0"));
            var weakConduct = reasons.Any(r => r.Contains("Hạnh kiểm") && r.Contains("'Yếu'"));

            bool canPromote = !fail3_5 && below5_0 <= 1 && !weakConduct;

            if (!canPromote)
            {
                studentClass.RepeatingYear = true;
                await _studentClassRepository.UpdateAsync(studentClass);
                throw new InvalidOperationException($"Học sinh {student.FullName} phải ở lại lớp vì: {string.Join("; ", reasons)}");
            }
            else
            {
                studentClass.RepeatingYear = false;
                await _studentClassRepository.UpdateAsync(studentClass);
            }
        }

        public async Task<IEnumerable<StudentClassResponseDto>> GetRepeatStudentsByAcademicYearAsync(int academicYearId)
        {
            if (!await HasReadPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập dữ liệu này.");
            }

            await ValidateAcademicYearAsync(academicYearId, mustBeActive: false);

            try
            {
                var repeatStudents = await _studentClassRepository.GetByAcademicYearIdAsync(academicYearId);
                repeatStudents = repeatStudents.Where(sc => sc.RepeatingYear == true).ToList();

                if (!repeatStudents.Any())
                {
                    throw new InvalidOperationException($"Không tìm thấy học sinh lưu ban trong năm học với ID {academicYearId}.");
                }

                var studentClassDtos = repeatStudents.Select(sc => new StudentClassResponseDto
                {
                    Id = sc.Id,
                    StudentId = sc.StudentId,
                    StudentName = sc.Student?.FullName ?? "N/A",
                    ClassId = sc.ClassId,
                    ClassName = sc.Class?.ClassName ?? "N/A",
                    AcademicYearId = sc.AcademicYearId,
                    YearName = sc.AcademicYear?.YearName ?? "N/A",
                    RepeatingYear = sc.RepeatingYear ?? false
                }).ToList();

                return studentClassDtos;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách học sinh lưu ban do lỗi hệ thống.", ex);
            }
        }
    }
}