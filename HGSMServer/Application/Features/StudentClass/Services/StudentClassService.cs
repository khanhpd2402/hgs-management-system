using Application.Features.AcademicYears.DTOs;
using Application.Features.Classes.DTOs;
using Application.Features.StudentClass.DTOs;
using Application.Features.StudentClass.Interfaces;
using Application.Features.Students.DTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.Services
{
    public class StudentClassService : IStudentClassService
    {
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IAcademicYearRepository _academicYearRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentClassService(
            IStudentClassRepository studentClassRepository,
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            IAcademicYearRepository academicYearRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _studentClassRepository = studentClassRepository;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _academicYearRepository = academicYearRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<bool> HasPermissionAsync()
        {
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"User Role: {userRole}"); // Logging để debug quyền truy cập
            var allowedRoles = new[] { "Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư" };
            return allowedRoles.Contains(userRole);
        }

        private async Task ValidateAcademicYearAsync(int academicYearId)
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
            if (academicYear == null)
            {
                throw new ArgumentException($"Academic Year with Id {academicYearId} does not exist.");
            }

            if (academicYear.StartDate > DateOnly.FromDateTime(DateTime.Now) || academicYear.EndDate < DateOnly.FromDateTime(DateTime.Now))
            {
                throw new InvalidOperationException($"Academic Year {academicYear.YearName} is not active.");
            }
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

            // Kiểm tra Id duy nhất
            var duplicateIds = dtos.GroupBy(d => d.Id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateIds.Any())
            {
                throw new ArgumentException($"Duplicate assignment IDs found: {string.Join(", ", duplicateIds)}.");
            }

            var updatedAssignments = new List<Domain.Models.StudentClass>();
            var assignmentsToDelete = new List<int>();

            foreach (var dto in dtos)
            {
                var existingAssignment = await _studentClassRepository.GetByIdAsync(dto.Id);
                if (existingAssignment == null)
                {
                    throw new KeyNotFoundException($"Class assignment with Id {dto.Id} does not exist.");
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

                // Kiểm tra nếu không có thay đổi
                if (existingAssignment.StudentId == dto.StudentId &&
                    existingAssignment.ClassId == dto.ClassId &&
                    existingAssignment.AcademicYearId == dto.AcademicYearId)
                {
                    continue;
                }

                var otherAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, dto.AcademicYearId);
                if (otherAssignment != null && otherAssignment.Id != dto.Id)
                {
                    assignmentsToDelete.Add(dto.Id);
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

            // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
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

        public async Task<List<StudentClassResponseDto>> SearchStudentClassesAsync(StudentClassFilterDto filter)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("You do not have permission to view class assignments.");
            }

            var assignments = await _studentClassRepository.SearchAsync(
                filter.StudentId,
                filter.ClassId,
                filter.AcademicYearId,
                filter.StudentName);

            var result = assignments
                .Where(sc => sc.Student.Status != "Tốt nghiệp") 
                .Select(sc => new StudentClassResponseDto
                {
                    Id = sc.Id,
                    StudentId = sc.StudentId,
                    StudentName = sc.Student.FullName,
                    ClassId = sc.ClassId,
                    ClassName = sc.Class.ClassName,
                    AcademicYearId = sc.AcademicYearId,
                    YearName = sc.AcademicYear.YearName
                }).ToList();

            return result;
        }

        public async Task<StudentClassFilterDataDto> GetFilterDataAsync()
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("You do not have permission to access this data.");
            }

            var students = (await _studentRepository.GetAllAsync())
                .Select(s => new StudentFilterDto { StudentId = s.StudentId, FullName = s.FullName, Status = s.Status })
                .ToList();
            var classes = (await _classRepository.GetAllAsync())
                .Select(c => new ClassDto { ClassId = c.ClassId, ClassName = c.ClassName, GradeLevelId = c.GradeLevelId })
                .OrderBy(c => c.ClassName)
                .ToList();
            var academicYears = (await _academicYearRepository.GetAllAsync())
                .Select(ay => new AcademicYearDto { AcademicYearID = ay.AcademicYearId, YearName = ay.YearName, StartDate = ay.StartDate, EndDate = ay.EndDate })
                .ToList();


            var filterData = new StudentClassFilterDataDto
            {
                Students = students,
                Classes = classes,
                AcademicYears = academicYears
            };

            return filterData;
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

            await ValidateAcademicYearAsync(dto.TargetAcademicYearId);

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
    }
}