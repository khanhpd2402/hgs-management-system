using Application.Features.Semesters.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Semesters.Interfaces
{
    public interface ISemesterService
    {
        Task<List<SemesterDto>> GetByAcademicYearIdAsync(int academicYearId);
        Task<SemesterDto?> GetByIdAsync(int id);
        Task AddAsync(CreateSemesterDto semester);
        Task UpdateAsync(SemesterDto semester);
        Task DeleteAsync(int id);
    }

}
