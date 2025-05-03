using System;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories.UnitOfWork
{
    public class TimetableUnitOfWork : ITimetableUnitOfWork
    {
        private readonly HgsdbContext _context;

        // Inject các Repository Interfaces trực tiếp
        public TimetableUnitOfWork(
            HgsdbContext context,
            ITimetableRepository timetables,
            ITimetableDetailRepository timetableDetails,
            IPeriodRepository periods,
            ISubjectRepository subjects,
            ITeacherRepository teachers,
            IClassRepository classes,
            ITeachingAssignmentRepository teachingAssignments,
            ISemesterRepository semesters)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            // Gán các repository được inject vào properties
            Timetables = timetables ?? throw new ArgumentNullException(nameof(timetables));
            TimetableDetails = timetableDetails ?? throw new ArgumentNullException(nameof(timetableDetails));
            Periods = periods ?? throw new ArgumentNullException(nameof(periods));
            Subjects = subjects ?? throw new ArgumentNullException(nameof(subjects));
            Teachers = teachers ?? throw new ArgumentNullException(nameof(teachers));
            Classes = classes ?? throw new ArgumentNullException(nameof(classes));
            TeachingAssignments = teachingAssignments ?? throw new ArgumentNullException(nameof(teachingAssignments));
            Semesters = semesters ?? throw new ArgumentNullException(nameof(semesters));
        }

        // Properties public để service truy cập
        public ITimetableRepository Timetables { get; }
        public ITimetableDetailRepository TimetableDetails { get; }
        public IPeriodRepository Periods { get; }
        public ISubjectRepository Subjects { get; }
        public ITeacherRepository Teachers { get; }
        public IClassRepository Classes { get; }
        public ITeachingAssignmentRepository TeachingAssignments { get; }
        public ISemesterRepository Semesters { get; }

        public async Task SaveChangesAsync()
        {
            // Lưu thay đổi thông qua DbContext được inject
            await _context.SaveChangesAsync();
        }

        // Implement IDisposable
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}