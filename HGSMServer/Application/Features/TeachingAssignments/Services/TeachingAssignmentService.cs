using Application.Features.Classes.DTOs;
using Application.Features.Subjects.DTOs;
using Application.Features.Teachers.DTOs;
using Application.Features.TeachingAssignments.DTOs;
using Application.Features.TeachingAssignments.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        var allowedRoles = new[] { "Principal", "VicePrincipal", "HeadOfDepartment", "AdministrativeOfficer" };
        return allowedRoles.Contains(userRole);
    }

    private async Task<bool> HasHomeroomPermissionAsync()
    {
        var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        var allowedRoles = new[] { "Hiệu trưởng", "Cán bộ văn thư", "Hiệu phó" }; // Chỉ Hiệu trưởng và Cán bộ văn thư
        return allowedRoles.Contains(userRole);
    }

    public async Task CreateTeachingAssignmentAsync(TeachingAssignmentCreateDto dto)
    {
        if (!await HasPermissionAsync())
        {
            throw new UnauthorizedAccessException("You do not have permission to assign teaching duties.");
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

        var teacherSubject = await _context.TeacherSubjects
            .FirstOrDefaultAsync(ts => ts.TeacherId == dto.TeacherId && ts.SubjectId == dto.SubjectId);
        if (teacherSubject == null)
        {
            throw new ArgumentException($"Teacher {teacher.FullName} is not assigned to subject {subject.SubjectName}.");
        }

        foreach (var classAssignment in dto.ClassAssignments)
        {
            var classEntity = await _context.Classes.FindAsync(classAssignment.ClassId);
            if (classEntity == null)
            {
                throw new ArgumentException($"Class with Id {classAssignment.ClassId} does not exist.");
            }

            var existingAssignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(ta =>
                    ta.TeacherId == dto.TeacherId &&
                    ta.SubjectId == dto.SubjectId &&
                    ta.ClassId == classAssignment.ClassId &&
                    ta.SemesterId == dto.SemesterId);

            if (existingAssignment == null)
            {
                var assignment = new Domain.Models.TeachingAssignment
                {
                    TeacherId = dto.TeacherId,
                    SubjectId = dto.SubjectId,
                    ClassId = classAssignment.ClassId,
                    SemesterId = dto.SemesterId,
                    IsHomeroomTeacher = false // Mặc định là false
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
                .Select(t => new TeacherListDto { TeacherId = t.TeacherId, FullName = t.FullName })
                .ToListAsync(),
            Subjects = await _context.Subjects
                .Select(s => new SubjectDto { SubjectID = s.SubjectId, SubjectName = s.SubjectName })
                .ToListAsync(),
            Classes = await _context.Classes
                .Select(c => new ClassDto { ClassId = c.ClassId, ClassName = c.ClassName })
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

            result.Add(new TeachingAssignmentResponseDto
            {
                AssignmentId = 0,
                TeacherId = dto.TeacherId,
                TeacherName = teacher.FullName,
                SubjectId = dto.SubjectId,
                SubjectName = subject.SubjectName,
                ClassId = classAssignment.ClassId,
                ClassName = classEntity.ClassName,
                SemesterId = dto.SemesterId,
                SemesterName = semester.SemesterName,
                IsHomeroomTeacher = false // Thêm trường mới
            });
        }

        return result;
    }

    public async Task<List<TeachingAssignmentResponseDto>> SearchTeachingAssignmentsAsync(TeachingAssignmentFilterDto filter)
    {
        var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

        var query = _context.TeachingAssignments
            .Include(ta => ta.Teacher)
            .Include(ta => ta.Subject)
            .Include(ta => ta.Class)
            .Include(ta => ta.Semester)
            .AsQueryable();

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

        if (filter.TeacherId.HasValue)
            query = query.Where(ta => ta.TeacherId == filter.TeacherId.Value);
        if (!string.IsNullOrEmpty(filter.TeacherName))
            query = query.Where(ta => ta.Teacher.FullName.Contains(filter.TeacherName));
        if (filter.SubjectId.HasValue)
            query = query.Where(ta => ta.SubjectId == filter.SubjectId.Value);
        if (!string.IsNullOrEmpty(filter.SubjectName))
            query = query.Where(ta => ta.Subject.SubjectName.Contains(filter.SubjectName));
        if (filter.ClassId.HasValue)
            query = query.Where(ta => ta.ClassId == filter.ClassId.Value);
        if (!string.IsNullOrEmpty(filter.ClassName))
            query = query.Where(ta => ta.Class.ClassName.Contains(filter.ClassName));
        if (filter.SemesterId.HasValue)
            query = query.Where(ta => ta.SemesterId == filter.SemesterId.Value);

        var assignmentList = await query.ToListAsync();
        var result = new List<TeachingAssignmentResponseDto>();

        foreach (var ta in assignmentList)
        {
            result.Add(new TeachingAssignmentResponseDto
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
                IsHomeroomTeacher = ta.IsHomeroomTeacher // Thêm trường mới
            });
        }

        return result;
    }

    public async Task AssignHomeroomAsync(AssignHomeroomDto dto)
    {
        if (!await HasHomeroomPermissionAsync())
        {
            throw new UnauthorizedAccessException("You do not have permission to assign homeroom duties.");
        }

        if (dto.TeacherId <= 0 || dto.ClassId <= 0 || dto.SemesterId <= 0)
        {
            throw new ArgumentException("TeacherId, ClassId, and SemesterId must be positive.");
        }

        var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
        if (teacher == null)
        {
            throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.");
        }

        var classEntity = await _context.Classes.FindAsync(dto.ClassId);
        if (classEntity == null)
        {
            throw new ArgumentException($"Class with Id {dto.ClassId} does not exist.");
        }

        var semester = await _context.Semesters.FindAsync(dto.SemesterId);
        if (semester == null)
        {
            throw new ArgumentException($"Semester with Id {dto.SemesterId} does not exist.");
        }

        var hasHomeroomTeacher = await _context.TeachingAssignments
            .AnyAsync(ta => ta.ClassId == dto.ClassId && ta.SemesterId == dto.SemesterId && ta.IsHomeroomTeacher == true);
        if (hasHomeroomTeacher)
        {
            throw new InvalidOperationException($"Class with Id {dto.ClassId} already has a homeroom teacher in semester {dto.SemesterId}.");
        }

        var existingAssignment = await _context.TeachingAssignments
            .FirstOrDefaultAsync(ta => ta.TeacherId == dto.TeacherId && ta.ClassId == dto.ClassId && ta.SemesterId == dto.SemesterId);

        if (existingAssignment != null)
        {
            existingAssignment.IsHomeroomTeacher = true;
            _context.TeachingAssignments.Update(existingAssignment);
        }
        else
        {
            var defaultSubject = await _context.TeacherSubjects
                .Where(ts => ts.TeacherId == dto.TeacherId)
                .Select(ts => ts.SubjectId)
                .FirstOrDefaultAsync();

            if (defaultSubject == 0 || defaultSubject == null) // Kiểm tra null hoặc 0
            {
                throw new InvalidOperationException($"Teacher with Id {dto.TeacherId} has no assigned subjects.");
            }

            var newAssignment = new Domain.Models.TeachingAssignment
            {
                TeacherId = dto.TeacherId,
                SubjectId = defaultSubject.Value, // Ép kiểu từ int? sang int
                ClassId = dto.ClassId,
                SemesterId = dto.SemesterId,
                IsHomeroomTeacher = true
            };
            _context.TeachingAssignments.Add(newAssignment);
        }

        await _context.SaveChangesAsync();
    }
}