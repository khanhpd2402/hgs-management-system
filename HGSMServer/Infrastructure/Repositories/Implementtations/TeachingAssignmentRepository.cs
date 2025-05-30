﻿using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementtations
{
    public class TeachingAssignmentRepository : ITeachingAssignmentRepository
    {
        private readonly HgsdbContext _context;

        public TeachingAssignmentRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<TeachingAssignment> GetAssignmentByClassSubjectTeacherAsync(int classId, int subjectId, int semesterId)
        {
            return await _context.TeachingAssignments
                .Include(ta => ta.Teacher)
                    .ThenInclude(t => t.User) 
                .Include(ta => ta.Subject) 
                .Where(ta => ta.ClassId == classId && ta.SubjectId == subjectId && ta.SemesterId == semesterId)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<TeachingAssignment>> GetBySemesterIdAsync(int semesterId)
        {
            return await _context.TeachingAssignments
        .Where(t => t.SemesterId == semesterId)
        .Include(t => t.Class)
        .Include(t => t.Teacher)
        .Include(ta => ta.Subject)
        .ToListAsync();
        }
        public async Task<bool> IsTeacherAssignedAsync(int teacherId, int classId, int semesterId)
        {
            return await _context.TeachingAssignments.AnyAsync(t =>
                t.TeacherId == teacherId &&
                t.ClassId == classId &&
                t.SemesterId == semesterId);
        }
        public async Task<TeachingAssignment> GetByIdAsync(int assignmentId)
        {
            return await _context.TeachingAssignments
                .Include(ta => ta.Subject)
                .FirstOrDefaultAsync(ta => ta.AssignmentId == assignmentId);
        }
        public async Task<List<TeachingAssignment>> GetAssignmentsByClassAndSemesterAsync(int classId, int semesterId)
        {
            return await _context.TeachingAssignments
                .Include(ta => ta.Subject) 
                .Where(ta => ta.ClassId == classId && ta.SemesterId == semesterId && ta.Subject != null)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
