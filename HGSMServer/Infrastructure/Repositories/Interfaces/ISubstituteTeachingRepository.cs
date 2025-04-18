using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ISubstituteTeachingRepository
    {
        Task<SubstituteTeaching> CreateAsync(SubstituteTeaching entity);
        Task<SubstituteTeaching> GetByIdAsync(int substituteId);
        Task<IEnumerable<SubstituteTeaching>> GetAllAsync(int? timetableDetailId = null, int? teacherId = null, DateOnly? date = null);
        Task UpdateAsync(SubstituteTeaching entity);
        Task DeleteAsync(int substituteId);
    }

}
