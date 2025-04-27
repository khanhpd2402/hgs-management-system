using Application.Features.Classes.DTOs;
using Application.Features.Subjects.DTOs;
using Application.Features.Teachers.DTOs;
using Application.Features.TeachingAssignments.DTOs;
using Application.Features.TeachingAssignments.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TeachingAssignmentService : ITeachingAssignmentService
{
    private readonly HgsdbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TeachingAssignmentService(HgsdbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task CreateTeachingAssignmentsAsync(List<TeachingAssignmentCreateDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
            if (teacher == null)
            {
                throw new ArgumentException("Giáo viên không tồn tại.");
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectId == dto.SubjectId);
            if (subject == null)
            {
                throw new ArgumentException("Môn học không tồn tại.");
            }

            var semester = await _context.Semesters.FindAsync(dto.SemesterId);
            if (semester == null)
            {
                throw new ArgumentException("Học kỳ không tồn tại.");
            }

            var teacherSubject = await _context.TeacherSubjects
                .FirstOrDefaultAsync(ts => ts.TeacherId == dto.TeacherId && ts.SubjectId == dto.SubjectId);
            if (teacherSubject == null)
            {
                throw new ArgumentException("Giáo viên chưa được phân công dạy môn học này.");
            }

            var existingAssignments = await _context.TeachingAssignments
                .Where(ta => ta.TeacherId == dto.TeacherId && ta.SubjectId == dto.SubjectId && ta.SemesterId == dto.SemesterId)
                .ToListAsync();
            _context.TeachingAssignments.RemoveRange(existingAssignments);

            foreach (var classAssignment in dto.ClassAssignments)
            {
                var classEntity = await _context.Classes.FindAsync(classAssignment.ClassId);
                if (classEntity == null)
                {
                    throw new ArgumentException("Lớp học không tồn tại.");
                }

                var newAssignment = new Domain.Models.TeachingAssignment
                {
                    TeacherId = dto.TeacherId,
                    SubjectId = dto.SubjectId,
                    ClassId = classAssignment.ClassId,
                    SemesterId = dto.SemesterId
                };
                _context.TeachingAssignments.Add(newAssignment);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<TeachingAssignmentResponseDto>> GetAllTeachingAssignmentsAsync(int semesterId)
    {
        var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

        var query = _context.TeachingAssignments
            .Include(ta => ta.Teacher)
            .Include(ta => ta.Subject)
            .Include(ta => ta.Class)
            .Include(ta => ta.Semester)
            .Where(ta => ta.SemesterId == semesterId)
            .AsQueryable();

        if (userRole == "Giáo viên")
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher != null)
            {
                query = query.Where(ta => ta.TeacherId == teacher.TeacherId);
            }
            else
            {
                throw new UnauthorizedAccessException("Giáo viên không tồn tại.");
            }
        }

        var homeroomAssignments = await _context.HomeroomAssignments
            .Where(ha => ha.Status == "Hoạt Động" && ha.SemesterId == semesterId)
            .Include(ha => ha.Teacher)
            .ToListAsync();

        var assignmentList = await query.ToListAsync();
        var result = new List<TeachingAssignmentResponseDto>();

        foreach (var ta in assignmentList)
        {
            var homeroomTeacher = homeroomAssignments
                .FirstOrDefault(ha => ha.ClassId == ta.ClassId && ha.SemesterId == ta.SemesterId);

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
            });
        }

        return result;
    }

    public async Task<TeachingAssignmentFilterDataDto> GetFilterDataAsync()
    {
        var filterData = new TeachingAssignmentFilterDataDto
        {
            Teachers = await _context.Teachers
                .Include(t => t.User)
                .Select(t => new TeacherListDto
                {
                    TeacherId = t.TeacherId,
                    FullName = t.FullName,
                    Dob = t.Dob,
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
                    HiringDate = t.HiringDate,
                    PermanentEmploymentDate = t.PermanentEmploymentDate,
                    SchoolJoinDate = t.SchoolJoinDate,
                    PermanentAddress = t.PermanentAddress,
                    Hometown = t.Hometown,
                    Email = t.User != null ? t.User.Email : "N/A",
                    PhoneNumber = t.User != null ? t.User.PhoneNumber : "N/A",
                    Subjects = t.TeacherSubjects.Select(ts => new SubjectTeacherDto
                    {
                        SubjectId = ts.Subject.SubjectId,
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
        if (!dtos.Any())
        {
            throw new ArgumentException("Danh sách phân công không được để trống.");
        }

        var teacherId = dtos.First().TeacherId;
        var semesterId = dtos.First().SemesterId;

        if (dtos.Any(dto => dto.TeacherId != teacherId || dto.SemesterId != semesterId))
        {
            throw new ArgumentException("Tất cả phân công phải có cùng giáo viên và học kỳ.");
        }

        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            throw new ArgumentException("Giáo viên không tồn tại.");
        }

        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null)
        {
            throw new ArgumentException("Học kỳ không tồn tại.");
        }

        var existingAssignments = await _context.TeachingAssignments
            .Where(ta => ta.TeacherId == teacherId && ta.SemesterId == semesterId)
            .ToListAsync();
        _context.TeachingAssignments.RemoveRange(existingAssignments);

        foreach (var dto in dtos)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectId == dto.SubjectId && s.SubjectCategory == dto.SubjectCategory);
            if (subject == null)
            {
                throw new ArgumentException("Môn học không tồn tại.");
            }

            var teacherSubject = await _context.TeacherSubjects
                .FirstOrDefaultAsync(ts => ts.TeacherId == dto.TeacherId && ts.SubjectId == dto.SubjectId);
            if (teacherSubject == null)
            {
                throw new ArgumentException("Giáo viên chưa được phân công dạy môn học này.");
            }

            foreach (var classAssignment in dto.ClassAssignments)
            {
                var classEntity = await _context.Classes.FindAsync(classAssignment.ClassId);
                if (classEntity == null)
                {
                    throw new ArgumentException("Lớp học không tồn tại.");
                }

                var newAssignment = new Domain.Models.TeachingAssignment
                {
                    TeacherId = dto.TeacherId,
                    SubjectId = dto.SubjectId,
                    ClassId = classAssignment.ClassId,
                    SemesterId = dto.SemesterId
                };
                _context.TeachingAssignments.Add(newAssignment);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<TeachingAssignmentResponseDto>> GetAssignmentsForCreationAsync(TeachingAssignmentCreateDto dto)
    {
        var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
        if (teacher == null)
        {
            throw new ArgumentException("Giáo viên không tồn tại.");
        }

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == dto.SubjectId);
        if (subject == null)
        {
            throw new ArgumentException("Môn học không tồn tại.");
        }

        var semester = await _context.Semesters.FindAsync(dto.SemesterId);
        if (semester == null)
        {
            throw new ArgumentException("Học kỳ không tồn tại.");
        }

        var homeroomAssignments = await _context.HomeroomAssignments
            .Where(ha => ha.Status == "Hoạt Động")
            .Include(ha => ha.Teacher)
            .ToListAsync();

        var result = new List<TeachingAssignmentResponseDto>();

        foreach (var classAssignment in dto.ClassAssignments)
        {
            var classEntity = await _context.Classes.FindAsync(classAssignment.ClassId);
            if (classEntity == null)
            {
                throw new ArgumentException("Lớp học không tồn tại.");
            }

            var homeroomTeacher = homeroomAssignments
                .FirstOrDefault(ha => ha.ClassId == classAssignment.ClassId && ha.SemesterId == dto.SemesterId);

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
            });
        }

        return result;
    }

    public async Task<List<TeachingAssignmentResponseDto>> GetTeachingAssignmentsByTeacherIdAsync(int teacherId, int semesterId)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            throw new ArgumentException("Giáo viên không tồn tại.");
        }

        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null)
        {
            throw new ArgumentException("Học kỳ không tồn tại.");
        }

        var query = _context.TeachingAssignments
            .Include(ta => ta.Teacher)
            .Include(ta => ta.Subject)
            .Include(ta => ta.Class)
            .Include(ta => ta.Semester)
            .Where(ta => ta.TeacherId == teacherId && ta.SemesterId == semesterId)
            .AsQueryable();

        var homeroomAssignments = await _context.HomeroomAssignments
            .Where(ha => ha.Status == "Hoạt Động" && ha.SemesterId == semesterId)
            .Include(ha => ha.Teacher)
            .ToListAsync();

        var assignmentList = await query.ToListAsync();
        var result = new List<TeachingAssignmentResponseDto>();

        foreach (var ta in assignmentList)
        {
            var homeroomTeacher = homeroomAssignments
                .FirstOrDefault(ha => ha.ClassId == ta.ClassId && ha.SemesterId == ta.SemesterId);

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
            });
        }

        return result;
    }

    public async Task DeleteTeachingAssignmentsByTeacherIdAndSemesterIdAsync(int teacherId, int semesterId)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            throw new ArgumentException("Giáo viên không tồn tại.");
        }

        var semester = await _context.Semesters.FindAsync(semesterId);
        if (semester == null)
        {
            throw new ArgumentException("Học kỳ không tồn tại.");
        }

        var assignmentsToDelete = await _context.TeachingAssignments
            .Where(ta => ta.TeacherId == teacherId && ta.SemesterId == semesterId)
            .ToListAsync();

        if (!assignmentsToDelete.Any())
        {
            throw new ArgumentException("Không tìm thấy phân công giảng dạy nào cho giáo viên trong học kỳ này.");
        }

        _context.TeachingAssignments.RemoveRange(assignmentsToDelete);
        await _context.SaveChangesAsync();
    }
}