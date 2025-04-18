using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Features.HomeRooms.DTOs;
using Application.Features.HomeRooms.Interfaces;
using Application.Features.TeachingAssignments.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HomeRooms.Services
{
    public class AssignHomeRoomService : IAssignHomeRoomService
    {
        private readonly HgsdbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssignHomeRoomService(HgsdbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<bool> HasHomeroomPermissionAsync()
        {
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            var allowedRoles = new[] { "Hiệu trưởng", "Cán bộ văn thư", "Hiệu phó" };
            return allowedRoles.Contains(userRole);
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

            var hasHomeroomTeacher = await _context.HomeroomAssignments
                .AnyAsync(ha => ha.ClassId == dto.ClassId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động");
            if (hasHomeroomTeacher)
            {
                throw new InvalidOperationException($"Class with Id {dto.ClassId} already has an active homeroom teacher in semester {dto.SemesterId}.");
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
            if (!await HasHomeroomPermissionAsync())
            {
                throw new UnauthorizedAccessException("You do not have permission to update homeroom duties.");
            }

            foreach (var dto in dtos)
            {
                var assignment = await _context.HomeroomAssignments
                    .Include(ha => ha.Class)
                    .Include(ha => ha.Semester)
                    .FirstOrDefaultAsync(ha => ha.ClassId == dto.ClassId && ha.SemesterId == dto.SemesterId);

                if (assignment == null)
                {
                    throw new ArgumentException($"Homeroom assignment for ClassId {dto.ClassId} and SemesterId {dto.SemesterId} does not exist.");
                }

                if (dto.TeacherId > 0 && dto.TeacherId != assignment.TeacherId)
                {
                    var newTeacher = await _context.Teachers.FindAsync(dto.TeacherId);
                    if (newTeacher == null)
                    {
                        throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.");
                    }

                    var hasActiveHomeroom = await _context.HomeroomAssignments
                        .AnyAsync(ha => ha.ClassId == dto.ClassId &&
                                       ha.SemesterId == dto.SemesterId &&
                                       ha.Status == "Hoạt Động" &&
                                       ha.HomeroomAssignmentId != assignment.HomeroomAssignmentId);
                    if (hasActiveHomeroom)
                    {
                        throw new InvalidOperationException($"Class {assignment.Class.ClassName} already has an active homeroom teacher in semester {dto.SemesterId}.");
                    }

                    assignment.Status = "Không Hoạt Động";
                    _context.HomeroomAssignments.Update(assignment);

                    var newAssignment = new HomeroomAssignment
                    {
                        TeacherId = dto.TeacherId,
                        ClassId = dto.ClassId,
                        SemesterId = dto.SemesterId,
                        Status = "Hoạt Động"
                    };
                    _context.HomeroomAssignments.Add(newAssignment);
                }
                else
                {
                    assignment.Status = dto.Status;
                    _context.HomeroomAssignments.Update(assignment);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<HomeroomAssignmentResponseDto>> GetAllHomeroomAssignmentsAsync()
        {
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

            var query = _context.HomeroomAssignments
                .Include(ha => ha.Teacher)
                .Include(ha => ha.Class)
                .Include(ha => ha.Semester)
                .AsQueryable();

            if (userRole == "Teacher")
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
                if (teacher != null)
                {
                    query = query.Where(ha => ha.TeacherId == teacher.TeacherId);
                }
                else
                {
                    throw new UnauthorizedAccessException("Teacher not found.");
                }
            }

            var assignmentList = await query.ToListAsync();
            return assignmentList.Select(ha => new HomeroomAssignmentResponseDto
            {
                HomeroomAssignmentId = ha.HomeroomAssignmentId,
                TeacherId = ha.TeacherId,
                TeacherName = ha.Teacher.FullName,
                ClassId = ha.ClassId,
                ClassName = ha.Class.ClassName,
                SemesterId = ha.SemesterId,
                SemesterName = ha.Semester.SemesterName,
                Status = ha.Status
            }).ToList();
        }
    }
}