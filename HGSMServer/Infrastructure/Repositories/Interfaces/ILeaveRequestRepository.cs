using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ILeaveRequestRepository
    {
        Task<LeaveRequest?> GetByIdAsync(int id);
        Task<IEnumerable<LeaveRequest>> GetAllAsync(int? teacherId = null, string? status = null);
        Task AddAsync(LeaveRequest request);
        void Update(LeaveRequest request);
        void Delete(LeaveRequest request);
        Task SaveAsync();
    }

}
