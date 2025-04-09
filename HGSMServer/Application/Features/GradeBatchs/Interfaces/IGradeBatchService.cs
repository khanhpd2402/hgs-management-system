using Application.Features.GradeBatchs.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeBatchs.Interfaces
{
    public interface IGradeBatchService
    {
        Task<int> CreateBatchAndInsertGradesAsync(string batchName, int semesterId, DateOnly start, DateOnly end, string status);
    }
}
