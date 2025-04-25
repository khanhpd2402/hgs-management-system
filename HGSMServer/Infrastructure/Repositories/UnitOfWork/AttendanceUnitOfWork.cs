using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.UnitOfWork
{
    public class AttendanceUnitOfWork : IAttendanceUnitOfWork
    {
        private readonly HgsdbContext _context;

        public AttendanceUnitOfWork(
            HgsdbContext context,
            IAttendanceRepository attendanceRepo,
            ITeachingAssignmentRepository teachingAssignmentRepository,
            IStudentClassRepository studentClassRepo,
            IParentRepository parentRepo,
            ITeacherRepository teacherRepo)
        {
            _context = context;
            AttendanceRepository = attendanceRepo;
            TeachingAssignmentRepository = teachingAssignmentRepository;
            StudentClassRepository = studentClassRepo;
            ParentRepository = parentRepo;
            TeacherRepository = teacherRepo;
        }

        public IAttendanceRepository AttendanceRepository { get; }
        public ITeachingAssignmentRepository TeachingAssignmentRepository { get; }
        public IStudentClassRepository StudentClassRepository { get; }
        public IParentRepository ParentRepository { get; }
        public ITeacherRepository TeacherRepository { get; }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
