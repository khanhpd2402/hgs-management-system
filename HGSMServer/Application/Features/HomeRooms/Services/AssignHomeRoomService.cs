using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Features.HomeRooms.DTOs;
using Application.Features.HomeRooms.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HomeRooms.Services
{
    public class AssignHomeRoomService : IAssignHomeRoomService
    {
        private readonly HgsdbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly string[] AllowedRoles = { "Hiệu trưởng", "Cán bộ văn thư", "Hiệu phó" };

        public AssignHomeRoomService(HgsdbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        private bool HasHomeroomPermission()
        {
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            return !string.IsNullOrEmpty(userRole) && AllowedRoles.Contains(userRole);
        }

        public async Task AssignHomeroomAsync(AssignHomeroomDto dto)
        {
            if (!HasHomeroomPermission())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền để phân công giáo viên chủ nhiệm.");
            }

            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Thông tin phân công không được để trống.");
            }

            if (dto.TeacherId <= 0)
            {
                throw new ArgumentException("TeacherId phải là một số nguyên dương.", nameof(dto.TeacherId));
            }

            if (dto.ClassId <= 0)
            {
                throw new ArgumentException("ClassId phải là một số nguyên dương.", nameof(dto.ClassId));
            }

            if (dto.SemesterId <= 0)
            {
                throw new ArgumentException("SemesterId phải là một số nguyên dương.", nameof(dto.SemesterId));
            }

            var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
            if (teacher == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy giáo viên với ID {dto.TeacherId}.");
            }

            var classEntity = await _context.Classes.FindAsync(dto.ClassId);
            if (classEntity == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lớp học với ID {dto.ClassId}.");
            }

            var semester = await _context.Semesters.FindAsync(dto.SemesterId);
            if (semester == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy học kỳ với ID {dto.SemesterId}.");
            }

            bool classAlreadyHasActive = await _context.HomeroomAssignments
                .AnyAsync(ha => ha.ClassId == dto.ClassId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động");
            if (classAlreadyHasActive)
            {
                throw new InvalidOperationException($"Lớp '{classEntity.ClassName}' đã có giáo viên chủ nhiệm đang hoạt động trong học kỳ '{semester.SemesterName}'.");
            }

            bool teacherAlreadyActiveInSemester = await _context.HomeroomAssignments
                .AnyAsync(ha => ha.TeacherId == dto.TeacherId &&
                               ha.SemesterId == dto.SemesterId &&
                               ha.Status == "Hoạt Động");
            if (teacherAlreadyActiveInSemester)
            {
                var existingAssignment = await _context.HomeroomAssignments
                                            .Include(ha => ha.Class)
                                            .FirstOrDefaultAsync(ha => ha.TeacherId == dto.TeacherId &&
                                                                        ha.SemesterId == dto.SemesterId &&
                                                                        ha.Status == "Hoạt Động");
                string existingClassName = existingAssignment?.Class?.ClassName ?? "một lớp khác";
                throw new InvalidOperationException($"Giáo viên '{teacher.FullName}' (ID: {dto.TeacherId}) đã được phân công làm giáo viên chủ nhiệm cho lớp '{existingClassName}' trong học kỳ '{semester.SemesterName}'.");
            }

            var newAssignment = new HomeroomAssignment
            {
                TeacherId = dto.TeacherId,
                ClassId = dto.ClassId,
                SemesterId = dto.SemesterId,
                Status = "Hoạt Động"
            };

            try
            {
                _context.HomeroomAssignments.Add(newAssignment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể phân công giáo viên chủ nhiệm do lỗi hệ thống.", ex);
            }
        }

        public async Task UpdateHomeroomAssignmentsAsync(List<UpdateHomeroomDto> dtos)
        {
            if (!HasHomeroomPermission())
            {
                throw new UnauthorizedAccessException("Bạn không có quyền để cập nhật phân công giáo viên chủ nhiệm.");
            }

            if (dtos == null || !dtos.Any())
            {
                return;
            }

            if (dtos.Any(d => d == null))
            {
                throw new ArgumentException("Danh sách chứa thông tin phân công không hợp lệ.", nameof(dtos));
            }

            var duplicateCheck = dtos
               .Where(d => d.TeacherId > 0)
               .GroupBy(d => new { d.ClassId, d.SemesterId })
               .Where(g => g.Count() > 1)
               .Select(g => $"ClassId {g.Key.ClassId}, SemesterId {g.Key.SemesterId}")
               .ToList();
            if (duplicateCheck.Any())
            {
                throw new ArgumentException($"Phân công trùng lặp trong danh sách: {string.Join("; ", duplicateCheck)}.");
            }

            var classIds = dtos.Select(d => d.ClassId).Distinct().ToList();
            var semesterIds = dtos.Select(d => d.SemesterId).Distinct().ToList();
            var teacherIds = dtos.Where(d => d.TeacherId > 0).Select(d => d.TeacherId).Distinct().ToList();

            var existingClasses = await _context.Classes.Where(c => classIds.Contains(c.ClassId)).ToDictionaryAsync(c => c.ClassId);
            var existingSemesters = await _context.Semesters.Where(s => semesterIds.Contains(s.SemesterId)).ToDictionaryAsync(s => s.SemesterId);
            var existingTeachers = await _context.Teachers.Where(t => teacherIds.Contains(t.TeacherId)).ToDictionaryAsync(t => t.TeacherId);

            var relevantAssignments = await _context.HomeroomAssignments
                .Where(ha => (classIds.Contains(ha.ClassId) && semesterIds.Contains(ha.SemesterId)) ||
                             (teacherIds.Contains(ha.TeacherId) && semesterIds.Contains(ha.SemesterId) && ha.Status == "Hoạt Động"))
                .ToListAsync();

            var assignmentLookup = relevantAssignments.ToLookup(ha => new { ha.ClassId, ha.SemesterId });

            foreach (var dto in dtos)
            {
                if (!existingClasses.ContainsKey(dto.ClassId))
                {
                    throw new KeyNotFoundException($"Không tìm thấy lớp học với ID {dto.ClassId}.");
                }

                if (!existingSemesters.ContainsKey(dto.SemesterId))
                {
                    throw new KeyNotFoundException($"Không tìm thấy học kỳ với ID {dto.SemesterId}.");
                }

                int targetTeacherId = dto.TeacherId;
                if (targetTeacherId > 0 && !existingTeachers.ContainsKey(targetTeacherId))
                {
                    throw new KeyNotFoundException($"Không tìm thấy giáo viên với ID {targetTeacherId} cho lớp ID {dto.ClassId}, học kỳ ID {dto.SemesterId}.");
                }

                var currentAssignmentsForDto = assignmentLookup[new { dto.ClassId, dto.SemesterId }].ToList();
                var assignmentToUpdate = currentAssignmentsForDto.FirstOrDefault(a => a.Status == "Hoạt Động") ?? currentAssignmentsForDto.FirstOrDefault();

                if (assignmentToUpdate == null)
                {
                    if (targetTeacherId <= 0)
                    {
                        Console.WriteLine($"Bỏ qua việc tạo mới cho lớp ID {dto.ClassId}, học kỳ ID {dto.SemesterId} do thiếu TeacherId.");
                        continue;
                    }

                    string targetStatusCreate = "Hoạt Động";

                    bool classAlreadyHasActive = relevantAssignments.Any(ha => ha.ClassId == dto.ClassId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động") ||
                                                _context.HomeroomAssignments.Local.Any(ha => ha.ClassId == dto.ClassId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động");
                    if (targetStatusCreate == "Hoạt Động" && classAlreadyHasActive)
                    {
                        throw new InvalidOperationException($"Lớp {existingClasses[dto.ClassId].ClassName} đã có giáo viên chủ nhiệm đang hoạt động trong học kỳ {existingSemesters[dto.SemesterId].SemesterName}.");
                    }

                    bool teacherAlreadyActiveInSemester = relevantAssignments.Any(ha => ha.TeacherId == targetTeacherId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động") ||
                                                         _context.HomeroomAssignments.Local.Any(ha => ha.TeacherId == targetTeacherId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động");
                    if (teacherAlreadyActiveInSemester)
                    {
                        var existingAssignmentOtherClass = relevantAssignments.FirstOrDefault(ha => ha.TeacherId == targetTeacherId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động")
                          ?? _context.HomeroomAssignments.Local.FirstOrDefault(ha => ha.TeacherId == targetTeacherId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động");
                        string existingClassName = "một lớp khác";
                        if (existingAssignmentOtherClass != null && existingClasses.ContainsKey(existingAssignmentOtherClass.ClassId))
                        {
                            existingClassName = existingClasses[existingAssignmentOtherClass.ClassId].ClassName;
                        }
                        throw new InvalidOperationException($"Giáo viên {existingTeachers[targetTeacherId].FullName} (ID: {targetTeacherId}) đã được phân công làm giáo viên chủ nhiệm cho lớp '{existingClassName}' trong học kỳ {existingSemesters[dto.SemesterId].SemesterName}.");
                    }

                    var newAssignment = new HomeroomAssignment { TeacherId = targetTeacherId, ClassId = dto.ClassId, SemesterId = dto.SemesterId, Status = targetStatusCreate };
                    _context.HomeroomAssignments.Add(newAssignment);
                    relevantAssignments.Add(newAssignment);
                    Console.WriteLine($"Tạo mới phân công: ClassId {dto.ClassId}, SemesterId {dto.SemesterId}, TeacherId {targetTeacherId}, Status: {newAssignment.Status}.");
                }
                else
                {
                    bool teacherChanged = targetTeacherId > 0 && targetTeacherId != assignmentToUpdate.TeacherId;
                    if (!teacherChanged)
                    {
                        Console.WriteLine($"Không cần thay đổi cho phân công {assignmentToUpdate.HomeroomAssignmentId}.");
                        continue;
                    }

                    int finalTeacherId = teacherChanged ? targetTeacherId : assignmentToUpdate.TeacherId;

                    if (teacherChanged)
                    {
                        assignmentToUpdate.TeacherId = finalTeacherId;
                        _context.HomeroomAssignments.Update(assignmentToUpdate);
                        Console.WriteLine($"Cập nhật phân công {assignmentToUpdate.HomeroomAssignmentId}. TeacherId mới: {assignmentToUpdate.TeacherId}.");
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể cập nhật phân công giáo viên chủ nhiệm do lỗi hệ thống.", ex);
            }
        }

        public async Task<List<HomeroomAssignmentResponseDto>> GetAllHomeroomAssignmentsAsync()
        {
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = userIdClaim != null && int.TryParse(userIdClaim, out int parsedUserId) ? parsedUserId : null;

            var query = _context.HomeroomAssignments
                .Include(ha => ha.Teacher)
                .Include(ha => ha.Class)
                .Include(ha => ha.Semester)
                .ThenInclude(s => s.AcademicYear)
                .AsNoTracking()
                .AsQueryable();

            if (userRole == "Teacher")
            {
                if (!userId.HasValue)
                {
                    return new List<HomeroomAssignmentResponseDto>();
                }

                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId.Value);
                if (teacher == null)
                {
                    return new List<HomeroomAssignmentResponseDto>();
                }

                query = query.Where(ha => ha.TeacherId == teacher.TeacherId);
            }

            try
            {
                var assignmentList = await query.ToListAsync();
                return assignmentList.Select(ha => new HomeroomAssignmentResponseDto
                {
                    HomeroomAssignmentId = ha.HomeroomAssignmentId,
                    TeacherId = ha.TeacherId,
                    TeacherName = ha.Teacher?.FullName ?? "N/A",
                    ClassId = ha.ClassId,
                    ClassName = ha.Class?.ClassName ?? "N/A",
                    SemesterId = ha.SemesterId,
                    SemesterName = ha.Semester?.SemesterName ?? "N/A",
                    Status = ha.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách phân công giáo viên chủ nhiệm do lỗi hệ thống.", ex);
            }
        }
    }
}