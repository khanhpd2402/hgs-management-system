using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IConductRepository
    {
        Task<IEnumerable<Conduct>> GetAllAsync();
        Task<Conduct> GetByIdAsync(int id);
        Task<Conduct> CreateAsync(Conduct conduct);
        Task<bool> UpdateAsync(int id, Conduct conduct);
        Task<bool> DeleteAsync(int id);
    }

}
