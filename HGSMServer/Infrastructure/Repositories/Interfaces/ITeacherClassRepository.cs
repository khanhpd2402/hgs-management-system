using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITeacherClassRepository
    {
        Task AssignHomeroomAsync(int teacherId, int classId, int academicYearId, int semesterId);
        Task<bool> IsHomeroomAssignedAsync(int teacherId, int classId, int academicYearId);
        Task<bool> HasHomeroomTeacherAsync(int classId, int academicYearId);
    }
}
