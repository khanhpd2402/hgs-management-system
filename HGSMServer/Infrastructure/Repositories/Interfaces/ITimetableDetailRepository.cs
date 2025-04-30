using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITimetableDetailRepository
    {
        Task AddAsync(TimetableDetail detail);
        Task<bool> IsConflictAsync(int classId, string dayOfWeek, int periodId);
        Task<TimetableDetail> GetByIdAsync(int timetableDetailId);
        Task<TimetableDetail> GetByTeacherAndTimeAsync(int teacherId, string dayOfWeek, int periodId, int timetableId);
        Task SaveChangesAsync();
    }
}
