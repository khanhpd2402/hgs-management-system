﻿using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Repositories.Implementtations
{
    public class GradeRepository : IGradeRepository
    {
        private readonly HgsdbContext _context;

        public GradeRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Grade> grades)
        {
            await _context.Grades.AddRangeAsync(grades);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Grade>> GetGradesByStudentAsync(int studentId, int semesterId)
        {
            return await (from g in _context.Grades
                          join gb in _context.GradeBatches on g.BatchId equals gb.BatchId
                          join sem in _context.Semesters on gb.SemesterId equals sem.SemesterId
                          where g.StudentClass.StudentId == studentId
                                && gb.SemesterId == semesterId
                          select g)
                          .Include(g => g.StudentClass.Student) 
                          .Include(g => g.Assignment.Subject)
                          .Include(g => g.Batch).ThenInclude(b => b.Semester)
                          .ToListAsync();
        }
        public async Task<List<Grade>> GetGradesByClassAsync(int classId, int subjectId, int semesterId)
        {
            return await (from g in _context.Grades
                          join gb in _context.GradeBatches on g.BatchId equals gb.BatchId
                          join sem in _context.Semesters on gb.SemesterId equals sem.SemesterId
                          join ta in _context.TeachingAssignments on g.AssignmentId equals ta.AssignmentId
                          where g.StudentClass.ClassId == classId
                                && ta.SubjectId == subjectId
                                && gb.SemesterId == semesterId
                          select g)
                          .Include(g => g.StudentClass.Student)
                          .Include(g => g.Batch)
                          .ToListAsync();
        }
        public async Task<List<Grade>> GetGradesByTeacherAsync(int teacherId, int classId, int subjectId, int semesterId)
        {
            return await (from g in _context.Grades
                          join gb in _context.GradeBatches on g.BatchId equals gb.BatchId
                          join sem in _context.Semesters on gb.SemesterId equals sem.SemesterId
                          join ta in _context.TeachingAssignments on g.AssignmentId equals ta.AssignmentId
                          where ta.TeacherId == teacherId 
                                && g.StudentClass.ClassId == classId
                                && ta.SubjectId == subjectId
                                && gb.SemesterId == semesterId
                          select g)
                          .Include(g => g.StudentClass.Student)
                          .Include(g => g.Batch)
                          .ToListAsync();
        }

        public async Task<bool> UpdateMultipleGradesAsync(List<Grade> grades)
        {
            if (grades == null || grades.Count == 0) return false;

            _context.Grades.UpdateRange(grades);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Grade>> GetGradesByIdsAsync(List<int> gradeIds)
        {
            return await _context.Grades
                .Where(g => gradeIds.Contains(g.GradeId)).Include(g => g.Batch)
                .ToListAsync();
        }
        public async Task<IEnumerable<Grade>> GetByBatchIdAsync(int batchId)
        {
            return await _context.Grades
                .Where(g => g.BatchId == batchId).Include(g => g.Batch)
                .ToListAsync();
        }
        public async Task DeleteRangeAsync(IEnumerable<Grade> grades)
        {
            _context.Grades.RemoveRange(grades);
        }
        public async Task<IEnumerable<Grade>> GetByStudentClassIdsAsync(IEnumerable<int> studentClassIds)
        {
            var studentClassIdsList = studentClassIds.ToList();
            if (!studentClassIdsList.Any())
            {
                return new List<Grade>();
            }

            return await _context.Grades
                .Where(g => g.StudentClassId.HasValue && studentClassIdsList.Contains(g.StudentClassId.Value))
                .Include(g => g.StudentClass)
                .Include(g => g.Batch)
                .ToListAsync();
        }
        public async Task<Grade?> GetGradeAsync(int studentClassId, int assignmentId, int batchId, string assessmentsTypeName)
        {
            return await _context.Grades
                .FirstOrDefaultAsync(g =>
                    g.StudentClassId == studentClassId &&
                    g.AssignmentId == assignmentId &&
                    g.BatchId == batchId &&
                    g.AssessmentsTypeName == assessmentsTypeName);
        }
        public async Task<List<Grade>> GetGradesForStudentSubjectSemesterAsync(
           int studentClassId,
           int assignmentId,
           int semesterId)
        {
            return await _context.Grades
                .Include(g => g.Batch) // Cần Batch để lọc theo SemesterId
                .Where(g => g.StudentClassId == studentClassId &&
                             g.AssignmentId == assignmentId &&
                             g.Batch != null && // Đảm bảo Batch không null
                             g.Batch.SemesterId == semesterId)
                .AsNoTracking() // Sử dụng AsNoTracking nếu dữ liệu chỉ để đọc
                .ToListAsync();
        }
    }
}
