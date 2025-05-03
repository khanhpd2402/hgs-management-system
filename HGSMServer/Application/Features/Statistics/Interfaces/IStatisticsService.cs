using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Statistics.Interfaces
{
    public interface IStatisticsService
    {
        Task<object> GetSchoolStatisticsAsync();
    }
}
