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

            var classIds = dtos.Select(d => d.ClassId).Distinct().ToList();
            var semesterIds = dtos.Select(d => d.SemesterId).Distinct().ToList();
            var teacherIds = dtos.Where(d => d.TeacherId > 0).Select(d => d.TeacherId).Distinct().ToList();

            var existingClasses = await _context.Classes.Where(c => classIds.Contains(c.ClassId)).ToDictionaryAsync(c => c.ClassId);
            var existingSemesters = await _context.Semesters.Where(s => semesterIds.Contains(s.SemesterId)).ToDictionaryAsync(s => s.SemesterId);
            var existingTeachers = await _context.Teachers.Where(t => teacherIds.Contains(t.TeacherId)).ToDictionaryAsync(t => t.TeacherId);

            var relevantAssignments = await _context.HomeroomAssignments
                .Where(ha => classIds.Contains(ha.ClassId) && semesterIds.Contains(ha.SemesterId))
                .ToListAsync();
            var assignmentLookup = relevantAssignments.ToLookup(ha => new { ha.ClassId, ha.SemesterId });


            foreach (var dto in dtos)
            {
                // Kiểm tra xem Class, Semester có tồn tại không
                if (!existingClasses.ContainsKey(dto.ClassId))
                {
                    throw new ArgumentException($"Class with Id {dto.ClassId} does not exist.");
                }
                if (!existingSemesters.ContainsKey(dto.SemesterId))
                {
                    throw new ArgumentException($"Semester with Id {dto.SemesterId} does not exist.");
                }

                var currentAssignments = assignmentLookup[new { dto.ClassId, dto.SemesterId }].ToList();
                var activeAssignment = currentAssignments.FirstOrDefault(a => a.Status == "Hoạt Động");
                var assignmentToUpdate = activeAssignment ?? currentAssignments.FirstOrDefault();

                if (assignmentToUpdate == null) //KHÔNG TÌM THẤY ASSIGNMENT, TẠO MỚI
                {
                    if (dto.TeacherId <= 0)
                    {
                        Console.WriteLine($"Skipping creation for ClassId {dto.ClassId}, SemesterId {dto.SemesterId} due to missing TeacherId.");
                        continue;
                    }

                    if (!existingTeachers.ContainsKey(dto.TeacherId))
                    {
                        throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.");
                    }

                    bool alreadyHasActive = await _context.HomeroomAssignments.AnyAsync(ha => ha.ClassId == dto.ClassId && ha.SemesterId == dto.SemesterId && ha.Status == "Hoạt Động");
                    if (alreadyHasActive)
                    {
                        var className = existingClasses[dto.ClassId].ClassName;
                        var semesterName = existingSemesters[dto.SemesterId].SemesterName;
                        throw new InvalidOperationException($"Class {className} already has an active homeroom teacher in semester {semesterName}. Cannot create a new one.");
                    }
                    // tạo new ass
                    var newAssignment = new HomeroomAssignment
                    {
                        TeacherId = dto.TeacherId,
                        ClassId = dto.ClassId,
                        SemesterId = dto.SemesterId,
                       
                        Status = !string.IsNullOrWhiteSpace(dto.Status) ? dto.Status : "Hoạt Động"
                    };
                    _context.HomeroomAssignments.Add(newAssignment);
                    Console.WriteLine($"Creating new assignment for ClassId {dto.ClassId}, SemesterId {dto.SemesterId}, TeacherId {dto.TeacherId}.");
                }
                else //TÌM THẤY ASSIGNMENT, CẬP NHẬT ===
                {
                    if (dto.TeacherId > 0 && dto.TeacherId != assignmentToUpdate.TeacherId)
                    {
                        if (!existingTeachers.ContainsKey(dto.TeacherId))
                        {
                            throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.");
                        }

                        var hasOtherActiveHomeroom = currentAssignments.Any(ha => ha.HomeroomAssignmentId != assignmentToUpdate.HomeroomAssignmentId && ha.Status == "Hoạt Động");

                        if (hasOtherActiveHomeroom)
                        {
                            var className = existingClasses[dto.ClassId].ClassName;
                            var semesterName = existingSemesters[dto.SemesterId].SemesterName;
                            throw new InvalidOperationException($"Class {className} already has another active homeroom teacher in semester {semesterName}. Cannot assign TeacherId {dto.TeacherId}.");
                        }

                        if (assignmentToUpdate.Status == "Hoạt Động")
                        {
                            assignmentToUpdate.Status = "Không Hoạt Động";
                            _context.HomeroomAssignments.Update(assignmentToUpdate);
                            Console.WriteLine($"Deactivating existing assignment {assignmentToUpdate.HomeroomAssignmentId} for ClassId {dto.ClassId}, SemesterId {dto.SemesterId}.");
                        }
                       
                        var newAssignmentForUpdate = new HomeroomAssignment
                        {
                            TeacherId = dto.TeacherId,
                            ClassId = dto.ClassId,
                            SemesterId = dto.SemesterId,
                            Status = "Hoạt Động"
                        };
                        _context.HomeroomAssignments.Add(newAssignmentForUpdate);
                        Console.WriteLine($"Creating new active assignment for ClassId {dto.ClassId}, SemesterId {dto.SemesterId}, new TeacherId {dto.TeacherId}.");
                    }
                
                    else if (!string.IsNullOrWhiteSpace(dto.Status) && dto.Status != assignmentToUpdate.Status)
                    {
                        if (dto.Status == "Hoạt Động")
                        {
                            var hasOtherActiveHomeroom = currentAssignments.Any(ha => ha.HomeroomAssignmentId != assignmentToUpdate.HomeroomAssignmentId && ha.Status == "Hoạt Động");
                            if (hasOtherActiveHomeroom)
                            {
                                var className = existingClasses[dto.ClassId].ClassName;
                                var semesterName = existingSemesters[dto.SemesterId].SemesterName;
                                throw new InvalidOperationException($"Cannot activate assignment {assignmentToUpdate.HomeroomAssignmentId}. Class {className} already has another active homeroom teacher in semester {semesterName}.");
                            }
                        }

                        assignmentToUpdate.Status = dto.Status;
                        _context.HomeroomAssignments.Update(assignmentToUpdate);
                        Console.WriteLine($"Updating status for assignment {assignmentToUpdate.HomeroomAssignmentId} to '{dto.Status}' for ClassId {dto.ClassId}, SemesterId {dto.SemesterId}.");
                    }
                    else
                    {
                        Console.WriteLine($"No changes needed for assignment {assignmentToUpdate.HomeroomAssignmentId} (ClassId {dto.ClassId}, SemesterId {dto.SemesterId}).");
                    }
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