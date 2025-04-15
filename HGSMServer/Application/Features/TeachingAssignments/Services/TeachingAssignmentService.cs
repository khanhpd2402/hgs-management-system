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

    //private async Task<bool> HasPermissionAsync()
    //{
    //    var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
    //    var allowedRoles = new[] { "Principal", "VicePrincipal", "HeadOfDepartment", "AdministrativeOfficer" };
    //    return allowedRoles.Contains(userRole);
    //}

    private async Task<bool> HasHomeroomPermissionAsync()
    {
        var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        var allowedRoles = new[] { "Hiệu trưởng", "Cán bộ văn thư", "Hiệu phó" }; // Chỉ Hiệu trưởng và Cán bộ văn thư
        return allowedRoles.Contains(userRole);
    }

    public async Task CreateTeachingAssignmentsAsync(List<TeachingAssignmentCreateDto> dtos)
    {
        // Kiểm tra quyền (nếu cần)
        // if (!await HasPermissionAsync())
        // {
        //     throw new UnauthorizedAccessException("You do not have permission to assign teaching duties.");
        // }

        foreach (var dto in dtos)
        {
            // Validate teacher
            var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
            if (teacher == null)
            {
                throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.");
            }

            // Validate subject
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectId == dto.SubjectId && s.SubjectCategory == dto.SubjectCategory);
            if (subject == null)
            {
                throw new ArgumentException($"Subject with Id {dto.SubjectId} and category {dto.SubjectCategory} does not exist.");
            }

            // Validate semester
            var semester = await _context.Semesters.FindAsync(dto.SemesterId);
            if (semester == null)
            {
                throw new ArgumentException($"Semester with Id {dto.SemesterId} does not exist.");
            }

            // Validate teacher-subject assignment
            var teacherSubject = await _context.TeacherSubjects
                .FirstOrDefaultAsync(ts => ts.TeacherId == dto.TeacherId && ts.SubjectId == dto.SubjectId);
            if (teacherSubject == null)
            {
                throw new ArgumentException($"Teacher {teacher.FullName} is not assigned to subject {subject.SubjectName}.");
            }

            // Process class assignments
            foreach (var classAssignment in dto.ClassAssignments)
            {
                var classEntity = await _context.Classes.FindAsync(classAssignment.ClassId);
                if (classEntity == null)
                {
                    throw new ArgumentException($"Class with Id {classAssignment.ClassId} does not exist.");
                }

                // Check for existing assignment
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
                        IsHomeroomTeacher = false
                    };
                    _context.TeachingAssignments.Add(assignment);
                }
            }
        }

        await _context.SaveChangesAsync();
    }
    
    public async Task<List<TeachingAssignmentResponseDto>> GetAllTeachingAssignmentsAsync()
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

        // Nếu là giáo viên, chỉ trả về assignments của họ
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
                IsHomeroomTeacher = ta.IsHomeroomTeacher
            });
        }

        return result;
    }

    public async Task<TeachingAssignmentFilterDataDto> GetFilterDataAsync()
    {
        var filterData = new TeachingAssignmentFilterDataDto
        {
            Teachers = await _context.Teachers
                .Include(t => t.User) // Để lấy Email, PhoneNumber
                .Select(t => new TeacherListDto
                {
                    TeacherId = t.TeacherId,
                    FullName = t.FullName,
                    Dob = t.Dob, // Trực tiếp gán vì DOB là NOT NULL và đã là DateOnly
                    Gender = t.Gender ?? "Unknown",
                    Ethnicity = t.Ethnicity,
                    Religion = t.Religion,
                    MaritalStatus = t.MaritalStatus,
                    IdcardNumber = t.IdcardNumber,
                    InsuranceNumber = t.InsuranceNumber,
                    EmploymentType = t.EmploymentType,
                    Position = t.Position,
                    Department = t.Department,
                    IsHeadOfDepartment = t.IsHeadOfDepartment,
                    EmploymentStatus = t.EmploymentStatus,
                    RecruitmentAgency = t.RecruitmentAgency,
                    HiringDate = t.HiringDate, // Nullable, gán trực tiếp
                    PermanentEmploymentDate = t.PermanentEmploymentDate, // Nullable, gán trực tiếp
                    SchoolJoinDate = t.SchoolJoinDate, // Trực tiếp gán vì NOT NULL
                    PermanentAddress = t.PermanentAddress,
                    Hometown = t.Hometown,
                    Email = t.User != null ? t.User.Email : "N/A",
                    PhoneNumber = t.User != null ? t.User.PhoneNumber : "N/A",
                    Subjects = t.TeacherSubjects.Select(ts => new SubjectTeacherDto
                    {
                        SubjectName = ts.Subject.SubjectName,
                        IsMainSubject = ts.IsMainSubject ?? false
                    }).ToList()
                })
                .ToListAsync(),
            Subjects = await _context.Subjects
                .Select(s => new SubjectDto
                {
                    SubjectID = s.SubjectId,
                    SubjectName = s.SubjectName,
                    SubjectCategory = s.SubjectCategory ?? "General",
                    TypeOfGrade = s.TypeOfGrade ?? "Tính điểm"
                })
                .ToListAsync(),
            Classes = await _context.Classes
                .Include(c => c.GradeLevel)
                .Select(c => new ClassDto
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName,
                    GradeLevelId = c.GradeLevelId
                })
                .OrderBy(c => c.ClassName)
                .ToListAsync()
        };

        return filterData;
    }
    public async Task UpdateTeachingAssignmentsAsync(List<TeachingAssignmentUpdateDto> dtos)
    {
        foreach (var dto in dtos)
        {
            // Validate teacher
            var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
            if (teacher == null)
                throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.");

            // Validate subject
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectId == dto.SubjectId && s.SubjectCategory == dto.SubjectCategory);
            if (subject == null)
                throw new ArgumentException($"Subject with Id {dto.SubjectId} and category {dto.SubjectCategory} does not exist.");

            // Validate semester
            var semester = await _context.Semesters.FindAsync(dto.SemesterId);
            if (semester == null)
                throw new ArgumentException($"Semester with Id {dto.SemesterId} does not exist.");

            // Validate teacher-subject assignment
            var teacherSubject = await _context.TeacherSubjects
                .FirstOrDefaultAsync(ts => ts.TeacherId == dto.TeacherId && ts.SubjectId == dto.SubjectId);
            if (teacherSubject == null)
                throw new ArgumentException($"Teacher {teacher.FullName} is not assigned to subject {subject.SubjectName}.");

            foreach (var classAssignment in dto.ClassAssignments)
            {
                var classEntity = await _context.Classes.FindAsync(classAssignment.ClassId);
                if (classEntity == null)
                    throw new ArgumentException($"Class with Id {classAssignment.ClassId} does not exist.");

                if (dto.AssignmentId.HasValue)
                {
                    // Update existing assignment
                    var assignment = await _context.TeachingAssignments
                        .FirstOrDefaultAsync(ta => ta.AssignmentId == dto.AssignmentId.Value);
                    if (assignment == null)
                        throw new ArgumentException($"Teaching assignment with Id {dto.AssignmentId} does not exist.");

                    assignment.TeacherId = dto.TeacherId;
                    assignment.SubjectId = dto.SubjectId;
                    assignment.ClassId = classAssignment.ClassId;
                    assignment.SemesterId = dto.SemesterId;
                    _context.TeachingAssignments.Update(assignment);
                }
                else
                {
                    // Create new assignment if it doesn’t exist
                    var existingAssignment = await _context.TeachingAssignments
                        .FirstOrDefaultAsync(ta =>
                            ta.TeacherId == dto.TeacherId &&
                            ta.SubjectId == dto.SubjectId &&
                            ta.ClassId == classAssignment.ClassId &&
                            ta.SemesterId == dto.SemesterId);

                    if (existingAssignment == null)
                    {
                        var newAssignment = new Domain.Models.TeachingAssignment
                        {
                            TeacherId = dto.TeacherId,
                            SubjectId = dto.SubjectId,
                            ClassId = classAssignment.ClassId,
                            SemesterId = dto.SemesterId,
                            IsHomeroomTeacher = false
                        };
                        _context.TeachingAssignments.Add(newAssignment);
                    }
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<TeachingAssignmentResponseDto>> GetAssignmentsForCreationAsync(TeachingAssignmentCreateDto dto)
    {
        //if (!await HasPermissionAsync())
        //{
        //    throw new UnauthorizedAccessException("You do not have permission to access this data.");
        //}

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
    public async Task UpdateHomeroomAssignmentsAsync(List<UpdateHomeroomDto> dtos)
    {
        // Kiểm tra quyền
        if (!await HasHomeroomPermissionAsync())
        {
            throw new UnauthorizedAccessException("You do not have permission to update homeroom duties.");
        }

        foreach (var dto in dtos)
        {
            // Validate AssignmentId
            var assignment = await _context.TeachingAssignments
                .Include(ta => ta.Class)
                .Include(ta => ta.Semester)
                .FirstOrDefaultAsync(ta => ta.AssignmentId == dto.AssignmentId);
            if (assignment == null)
            {
                throw new ArgumentException($"Teaching assignment with Id {dto.AssignmentId} does not exist.");
            }

            // Nếu set IsHomeroomTeacher = true, kiểm tra xem lớp đã có giáo viên chủ nhiệm chưa
            if (dto.IsHomeroomTeacher)
            {
                var hasHomeroomTeacher = await _context.TeachingAssignments
                    .AnyAsync(ta => ta.ClassId == assignment.ClassId &&
                                   ta.SemesterId == assignment.SemesterId &&
                                   ta.IsHomeroomTeacher == true &&
                                   ta.AssignmentId != dto.AssignmentId);
                if (hasHomeroomTeacher)
                {
                    throw new InvalidOperationException($"Class {assignment.Class.ClassName} already has a homeroom teacher in semester {assignment.SemesterId}.");
                }
            }

            // Cập nhật chỉ trường IsHomeroomTeacher
            assignment.IsHomeroomTeacher = dto.IsHomeroomTeacher;
            _context.TeachingAssignments.Update(assignment);
        }

        await _context.SaveChangesAsync();
    }
}