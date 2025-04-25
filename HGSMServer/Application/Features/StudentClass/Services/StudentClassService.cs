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

            return targetYear.StartDate > currentYear.EndDate;
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

        public async Task CreateStudentClassAsync(StudentClassAssignmentDto dto)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền phân công lớp.");
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

            await ValidateAcademicYearAsync(dto.AcademicYearId);

            var existingAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, dto.AcademicYearId);
            if (existingAssignment != null)
            {
                throw new InvalidOperationException($"Học sinh {student.FullName} đã được phân công vào lớp {existingAssignment.Class.ClassName} trong năm học {existingAssignment.AcademicYear.YearName}.");
            }

            var assignment = new Domain.Models.StudentClass
            {
                StudentId = dto.StudentId,
                ClassId = dto.ClassId,
                AcademicYearId = dto.AcademicYearId
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

            var currentAcademicYear = await GetCurrentAcademicYearAsync();
            var updatedAssignments = new List<Domain.Models.StudentClass>();

            using var transaction = await _studentClassRepository.BeginTransactionAsync();
            try
            {
                foreach (var dto in dtos)
                {
                    if (dto.AcademicYearId != currentAcademicYear.AcademicYearId && !await IsNextAcademicYearAsync(currentAcademicYear.AcademicYearId, dto.AcademicYearId))
                    {
                        throw new InvalidOperationException($"Chỉ có thể cập nhật phân công lớp cho năm học hiện tại (ID: {currentAcademicYear.AcademicYearId}) hoặc năm học tiếp theo.");
                    }

                    if (await IsPreviousAcademicYearAsync(dto.AcademicYearId))
                    {
                        throw new InvalidOperationException($"Không thể cập nhật phân công lớp cho năm học trước (ID: {dto.AcademicYearId}).");
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

                    await ValidateAcademicYearAsync(dto.AcademicYearId, mustBeActive: dto.AcademicYearId == currentAcademicYear.AcademicYearId);

                    // Lấy GradeLevelId hiện tại của học sinh trong năm học hiện tại
                    var currentAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, currentAcademicYear.AcademicYearId);
                    var lastGradeLevel = currentAssignment?.Class?.GradeLevelId ?? 0;

                    bool isNextYear = await IsNextAcademicYearAsync(currentAcademicYear.AcademicYearId, dto.AcademicYearId);
                    if (!isNextYear)
                    {
                        // Kiểm tra trong cùng năm học, chỉ được chuyển đến lớp cùng khối (GradeLevelId)
                        if (lastGradeLevel > 0 && classEntity.GradeLevelId != lastGradeLevel)
                        {
                            var gradeLevel = await _context.GradeLevels.FindAsync(lastGradeLevel);
                            throw new InvalidOperationException($"Trong năm học hiện tại, học sinh chỉ có thể được phân công vào lớp thuộc {gradeLevel?.GradeName ?? "khối hiện tại"}.");
                        }
                    }
                    else
                    {
                        // Kiểm tra năm học hiện tại đã kết thúc
                        var currentAcademicYearData = await _academicYearRepository.GetByIdAsync(currentAcademicYear.AcademicYearId);
                        if (currentAcademicYearData.EndDate > DateOnly.FromDateTime(DateTime.Now))
                        {
                            throw new InvalidOperationException("Không thể chuyển học sinh sang năm học tiếp theo cho đến khi năm học hiện tại kết thúc.");
                        }

                        // Kiểm tra điều kiện lên lớp
                        await CheckStudentPromotionEligibilityAsync(dto.StudentId, currentAcademicYear.AcademicYearId);
                    }

                    var existingAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, dto.AcademicYearId);
                    if (existingAssignment != null)
                    {
                        // Kiểm tra nếu ClassId mới trùng với ClassId hiện tại
                        if (existingAssignment.ClassId == dto.ClassId)
                        {
                            throw new InvalidOperationException($"Học sinh {student.FullName} đã được phân công vào lớp {classEntity.ClassName} trong năm học này. Không thể chuyển sang lớp trùng với lớp hiện tại.");
                        }

                        if (!isNextYear)
                        {
                            // Xóa bản ghi cũ ngay lập tức khi chuyển lớp trong cùng năm học
                            await _studentClassRepository.DeleteAsync(existingAssignment.Id);
                        }
                    }

                    // Tạo bản ghi mới
                    var newAssignment = new Domain.Models.StudentClass
                    {
                        StudentId = dto.StudentId,
                        ClassId = dto.ClassId,
                        AcademicYearId = dto.AcademicYearId
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
                        StudentClassId = sc.Id,
                        StudentId = sc.Student.StudentId,
                        FullName = sc.Student.FullName,
                        Status = sc.Student.Status
                    })
                    .ToList();
            }
            else
            {
                students = (await _studentRepository.GetAllAsync())
                    .Select(s => new StudentFilterDto { StudentId = s.StudentId, FullName = s.FullName, Status = s.Status })
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

        public async Task<List<ClassDto>> GetClassesWithStudentCountAsync(int? academicYearId = null)
        {
            if (!await HasPermissionAsync())
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

                var classDto = new ClassDto
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

        public async Task BulkTransferClassAsync(BulkClassTransferDto dto)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền chuyển lớp hàng loạt.");
            }

            var currentAcademicYear = await GetCurrentAcademicYearAsync();
            if (await IsPreviousAcademicYearAsync(dto.AcademicYearId))
            {
                throw new InvalidOperationException($"Không thể chuyển lớp cho năm học trước (ID: {dto.AcademicYearId}).");
            }
            if (await IsPreviousAcademicYearAsync(dto.TargetAcademicYearId))
            {
                throw new InvalidOperationException($"Không thể chuyển lớp sang năm học trước (ID: {dto.TargetAcademicYearId}).");
            }

            var currentClass = await _classRepository.GetByIdAsync(dto.ClassId);
            if (currentClass == null)
            {
                throw new ArgumentException($"Lớp với Id {dto.ClassId} không tồn tại.");
            }

            await ValidateAcademicYearAsync(dto.AcademicYearId);

            if (currentClass.GradeLevelId == 4)
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

            await ValidateAcademicYearAsync(dto.TargetAcademicYearId, mustBeActive: false);

            bool isCurrentYear = dto.TargetAcademicYearId == currentAcademicYear.AcademicYearId;
            bool isNextYear = await IsNextAcademicYearAsync(dto.AcademicYearId, dto.TargetAcademicYearId);

            if (isCurrentYear)
            {
                if (targetClass.GradeLevelId != currentClass.GradeLevelId)
                {
                    var gradeLevel = await _context.GradeLevels.FindAsync(currentClass.GradeLevelId);
                    throw new InvalidOperationException($"Trong năm học hiện tại, học sinh chỉ có thể được chuyển sang lớp thuộc {gradeLevel?.GradeName ?? "khối hiện tại"}.");
                }
            }
            else if (isNextYear)
            {
                // Kiểm tra năm học hiện tại đã kết thúc
                var currentAcademicYearData = await _academicYearRepository.GetByIdAsync(dto.AcademicYearId);
                if (currentAcademicYearData.EndDate > DateOnly.FromDateTime(DateTime.Now))
                {
                    throw new InvalidOperationException("Không thể chuyển học sinh sang năm học tiếp theo cho đến khi năm học hiện tại kết thúc.");
                }
            }
            else
            {
                throw new InvalidOperationException("Năm học đích phải là năm học hiện tại hoặc năm học tiếp theo.");
            }

            var studentsInClass = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(dto.ClassId, dto.AcademicYearId);
            if (!studentsInClass.Any())
            {
                throw new InvalidOperationException($"Không tìm thấy học sinh trong lớp {currentClass.ClassName} cho năm học {dto.AcademicYearId}.");
            }

            var newAssignments = new List<Domain.Models.StudentClass>();
            var skippedStudents = new List<string>();

            using var transaction = await _studentClassRepository.BeginTransactionAsync();
            try
            {
                foreach (var studentClass in studentsInClass)
                {
                    var student = await _studentRepository.GetByIdAsync(studentClass.StudentId);
                    if (student.Status == "Tốt nghiệp")
                    {
                        skippedStudents.Add($"Học sinh {student.FullName} đã tốt nghiệp và được bỏ qua.");
                        continue;
                    }

                    var existingAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(studentClass.StudentId, dto.TargetAcademicYearId);
                    if (existingAssignment != null)
                    {
                        skippedStudents.Add($"Học sinh {student.FullName} đã được phân công vào lớp {existingAssignment.Class.ClassName} trong năm học {dto.TargetAcademicYearId} và được bỏ qua.");
                        continue;
                    }

                    if (isNextYear)
                    {
                        try
                        {
                            await CheckStudentPromotionEligibilityAsync(studentClass.StudentId, dto.AcademicYearId);
                        }
                        catch (InvalidOperationException ex)
                        {
                            skippedStudents.Add(ex.Message);
                            continue;
                        }
                    }

                    newAssignments.Add(new Domain.Models.StudentClass
                    {
                        StudentId = studentClass.StudentId,
                        ClassId = dto.TargetClassId,
                        AcademicYearId = dto.TargetAcademicYearId
                    });
                }

                if (newAssignments.Any())
                {
                    await _studentClassRepository.AddRangeAsync(newAssignments);
                }
                else
                {
                    throw new InvalidOperationException("Không có học sinh nào được chuyển lớp. Tất cả học sinh đã tốt nghiệp, được phân lớp, hoặc không đủ điều kiện. Chi tiết: " + string.Join("; ", skippedStudents));
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
        public async Task CheckStudentPromotionEligibilityAsync(int studentId, int academicYearId)
        {
            if (!await HasPermissionAsync())
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

            // Kiểm tra điểm tổng kết tất cả môn
            foreach (var semester in semesters)
            {
                var grades = await _gradeRepository.GetGradesByStudentAsync(studentId, semester.SemesterId);
                var finalGrades = grades.Where(g => g.AssessmentsTypeName == "Final").ToList();

                foreach (var grade in finalGrades)
                {
                    var teachingAssignment = await _teachingAssignmentRepository.GetByIdAsync(grade.AssignmentId.Value);
                    if (teachingAssignment == null)
                    {
                        reasons.Add($"Không tìm thấy phân công giảng dạy cho điểm tổng kết trong học kỳ {semester.SemesterName}.");
                        continue;
                    }
                    var subject = teachingAssignment.Subject;
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
                        reasons.Add($"Điểm tổng kết môn {subject.SubjectName} trong học kỳ {semester.SemesterName} là {grade.Score} (không phải 'Đạt').");
                    }
                }
            }

            // Kiểm tra hạnh kiểm
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
                    reasons.Add($"Hạnh kiểm trong học kỳ {semester.SemesterName} là 'Chưa Đạt'.");
                }
            }

            if (reasons.Any())
            {
                student.RepeatingYear = true;
                await _studentRepository.UpdateAsync(student);
                throw new InvalidOperationException($"Học sinh {student.FullName} phải ở lại lớp vì: {string.Join("; ", reasons)}");
            }
        }




    }

}