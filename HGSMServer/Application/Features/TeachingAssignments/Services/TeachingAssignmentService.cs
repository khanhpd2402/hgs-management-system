using Application.Features.Classes.DTOs;
using Application.Features.Subjects.DTOs;
using Application.Features.Teachers.DTOs;
using Application.Features.TeachingAssignments.DTOs;
using Application.Features.TeachingAssignments.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TeachingAssignments.Services
{
    public class TeachingAssignmentService : ITeachingAssignmentService
    {
        private readonly HgsdbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TeachingAssignmentService(HgsdbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<bool> HasPermissionAsync()
        {
            var userRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var allowedRoles = new[] { "Principal", "VicePrincipal", "HeadOfDepartment", "AdministrativeOfficer" };
            return allowedRoles.Contains(userRole);
        }

        public async Task CreateTeachingAssignmentAsync(TeachingAssignmentCreateDto dto)
        {
            // Kiểm tra quyền truy cập
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("You do not have permission to assign teaching duties.");
            }

            // Kiểm tra sự tồn tại của giáo viên, môn học, phân môn, học kỳ
            var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
            if (teacher == null)
            {
                throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.");
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectId == dto.SubjectId && s.SubjectCategory == dto.SubjectCategory);
            if (subject == null)
            {
                throw new ArgumentException($"Subject with Id {dto.SubjectId} and category {dto.SubjectCategory} does not exist.");
            }

            var semester = await _context.Semesters.FindAsync(dto.SemesterId);
            if (semester == null)
            {
                throw new ArgumentException($"Semester with Id {dto.SemesterId} does not exist.");
            }

            // Kiểm tra xem giáo viên có được gán môn học này không
            var teacherSubject = await _context.TeacherSubjects
                .FirstOrDefaultAsync(ts => ts.TeacherId == dto.TeacherId && ts.SubjectId == dto.SubjectId);
            if (teacherSubject == null)
            {
                throw new ArgumentException($"Teacher {teacher.FullName} is not assigned to subject {subject.SubjectName}.");
            }

            // Tạo bản ghi trong TeachingAssignments cho từng lớp
            foreach (var classAssignment in dto.ClassAssignments)
            {
                var classEntity = await _context.Classes.FindAsync(classAssignment.ClassId);
                if (classEntity == null)
                {
                    throw new ArgumentException($"Class with Id {classAssignment.ClassId} does not exist.");
                }

                // Kiểm tra xem phân công đã tồn tại chưa
                var existingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta =>
                        ta.TeacherId == dto.TeacherId &&
                        ta.SubjectId == dto.SubjectId &&
                        ta.ClassId == classAssignment.ClassId &&
                        ta.SemesterId == dto.SemesterId);

                if (existingAssignment == null)
                {
                    // Tạo mới nếu chưa tồn tại
                    var assignment = new Domain.Models.TeachingAssignment
                    {
                        TeacherId = dto.TeacherId,
                        SubjectId = dto.SubjectId,
                        ClassId = classAssignment.ClassId,
                        SemesterId = dto.SemesterId
                    };
                    _context.TeachingAssignments.Add(assignment);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<TeachingAssignmentFilterDataDto> GetFilterDataAsync()
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("You do not have permission to access this data.");
            }

            var filterData = new TeachingAssignmentFilterDataDto
            {
                Teachers = await _context.Teachers
                    .Select(t => new TeacherListDto
                    {
                        TeacherId = t.TeacherId,
                        FullName = t.FullName
                    })
                    .ToListAsync(),

                Subjects = await _context.Subjects
                    .Select(s => new SubjectDto
                    {
                        SubjectId = s.SubjectId,
                        SubjectName = s.SubjectName,
                        SubjectCategory = s.SubjectCategory
                    })
                    .ToListAsync(),

                Classes = await _context.Classes
                    .Select(c => new ClassDto
                    {
                        ClassId = c.ClassId,
                        ClassName = c.ClassName
                    })
                    .OrderBy(c => c.ClassName)
                    .ToListAsync()
            };

            return filterData;
        }

        public async Task<List<TeachingAssignmentResponseDto>> GetAssignmentsForCreationAsync(TeachingAssignmentCreateDto dto)
        {
            if (!await HasPermissionAsync())
            {
                throw new UnauthorizedAccessException("You do not have permission to access this data.");
            }

            var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
            if (teacher == null)
            {
                throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.");
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectId == dto.SubjectId && s.SubjectCategory == dto.SubjectCategory);
            if (subject == null)
            {
                throw new ArgumentException($"Subject with Id {dto.SubjectId} and category {dto.SubjectCategory} does not exist.");
            }

            var semester = await _context.Semesters.FindAsync(dto.SemesterId);
            if (semester == null)
            {
                throw new ArgumentException($"Semester with Id {dto.SemesterId} does not exist.");
            }

            var result = new List<TeachingAssignmentResponseDto>();

            foreach (var classAssignment in dto.ClassAssignments)
            {
                var classEntity = await _context.Classes.FindAsync(classAssignment.ClassId);
                if (classEntity == null)
                {
                    throw new ArgumentException($"Class with Id {classAssignment.ClassId} does not exist.");
                }

                // Tính số tiết/tuần thực tế từ Timetables
                int actualPeriodsPerWeekHK1 = await _context.Timetables
                    .Where(t => t.TeacherId == dto.TeacherId &&
                                t.SubjectId == dto.SubjectId &&
                                t.ClassId == classAssignment.ClassId &&
                                t.SemesterId == dto.SemesterId &&
                                t.Semester.SemesterName == "Học kỳ 1")
                    .GroupBy(t => t.DayOfWeek)
                    .CountAsync();

                int actualPeriodsPerWeekHK2 = await _context.Timetables
                    .Where(t => t.TeacherId == dto.TeacherId &&
                                t.SubjectId == dto.SubjectId &&
                                t.ClassId == classAssignment.ClassId &&
                                t.SemesterId == dto.SemesterId &&
                                t.Semester.SemesterName == "Học kỳ 2")
                    .GroupBy(t => t.DayOfWeek)
                    .CountAsync();

                result.Add(new TeachingAssignmentResponseDto
                {
                    AssignmentId = 0, // Chưa có AssignmentId vì chưa lưu
                    TeacherId = dto.TeacherId,
                    TeacherName = teacher.FullName,
                    SubjectId = dto.SubjectId,
                    SubjectName = subject.SubjectName,
                    ClassId = classAssignment.ClassId,
                    ClassName = classEntity.ClassName,
                    SemesterId = dto.SemesterId,
                    SemesterName = semester.SemesterName,
                    ActualPeriodsPerWeekHK1 = actualPeriodsPerWeekHK1,
                    ActualPeriodsPerWeekHK2 = actualPeriodsPerWeekHK2
                });
            }

            return result;
        }

        public async Task<List<TeachingAssignmentResponseDto>> SearchTeachingAssignmentsAsync(TeachingAssignmentFilterDto filter)
        {
            var userRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = userIdClaim != null ? int.Parse(userIdClaim) : (int?)null;

            var query = _context.TeachingAssignments
                .Include(ta => ta.Teacher)
                .Include(ta => ta.Subject)
                .Include(ta => ta.Class)
                .Include(ta => ta.Semester)
                .AsQueryable();

            // Nếu là giáo viên, chỉ được xem phân công của bản thân
            if (userRole == "Teacher")
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
                if (teacher != null)
                {
                    query = query.Where(ta => ta.TeacherId == teacher.TeacherId);
                }
                else
                {
                    throw new UnauthorizedAccessException("Teacher not found.");
                }
            }

            // Lọc theo các tiêu chí
            if (filter.TeacherId.HasValue)
            {
                query = query.Where(ta => ta.TeacherId == filter.TeacherId.Value);
            }

            if (!string.IsNullOrEmpty(filter.TeacherName))
            {
                query = query.Where(ta => ta.Teacher.FullName.Contains(filter.TeacherName));
            }

            if (filter.SubjectId.HasValue)
            {
                query = query.Where(ta => ta.SubjectId == filter.SubjectId.Value);
            }

            if (!string.IsNullOrEmpty(filter.SubjectName))
            {
                query = query.Where(ta => ta.Subject.SubjectName.Contains(filter.SubjectName));
            }

            if (filter.ClassId.HasValue)
            {
                query = query.Where(ta => ta.ClassId == filter.ClassId.Value);
            }

            if (!string.IsNullOrEmpty(filter.ClassName))
            {
                query = query.Where(ta => ta.Class.ClassName.Contains(filter.ClassName));
            }

            if (filter.SemesterId.HasValue)
            {
                query = query.Where(ta => ta.SemesterId == filter.SemesterId.Value);
            }

            // Tính số tiết/tuần thực tế từ Timetables
            var result = await query
                .Select(ta => new TeachingAssignmentResponseDto
                {
                    AssignmentId = ta.AssignmentId,
                    TeacherId = ta.TeacherId,
                    TeacherName = ta.Teacher.FullName,
                    SubjectId = ta.SubjectId,
                    SubjectName = ta.Subject.SubjectName,
                    ClassId = ta.ClassId,
                    ClassName = ta.Class.ClassName,
                    SemesterId = ta.SemesterId,
                    SemesterName = ta.Semester.SemesterName,
                    ActualPeriodsPerWeekHK1 = _context.Timetables
                        .Where(t => t.TeacherId == ta.TeacherId &&
                                    t.SubjectId == ta.SubjectId &&
                                    t.ClassId == ta.ClassId &&
                                    t.SemesterId == ta.SemesterId &&
                                    t.Semester.SemesterName == "Học kỳ 1")
                        .GroupBy(t => t.DayOfWeek)
                        .Count(),
                    ActualPeriodsPerWeekHK2 = _context.Timetables
                        .Where(t => t.TeacherId == ta.TeacherId &&
                                    t.SubjectId == ta.SubjectId &&
                                    t.ClassId == ta.ClassId &&
                                    t.SemesterId == ta.SemesterId &&
                                    t.Semester.SemesterName == "Học kỳ 2")
                        .GroupBy(t => t.DayOfWeek)
                        .Count()
                })
                .ToListAsync();

            return result;
        }
    }
}
