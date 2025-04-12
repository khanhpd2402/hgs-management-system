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

        public async Task UpdateStudentClassAsync(int id, StudentClassAssignmentDto dto)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("You do not have permission to update class assignments.");
            }

            var existingAssignment = await _studentClassRepository.GetByIdAsync(id);
            if (existingAssignment == null)
            {
                throw new KeyNotFoundException($"Class assignment with Id {id} does not exist.");
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

            // Kiểm tra nếu có phân lớp khác cho cùng học sinh và năm học
            var otherAssignment = await _studentClassRepository.GetByStudentAndAcademicYearAsync(dto.StudentId, dto.AcademicYearId);
            if (otherAssignment != null && otherAssignment.Id != id)
            {
                throw new InvalidOperationException($"Student {student.FullName} is already assigned to another class in academic year {academicYear.YearName}.");
            }

            existingAssignment.StudentId = dto.StudentId;
            existingAssignment.ClassId = dto.ClassId;
            existingAssignment.AcademicYearId = dto.AcademicYearId;

            await _studentClassRepository.UpdateAsync(existingAssignment);
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
                    .Select(s => new StudentDto { StudentId = s.StudentId, FullName = s.FullName })
                    .ToList(),
                Classes = (await _classRepository.GetAllAsync())
                    .Select(c => new ClassDto { ClassId = c.ClassId, ClassName = c.ClassName })
                    .OrderBy(c => c.ClassName)
                    .ToList(),
                AcademicYears = (await _academicYearRepository.GetAllAsync())
                    .Select(ay => new AcademicYearDto { AcademicYearID = ay.AcademicYearId, YearName = ay.YearName })
                    .ToList()
            };

            return filterData;
        }
    }
}
