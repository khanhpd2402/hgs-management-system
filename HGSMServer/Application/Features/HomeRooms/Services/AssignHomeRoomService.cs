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
                throw new UnauthorizedAccessException("User does not have permission to assign homeroom duties.");
            }

            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.TeacherId <= 0) throw new ArgumentException("TeacherId must be a positive integer.", nameof(dto.TeacherId));
            if (dto.ClassId <= 0) throw new ArgumentException("ClassId must be a positive integer.", nameof(dto.ClassId));
            if (dto.SemesterId <= 0) throw new ArgumentException("SemesterId must be a positive integer.", nameof(dto.SemesterId));

            var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
            if (teacher == null) throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.", nameof(dto.TeacherId));
            var classEntity = await _context.Classes.FindAsync(dto.ClassId);
            if (classEntity == null) throw new ArgumentException($"Class with Id {dto.ClassId} does not exist.", nameof(dto.ClassId));
            var semester = await _context.Semesters.FindAsync(dto.SemesterId);
            if (semester == null) throw new ArgumentException($"Semester with Id {dto.SemesterId} does not exist.", nameof(dto.SemesterId));

            bool classAlreadyHasActive = await _context.HomeroomAssignments
                .AnyAsync(ha => ha.ClassId == dto.ClassId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động");
            if (classAlreadyHasActive)
            {
                throw new InvalidOperationException($"Cannot assign homeroom teacher. Class '{classEntity.ClassName}' already has an active homeroom teacher in semester '{semester.SemesterName}'.");
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
                string existingClassName = existingAssignment?.Class?.ClassName ?? "another class";
                throw new InvalidOperationException($"Cannot assign Teacher '{teacher.FullName}' (ID: {dto.TeacherId}) to class '{classEntity.ClassName}'. Teacher is already assigned as an active homeroom teacher for '{existingClassName}' in the same semester ({semester.SemesterName}).");
            }

            var newAssignment = new HomeroomAssignment
            {
                TeacherId = dto.TeacherId,
                ClassId = dto.ClassId,
                SemesterId = dto.SemesterId,
                Status = "Hoạt Động"
            };
            _context.HomeroomAssignments.Add(newAssignment);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateHomeroomAssignmentsAsync(List<UpdateHomeroomDto> dtos)
        {
            if (!HasHomeroomPermission())
            {
                throw new UnauthorizedAccessException("User does not have permission to update homeroom duties.");
            }
            if (dtos == null || !dtos.Any()) return;
            if (dtos.Any(d => d == null)) throw new ArgumentException("The list contains null DTO entries.", nameof(dtos));

            var duplicateCheck = dtos
               .Where(d => d.TeacherId > 0)  // We're only checking for teacher duplicates now
               .GroupBy(d => new { d.ClassId, d.SemesterId })
               .Where(g => g.Count() > 1)
               .Select(g => $"ClassId {g.Key.ClassId}, SemesterId {g.Key.SemesterId}")
               .ToList();
            if (duplicateCheck.Any())
            {
                throw new ArgumentException($"Duplicate active assignments requested within the batch for: {string.Join("; ", duplicateCheck)}");
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
                if (!existingClasses.ContainsKey(dto.ClassId)) throw new ArgumentException($"Class with Id {dto.ClassId} provided in DTO does not exist.");
                if (!existingSemesters.ContainsKey(dto.SemesterId)) throw new ArgumentException($"Semester with Id {dto.SemesterId} provided in DTO does not exist.");
                int targetTeacherId = dto.TeacherId;
                if (targetTeacherId > 0 && !existingTeachers.ContainsKey(targetTeacherId)) throw new ArgumentException($"Teacher with Id {targetTeacherId} provided for ClassId {dto.ClassId}, SemesterId {dto.SemesterId} does not exist.");

                var currentAssignmentsForDto = assignmentLookup[new { dto.ClassId, dto.SemesterId }].ToList();
                var assignmentToUpdate = currentAssignmentsForDto.FirstOrDefault(a => a.Status == "Hoạt Động") ?? currentAssignmentsForDto.FirstOrDefault();

                if (assignmentToUpdate == null)
                {
                    if (targetTeacherId <= 0) { Console.WriteLine($"Skipping creation for ClassId {dto.ClassId}, SemesterId {dto.SemesterId} due to missing TeacherId."); continue; }

                    // Since we're not getting Status from DTO, new assignments are always "Hoạt Động"
                    string targetStatusCreate = "Hoạt Động";

                    bool classAlreadyHasActive = relevantAssignments.Any(ha => ha.ClassId == dto.ClassId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động") ||
                                                 _context.HomeroomAssignments.Local.Any(ha => ha.ClassId == dto.ClassId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động");
                    if (targetStatusCreate == "Hoạt Động" && classAlreadyHasActive)
                    {
                        throw new InvalidOperationException($"Cannot create assignment. Class {existingClasses[dto.ClassId].ClassName} already has an active homeroom teacher in semester {existingSemesters[dto.SemesterId].SemesterName}.");
                    }

                    bool teacherAlreadyActiveInSemester = relevantAssignments.Any(ha => ha.TeacherId == targetTeacherId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động") ||
                                                         _context.HomeroomAssignments.Local.Any(ha => ha.TeacherId == targetTeacherId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động");
                    if (teacherAlreadyActiveInSemester)
                    {
                        var existingAssignmentOtherClass = relevantAssignments.FirstOrDefault(ha => ha.TeacherId == targetTeacherId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động")
                          ?? _context.HomeroomAssignments.Local.FirstOrDefault(ha => ha.TeacherId == targetTeacherId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động");
                        string existingClassName = "another class";
                        if (existingAssignmentOtherClass != null && existingClasses.ContainsKey(existingAssignmentOtherClass.ClassId)) existingClassName = existingClasses[existingAssignmentOtherClass.ClassId].ClassName;
                        throw new InvalidOperationException($"Cannot assign Teacher {existingTeachers[targetTeacherId].FullName} (ID: {targetTeacherId}). Teacher is already active homeroom teacher for '{existingClassName}' in semester {existingSemesters[dto.SemesterId].SemesterName}.");
                    }

                    var newAssignment = new HomeroomAssignment { TeacherId = targetTeacherId, ClassId = dto.ClassId, SemesterId = dto.SemesterId, Status = targetStatusCreate };
                    _context.HomeroomAssignments.Add(newAssignment);
                    relevantAssignments.Add(newAssignment);
                    Console.WriteLine($"Marking new assignment for creation: ClassId {dto.ClassId}, SemesterId {dto.SemesterId}, TeacherId {targetTeacherId}, Status: {newAssignment.Status}.");
                }
                else
                {
                    bool teacherChanged = targetTeacherId > 0 && targetTeacherId != assignmentToUpdate.TeacherId;
                    if (!teacherChanged) { Console.WriteLine($"No changes needed for assignment {assignmentToUpdate.HomeroomAssignmentId}."); continue; }

                    int finalTeacherId = teacherChanged ? targetTeacherId : assignmentToUpdate.TeacherId;

                    // We no longer check or update status here!

                    if (teacherChanged)
                    {
                        assignmentToUpdate.TeacherId = finalTeacherId;
                        _context.HomeroomAssignments.Update(assignmentToUpdate);
                        Console.WriteLine($"Marking assignment {assignmentToUpdate.HomeroomAssignmentId} for update. New TeacherId: {assignmentToUpdate.TeacherId}.");
                    }
                }
            }

            await _context.SaveChangesAsync();
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
                if (userId.HasValue)
                {
                    var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId.Value);
                    if (teacher != null)
                    {
                        query = query.Where(ha => ha.TeacherId == teacher.TeacherId);
                    }
                    else
                    { 
                        // throw new UnauthorizedAccessException("Teacher profile not found for the current user.");
                        return new List<HomeroomAssignmentResponseDto>();
                    }
                }
                else
                {
                    return new List<HomeroomAssignmentResponseDto>();
                }
            }

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
                // AcademicYearName = ha.Semester?.AcademicYear?.YearName ?? "N/A",
                Status = ha.Status
            }).ToList();
        }
    }
}