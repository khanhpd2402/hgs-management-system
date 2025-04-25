using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.UnitOfWork
{
    public interface IAttendanceUnitOfWork
    {
        IAttendanceRepository AttendanceRepository { get; }
        ITeachingAssignmentRepository TeachingAssignmentRepository { get; }
        IStudentClassRepository StudentClassRepository { get; }
        IParentRepository ParentRepository { get; }
        ITeacherRepository TeacherRepository { get; }
        Task SaveChangesAsync();
    }
}
