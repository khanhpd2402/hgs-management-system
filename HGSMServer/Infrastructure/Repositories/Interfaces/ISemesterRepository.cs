using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ISemesterRepository
    {
        Task<List<Semester>> GetByAcademicYearIdAsync(int academicYearId);
        Task<Semester?> GetByIdAsync(int id);
        Task AddAsync(Semester semester);
        Task UpdateAsync(Semester semester);
        Task DeleteAsync(int id);
    }

}
