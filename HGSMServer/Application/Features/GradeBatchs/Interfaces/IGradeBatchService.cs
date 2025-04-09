using Application.Features.GradeBatchs.DTOs;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeBatchs.Interfaces
{
    public interface IGradeBatchService
    {
        Task<GradeBatchDto?> GetByIdAsync(int id);
        Task<IEnumerable<GradeBatchDto>> GetByAcademicYearIdAsync(int academicYearId);
        Task<int> CreateBatchAndInsertGradesAsync(string batchName, int semesterId, DateOnly start, DateOnly end, string status);
        Task<UpdateGradeBatchDto?> UpdateAsync(int id, UpdateGradeBatchDto dto);
    }
}
