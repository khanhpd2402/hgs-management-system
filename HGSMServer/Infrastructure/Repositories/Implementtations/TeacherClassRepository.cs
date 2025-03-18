using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class TeacherClassRepository : ITeacherClassRepository
    {
        private readonly HgsdbContext _context;

        public TeacherClassRepository(HgsdbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AssignHomeroomAsync(int teacherId, int classId, int academicYearId, int semesterId)
        {
            // Kiểm tra xem lớp đã có giáo viên chủ nhiệm trong năm học này chưa
            var hasHomeroomTeacher = await HasHomeroomTeacherAsync(classId, academicYearId);
            if (hasHomeroomTeacher)
            {
                throw new InvalidOperationException($"Class with Id {classId} already has a homeroom teacher in academic year {academicYearId}.");
            }

            // Kiểm tra xem bản ghi đã tồn tại chưa
            var existingAssignment = await _context.TeacherClasses
                .FirstOrDefaultAsync(tc => tc.TeacherId == teacherId && tc.ClassId == classId && tc.AcademicYearId == academicYearId);

            if (existingAssignment != null)
            {
                existingAssignment.IsHomeroomTeacher = true;
            }
            else
            {
                var teacherClass = new TeacherClass
                {
                    TeacherId = teacherId,
                    ClassId = classId,
                    IsHomeroomTeacher = true,
                    AcademicYearId = academicYearId
                };
                await _context.TeacherClasses.AddAsync(teacherClass);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsHomeroomAssignedAsync(int teacherId, int classId, int academicYearId)
        {
            return await _context.TeacherClasses
                .AnyAsync(tc => tc.TeacherId == teacherId && tc.ClassId == classId && tc.AcademicYearId == academicYearId && tc.IsHomeroomTeacher == true);
        }

        public async Task<bool> HasHomeroomTeacherAsync(int classId, int academicYearId)
        {
            return await _context.TeacherClasses
                .AnyAsync(tc => tc.ClassId == classId && tc.AcademicYearId == academicYearId && tc.IsHomeroomTeacher == true);
        }
    }
}