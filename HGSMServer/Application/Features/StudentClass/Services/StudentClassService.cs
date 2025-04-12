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
using System.Text;
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
            var allowedRoles = new[] { "Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư" };
            return allowedRoles.Contains(userRole);
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

            var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
            if (classEntity == null)
            {
                throw new ArgumentException($"Class with Id {dto.ClassId} does not exist.");
            }

            var academicYear = await _academicYearRepository.GetByIdAsync(dto.AcademicYearId);
            if (academicYear == null)
            {
                throw new ArgumentException($"Academic Year with Id {dto.AcademicYearId} does not exist.");
            }

            var existingAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, dto.AcademicYearId);
            if (existingAssignment != null)
            {
                throw new InvalidOperationException($"Student {student.FullName} is already assigned to class {existingAssignment.Class.ClassName} in academic year {academicYear.YearName}.");
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

            var updatedAssignments = new List<Domain.Models.StudentClass>();
            var assignmentsToDelete = new List<int>();

            foreach (var dto in dtos)
            {
                // Kiểm tra phân lớp hiện tại có tồn tại không
                var existingAssignment = await _studentClassRepository.GetByIdAsync(dto.StudentId);
                if (existingAssignment == null)
                {
                    throw new KeyNotFoundException($"Class assignment with Id {dto.StudentId} does not exist.");
                }

                // Kiểm tra học sinh
                var student = await _studentRepository.GetByIdAsync(dto.StudentId);
                if (student == null)
                {
                    throw new ArgumentException($"Student with Id {dto.StudentId} does not exist.");
                }

                // Kiểm tra lớp
                var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
                if (classEntity == null)
                {
                    throw new ArgumentException($"Class with Id {dto.ClassId} does not exist.");
                }

                // Kiểm tra năm học
                var academicYear = await _academicYearRepository.GetByIdAsync(dto.AcademicYearId);
                if (academicYear == null)
                {
                    throw new ArgumentException($"Academic Year with Id {dto.AcademicYearId} does not exist.");
                }

                // Kiểm tra nếu có phân lớp khác cho cùng học sinh và năm học
                var otherAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, dto.AcademicYearId);
                if (otherAssignment != null && otherAssignment.Id != dto.StudentId)
                {
                    // Nếu đã có phân lớp khác, đánh dấu để xóa bản ghi hiện tại
                    assignmentsToDelete.Add(dto.StudentId);

                    // Cập nhật phân lớp khác nếu cần
                    otherAssignment.ClassId = dto.ClassId;
                    otherAssignment.AcademicYearId = dto.AcademicYearId;
                    updatedAssignments.Add(otherAssignment);
                }
                else
                {
                    // Cập nhật phân lớp hiện tại
                    existingAssignment.StudentId = dto.StudentId;
                    existingAssignment.ClassId = dto.ClassId;
                    existingAssignment.AcademicYearId = dto.AcademicYearId;
                    updatedAssignments.Add(existingAssignment);
                }
            }

            // Cập nhật tất cả phân lớp
            await _studentClassRepository.UpdateRangeAsync(updatedAssignments);

            // Xóa các phân lớp trùng lặp nếu có
            if (assignmentsToDelete.Any())
            {
                await _studentClassRepository.DeleteRangeAsync(assignmentsToDelete);
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

            var result = assignments.Select(sc => new StudentClassResponseDto
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

            var filterData = new StudentClassFilterDataDto
            {
                Students = (await _studentRepository.GetAllAsync())
                    .Select(s => new StudentFilterDto { StudentId = s.StudentId, FullName = s.FullName })
                    .ToList(),
                Classes = (await _classRepository.GetAllAsync())
                    .Select(c => new ClassDto { ClassId = c.ClassId, ClassName = c.ClassName, GradeLevelId = c.GradeLevelId })
                    .OrderBy(c => c.ClassName)
                    .ToList(),
                AcademicYears = (await _academicYearRepository.GetAllAsync())
                    .Select(ay => new AcademicYearDto { AcademicYearID = ay.AcademicYearId, YearName = ay.YearName, StartDate = ay.StartDate, EndDate = ay.EndDate })
                    .ToList()
            };

            return filterData;
        }
        public async Task BulkTransferClassAsync(BulkClassTransferDto dto)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("You do not have permission to transfer classes.");
            }

            // Kiểm tra lớp hiện tại và năm học hiện tại
            var currentClass = await _classRepository.GetByIdAsync(dto.ClassId);
            if (currentClass == null)
            {
                throw new ArgumentException($"Class with Id {dto.ClassId} does not exist.");
            }

            var currentAcademicYear = await _academicYearRepository.GetByIdAsync(dto.AcademicYearId);
            if (currentAcademicYear == null)
            {
                throw new ArgumentException($"Academic Year with Id {dto.AcademicYearId} does not exist.");
            }

            // Kiểm tra lớp đích và năm học đích
            var targetClass = await _classRepository.GetByIdAsync(dto.TargetClassId);
            if (targetClass == null)
            {
                throw new ArgumentException($"Target Class with Id {dto.TargetClassId} does not exist.");
            }

            var targetAcademicYear = await _academicYearRepository.GetByIdAsync(dto.TargetAcademicYearId);
            if (targetAcademicYear == null)
            {
                throw new ArgumentException($"Target Academic Year with Id {dto.TargetAcademicYearId} does not exist.");
            }

            // Lấy danh sách học sinh trong lớp hiện tại
            var studentsInClass = await _studentClassRepository.GetByClassIdAndAcademicYearAsync(dto.ClassId, dto.AcademicYearId);
            if (!studentsInClass.Any())
            {
                throw new InvalidOperationException($"No students found in class {currentClass.ClassName} for academic year {currentAcademicYear.YearName}.");
            }

            // Kiểm tra xem có học sinh nào đã được phân lớp trong năm học đích chưa
            var newAssignments = new List<Domain.Models.StudentClass>();
            foreach (var studentClass in studentsInClass)
            {
                var existingAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(studentClass.StudentId, dto.TargetAcademicYearId);
                if (existingAssignment != null)
                {
                    throw new InvalidOperationException($"Student {studentClass.Student.FullName} is already assigned to class {existingAssignment.Class.ClassName} in academic year {targetAcademicYear.YearName}.");
                }

                newAssignments.Add(new Domain.Models.StudentClass
                {
                    StudentId = studentClass.StudentId,
                    ClassId = dto.TargetClassId,
                    AcademicYearId = dto.TargetAcademicYearId
                });
            }

            // Thêm các bản ghi phân lớp mới
            await _studentClassRepository.AddRangeAsync(newAssignments);
        }
    }
}
