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
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IParentRepository _parentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IHomeroomAssignmentRepository _homeroomAssignmentRepository;

        public AttendanceUnitOfWork(
            HgsdbContext context,
            IAttendanceRepository attendanceRepo,
            ITeachingAssignmentRepository teachingAssignmentRepository,
            IStudentClassRepository studentClassRepo,
            IParentRepository parentRepo,
            ITeacherRepository teacherRepo,
            IHomeroomAssignmentRepository homeroomAssignmentRepo)
        {
            _context = context;
            _attendanceRepository = attendanceRepo;
            _teachingAssignmentRepository = teachingAssignmentRepository;
            _studentClassRepository = studentClassRepo;
            _parentRepository = parentRepo;
            _teacherRepository = teacherRepo;
            _homeroomAssignmentRepository = homeroomAssignmentRepo; 
        }

        public IAttendanceRepository AttendanceRepository => _attendanceRepository;
        public ITeachingAssignmentRepository TeachingAssignmentRepository => _teachingAssignmentRepository;
        public IStudentClassRepository StudentClassRepository => _studentClassRepository;
        public IParentRepository ParentRepository => _parentRepository;
        public ITeacherRepository TeacherRepository => _teacherRepository;
        public IHomeroomAssignmentRepository HomeroomAssignmentRepository => _homeroomAssignmentRepository; 

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}