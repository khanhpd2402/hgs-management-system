using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementtations
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly HgsdbContext _context;

        public LeaveRequestRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest?> GetByIdAsync(int id)
         => await _context.LeaveRequests.FindAsync(id);

        public async Task<IEnumerable<LeaveRequest>> GetAllAsync(int? teacherId = null, string? status = null)
        {
            var query = _context.LeaveRequests.AsQueryable();

            if (teacherId.HasValue)
                query = query.Where(r => r.TeacherId == teacherId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            return await query.ToListAsync();
        }


        public async Task AddAsync(LeaveRequest request)
            => await _context.LeaveRequests.AddAsync(request);

        public void Update(LeaveRequest request) => _context.LeaveRequests.Update(request);

        public void Delete(LeaveRequest request) => _context.LeaveRequests.Remove(request);

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}


