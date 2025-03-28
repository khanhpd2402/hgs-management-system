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
        Task<List<LeaveRequest>> GetAllAsync();
        Task<LeaveRequest?> GetByIdAsync(int id);
        Task<List<LeaveRequest>> GetByTeacherIdAsync(int teacherId);
        Task AddAsync(LeaveRequest leaveRequest);
        Task UpdateAsync(LeaveRequest leaveRequest);
        Task DeleteAsync(int id);
    }

}
