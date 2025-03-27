using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITimetableRepository
    {
        Task<List<Timetable>> GetTimetableByClassAsync(int classId, DateOnly effectiveDate);
        Task<Timetable?> GetTimetableByIdAsync(int timetableId);
        Task<Timetable> AddTimetableAsync(Timetable timetable);
        Task<bool> UpdateTimetableAsync(Timetable timetable);
        Task<bool> DeleteTimetableAsync(int timetableId);
    }
}
