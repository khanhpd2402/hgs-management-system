using System;
using System.Threading.Tasks;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories.UnitOfWork
{
    public interface ITimetableUnitOfWork : IDisposable
    {
        ITimetableRepository Timetables { get; }
        ITimetableDetailRepository TimetableDetails { get; }
        IPeriodRepository Periods { get; }
        ISubjectRepository Subjects { get; }
        ITeacherRepository Teachers { get; }
        IClassRepository Classes { get; }
        ITeachingAssignmentRepository TeachingAssignments { get; }
        ISemesterRepository Semesters { get; }

        Task SaveChangesAsync();
    }
}