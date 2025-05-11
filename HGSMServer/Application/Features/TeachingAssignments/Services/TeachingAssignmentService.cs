using Application.Features.Classes.DTOs;
using Application.Features.Subjects.DTOs;
using Application.Features.Teachers.DTOs;
using Application.Features.TeachingAssignments.DTOs;
using Application.Features.TeachingAssignments.Interfaces;
using Application.Features.Semesters.Interfaces; 
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TeachingAssignmentService : ITeachingAssignmentService
    {
        private readonly HgsdbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISemesterService _semesterService;

        public TeachingAssignmentService(
            HgsdbContext context,
            IHttpContextAccessor httpContextAccessor,
            ISemesterService semesterService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _semesterService = semesterService;
        }

        private async Task ValidateSemesterNotStarted(int semesterId)
        {
            var semester = await _semesterService.GetByIdAsync(semesterId);
            if (semester == null)
            {
                throw new ArgumentException("Học kỳ không tồn tại.");
            }

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (currentDate >= DateOnly.Parse(semester.StartDate.ToString()))
            {
                throw new InvalidOperationException("Không thể thay đổi phân công giảng dạy vì kỳ học đã bắt đầu.");
            }
        }

        public async Task CreateTeachingAssignmentsAsync(List<TeachingAssignmentCreateDto> dtos)
        {
            foreach (var dto in dtos)
            {
                // Kiểm tra kỳ học chưa bắt đầu
                await ValidateSemesterNotStarted(dto.SemesterId);

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

                // Xóa các phân công hiện tại của giáo viên cho môn học và kỳ học này
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

                    // Kiểm tra xem lớp này đã được phân công cho giáo viên khác dạy môn học này trong kỳ học này chưa
                    var existingAssignmentForClass = await _context.TeachingAssignments
                        .FirstOrDefaultAsync(ta => ta.ClassId == classAssignment.ClassId
                                                && ta.SubjectId == dto.SubjectId
                                                && ta.SemesterId == dto.SemesterId
                                                && ta.TeacherId != dto.TeacherId); // Không tính chính giáo viên hiện tại (trong trường hợp cập nhật)

                    if (existingAssignmentForClass != null)
                    {
                        throw new InvalidOperationException($"Lớp {classEntity.ClassName} đã được phân công cho giáo viên khác dạy môn {subject.SubjectName} trong học kỳ này.");
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

        public async Task UpdateTeachingAssignmentsAsync(int teacherId, int semesterId, List<TeachingAssignmentUpdateDto> dtos)
        {
            // Kiểm tra kỳ học chưa bắt đầu
            await ValidateSemesterNotStarted(semesterId);

            if (!dtos.Any())
            {
                throw new ArgumentException("Danh sách phân công không được để trống.");
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

            // Xóa tất cả phân công hiện tại của giáo viên trong học kỳ
            var existingAssignments = await _context.TeachingAssignments
                .Where(ta => ta.TeacherId == teacherId && ta.SemesterId == semesterId)
                .ToListAsync();
            _context.TeachingAssignments.RemoveRange(existingAssignments);

            // Lấy danh sách môn học mà giáo viên được phân công
            var teacherSubjects = await _context.TeacherSubjects
                .Where(ts => ts.TeacherId == teacherId)
                .Select(ts => ts.SubjectId)
                .ToListAsync();

            foreach (var dto in dtos)
            {
                // Kiểm tra môn học có trong TeacherSubjects
                if (!teacherSubjects.Contains(dto.SubjectId))
                {
                    throw new ArgumentException($"Giáo viên chưa được phân công dạy môn học với ID {dto.SubjectId}.");
                }

                var subject = await _context.Subjects.FindAsync(dto.SubjectId);
                if (subject == null)
                {
                    throw new ArgumentException($"Môn học với ID {dto.SubjectId} không tồn tại.");
                }

                foreach (var classAssignment in dto.ClassAssignments)
                {
                    var classEntity = await _context.Classes.FindAsync(classAssignment.ClassId);
                    if (classEntity == null)
                    {
                        throw new ArgumentException($"Lớp học với ID {classAssignment.ClassId} không tồn tại.");
                    }

                    // Kiểm tra xem lớp này đã được phân công cho giáo viên khác dạy môn học này trong kỳ học này chưa
                    var existingAssignmentForClass = await _context.TeachingAssignments
                        .FirstOrDefaultAsync(ta => ta.ClassId == classAssignment.ClassId
                                                && ta.SubjectId == dto.SubjectId
                                                && ta.SemesterId == semesterId
                                                && ta.TeacherId != teacherId);

                    if (existingAssignmentForClass != null)
                    {
                        throw new InvalidOperationException($"Lớp {classEntity.ClassName} đã được phân công cho giáo viên khác dạy môn {subject.SubjectName} trong học kỳ này.");
                    }

                    var newAssignment = new Domain.Models.TeachingAssignment
                    {
                        TeacherId = teacherId,
                        SubjectId = dto.SubjectId,
                        ClassId = classAssignment.ClassId,
                        SemesterId = semesterId
                    };
                    _context.TeachingAssignments.Add(newAssignment);
                }
            }

            await _context.SaveChangesAsync();
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
            // Kiểm tra kỳ học chưa bắt đầu
            await ValidateSemesterNotStarted(semesterId);

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
}