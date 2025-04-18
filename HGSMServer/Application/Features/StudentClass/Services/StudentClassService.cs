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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HgsdbContext _context;

        public StudentClassService(
            IStudentClassRepository studentClassRepository,
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            ITeachingAssignmentRepository teachingAssignmentRepository,
            ISemesterRepository semesterRepository,
            IAcademicYearRepository academicYearRepository,
            IHttpContextAccessor httpContextAccessor,
            HgsdbContext context)
        {
            _studentClassRepository = studentClassRepository;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _teachingAssignmentRepository = teachingAssignmentRepository;
            _academicYearRepository = academicYearRepository;
            _semesterRepository = semesterRepository;
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
                throw new ArgumentException($"Academic Year with Id {academicYearId} does not exist.");
            }

            if (mustBeActive && (academicYear.StartDate > DateOnly.FromDateTime(DateTime.Now) || academicYear.EndDate < DateOnly.FromDateTime(DateTime.Now)))
            {
                throw new InvalidOperationException($"Academic Year {academicYear.YearName} is not active.");
            }
        }

        private async Task<Domain.Models.AcademicYear> GetCurrentAcademicYearAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var academicYears = await _academicYearRepository.GetAllAsync();
            var currentAcademicYear = academicYears.FirstOrDefault(ay => ay.StartDate <= today && ay.EndDate >= today);

            if (currentAcademicYear == null)
            {
                throw new InvalidOperationException("No active academic year found for the current date.");
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
                throw new UnauthorizedAccessException("You do not have permission to assign classes.");
            }

            var student = await _studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null)
            {
                throw new ArgumentException($"Student with Id {dto.StudentId} does not exist.");
            }

            if (student.Status == "Tốt nghiệp")
            {
                throw new InvalidOperationException($"Student {student.FullName} has already graduated and cannot be assigned to a class.");
            }

            var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
            if (classEntity == null)
            {
                throw new ArgumentException($"Class with Id {dto.ClassId} does not exist.");
            }

            await ValidateAcademicYearAsync(dto.AcademicYearId);

            var existingAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, dto.AcademicYearId);
            if (existingAssignment != null)
            {
                throw new InvalidOperationException($"Student {student.FullName} is already assigned to class {existingAssignment.Class.ClassName} in academic year {existingAssignment.AcademicYear.YearName}.");
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
                throw new UnauthorizedAccessException("You do not have permission to update class assignments.");
            }

            var currentAcademicYear = await GetCurrentAcademicYearAsync();

            var updatedAssignments = new List<Domain.Models.StudentClass>();
            var assignmentsToDelete = new List<int>();

            foreach (var dto in dtos)
            {
                if (dto.AcademicYearId != currentAcademicYear.AcademicYearId)
                {
                    throw new InvalidOperationException($"Can only update class assignments for the current academic year (ID: {currentAcademicYear.AcademicYearId}).");
                }

                if (await IsPreviousAcademicYearAsync(dto.AcademicYearId))
                {
                    throw new InvalidOperationException($"Cannot update class assignments for a previous academic year (ID: {dto.AcademicYearId}).");
                }

                var existingAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, dto.AcademicYearId);
                if (existingAssignment == null)
                {
                    throw new KeyNotFoundException($"Class assignment for student {dto.StudentId} in academic year {dto.AcademicYearId} does not exist.");
                }

                var student = await _studentRepository.GetByIdAsync(dto.StudentId);
                if (student == null)
                {
                    throw new ArgumentException($"Student with Id {dto.StudentId} does not exist.");
                }

                if (student.Status == "Tốt nghiệp")
                {
                    throw new InvalidOperationException($"Student {student.FullName} has already graduated and cannot be assigned to a class.");
                }

                var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
                if (classEntity == null)
                {
                    throw new ArgumentException($"Class with Id {dto.ClassId} does not exist.");
                }

                await ValidateAcademicYearAsync(dto.AcademicYearId);

                if (existingAssignment.StudentId == dto.StudentId &&
                    existingAssignment.ClassId == dto.ClassId &&
                    existingAssignment.AcademicYearId == dto.AcademicYearId)
                {
                    continue;
                }

                var otherAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, dto.AcademicYearId);
                if (otherAssignment != null && otherAssignment.Id != existingAssignment.Id)
                {
                    assignmentsToDelete.Add(existingAssignment.Id);
                    otherAssignment.ClassId = dto.ClassId;
                    otherAssignment.AcademicYearId = dto.AcademicYearId;
                    updatedAssignments.Add(otherAssignment);
                }
                else
                {
                    existingAssignment.StudentId = dto.StudentId;
                    existingAssignment.ClassId = dto.ClassId;
                    existingAssignment.AcademicYearId = dto.AcademicYearId;
                    updatedAssignments.Add(existingAssignment);
                }
            }

            using var transaction = await _studentClassRepository.BeginTransactionAsync();
            try
            {
                if (updatedAssignments.Any())
                {
                    await _studentClassRepository.UpdateRangeAsync(updatedAssignments);
                }

                if (assignmentsToDelete.Any())
                {
                    await _studentClassRepository.DeleteRangeAsync(assignmentsToDelete);
                }

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
                throw new UnauthorizedAccessException("You do not have permission to delete class assignments.");
            }

            var assignment = await _studentClassRepository.GetByIdAsync(id);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Class assignment with Id {id} does not exist.");
            }

            await _studentClassRepository.DeleteAsync(id);
        }

        public async Task ProcessGraduationAsync(int academicYearId)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("You do not have permission to process graduation.");
            }

            var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
            if (academicYear == null)
            {
                throw new ArgumentException($"Academic Year with Id {academicYearId} does not exist.");
            }

            if (academicYear.EndDate > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new InvalidOperationException("Academic year has not yet ended.");
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
                    student.Status = "Tốt nghiệp";
                    await _studentRepository.UpdateAsync(student);
                }
            }
        }

        public async Task BulkTransferClassAsync(BulkClassTransferDto dto)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("You do not have permission to transfer classes.");
            }

            var currentAcademicYear = await GetCurrentAcademicYearAsync();
            if (await IsPreviousAcademicYearAsync(dto.AcademicYearId))
            {
                throw new InvalidOperationException($"Cannot transfer classes for a previous academic year (ID: {dto.AcademicYearId}).");
            }
            if (await IsPreviousAcademicYearAsync(dto.TargetAcademicYearId))
            {
                throw new InvalidOperationException($"Cannot transfer classes to a previous academic year (ID: {dto.TargetAcademicYearId}).");
            }

            var currentClass = await _classRepository.GetByIdAsync(dto.ClassId);
            if (currentClass == null)
            {
                throw new ArgumentException($"Class with Id {dto.ClassId} does not exist.");
            }

            await ValidateAcademicYearAsync(dto.AcademicYearId);

            if (currentClass.GradeLevelId == 4)
            {
                throw new InvalidOperationException($"Students in class {currentClass.ClassName} (Grade 9) should be processed for graduation instead of transfer.");
            }

            var targetClass = await _classRepository.GetByIdAsync(dto.TargetClassId);
            if (targetClass == null)
            {
                throw new ArgumentException($"Target Class with Id {dto.TargetClassId} does not exist.");
            }

            await ValidateAcademicYearAsync(dto.TargetAcademicYearId, mustBeActive: false);

            bool isCurrentYear = dto.TargetAcademicYearId == currentAcademicYear.AcademicYearId;
            bool isNextYear = await IsNextAcademicYearAsync(dto.AcademicYearId, dto.TargetAcademicYearId);

            if (isCurrentYear)
            {
                if (targetClass.GradeLevelId != currentClass.GradeLevelId)
                {
                    throw new InvalidOperationException($"In the current academic year, students can only be transferred to a class with the same GradeLevelId ({currentClass.GradeLevelId}).");
                }
            }
            else if (isNextYear)
            {
                if (targetClass.GradeLevelId != currentClass.GradeLevelId + 1)
                {
                    throw new InvalidOperationException($"In the next academic year, students must be transferred to a class with GradeLevelId {currentClass.GradeLevelId + 1} (current GradeLevelId: {currentClass.GradeLevelId}).");
                }
            }
            else
            {
                throw new InvalidOperationException("Target academic year must be either the current year or the next academic year.");
            }

            var studentsInClass = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(dto.ClassId, dto.AcademicYearId);
            if (!studentsInClass.Any())
            {
                throw new InvalidOperationException($"No students found in class {currentClass.ClassName} for academic year {dto.AcademicYearId}.");
            }

            var newAssignments = new List<Domain.Models.StudentClass>();
            foreach (var studentClass in studentsInClass)
            {
                var student = await _studentRepository.GetByIdAsync(studentClass.StudentId);
                if (student.Status == "Tốt nghiệp")
                {
                    throw new InvalidOperationException($"Student {student.FullName} has already graduated and cannot be transferred.");
                }

                var existingAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(studentClass.StudentId, dto.TargetAcademicYearId);
                if (existingAssignment != null)
                {
                    throw new InvalidOperationException($"Student {student.FullName} is already assigned to class {existingAssignment.Class.ClassName} in academic year {dto.TargetAcademicYearId}.");
                }

                newAssignments.Add(new Domain.Models.StudentClass
                {
                    StudentId = studentClass.StudentId,
                    ClassId = dto.TargetClassId,
                    AcademicYearId = dto.TargetAcademicYearId
                });
            }

            await _studentClassRepository.AddRangeAsync(newAssignments);
        }

        // File: StudentClassService.cs
        public async Task<StudentClassFilterDataDto> GetFilterDataAsync(int? classId = null, int? semesterId = null)
        {
            if (!await HasPermissionAsync())
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

            var classDtos = new List<StudentClass.DTOs.ClassDto>();

            foreach (var classEntity in classes.Where(c => c != null))
            {
                int studentCount = 0;
                var classDto = new StudentClass.DTOs.ClassDto
                {
                    ClassId = classEntity.ClassId,
                    ClassName = classEntity.ClassName,
                    GradeLevelId = classEntity.GradeLevelId,
                    StudentCount = studentCount,
                    HomeroomTeachers = new List<HomeroomTeacherInfo>()
                };

                if (semesterId.HasValue)
                {
                    var semesterr = await _semesterRepository.GetByIdAsync(semesterId.Value);
                    if (semesterr != null)
                    {
                        var studentCountQuery = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(classEntity.ClassId, semesterr.AcademicYearId);
                        studentCount = studentCountQuery.Count();

                        // Lấy GVCN cho học kỳ được chỉ định
                        var homeroomAssignment = await _context.HomeroomAssignments
                            .Where(ha => ha.ClassId == classEntity.ClassId && ha.SemesterId == semesterId.Value && ha.Status == "Hoạt Động")
                            .Include(ha => ha.Teacher)
                            .FirstOrDefaultAsync();

                        classDto.HomeroomTeachers.Add(new HomeroomTeacherInfo
                        {
                            SemesterId = semesterr.SemesterId,
                            SemesterName = semesterr.SemesterName,
                            TeacherName = homeroomAssignment?.Teacher?.FullName ?? "Chưa có"
                        });

                        // Lấy GVCN cho học kỳ còn lại trong cùng năm học (nếu có)
                        var otherSemesters = await _semesterRepository.GetByAcademicYearIdAsync(semesterr.AcademicYearId);
                        foreach (var otherSemester in otherSemesters.Where(s => s.SemesterId != semesterId.Value))
                        {
                            var otherHomeroomAssignment = await _context.HomeroomAssignments
                                .Where(ha => ha.ClassId == classEntity.ClassId && ha.SemesterId == otherSemester.SemesterId && ha.Status == "Hoạt Động")
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
                    await ValidateAcademicYearAsync(academicYearId.Value);
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
                    HomeroomTeachers = new List<HomeroomTeacherInfo>()
                };

                if (academicYearId.HasValue)
                {
                    var semesters = await _semesterRepository.GetByAcademicYearIdAsync(academicYearId.Value);
                    foreach (var semester in semesters.OrderBy(s => s.SemesterName))
                    {
                        var homeroomAssignment = await _context.HomeroomAssignments
                            .Where(ha => ha.ClassId == classEntity.ClassId && ha.SemesterId == semester.SemesterId && ha.Status == "Hoạt Động")
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

                result.Add(classDto);
            }

            return result.OrderBy(c => c.ClassName).ToList();
        }
    }
}