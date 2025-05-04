using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class HomeroomAssignmentRepository : IHomeroomAssignmentRepository
    {
        private readonly HgsdbContext _context;

        public HomeroomAssignmentRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<HomeroomAssignment> GetByTeacherAndSemesterAsync(int teacherId, int semesterId)
        {
            return await _context.HomeroomAssignments
                .Where(ha => ha.TeacherId == teacherId && ha.SemesterId == semesterId && ha.Status == "Hoạt Động")
                .Include(ha => ha.Class)
                .FirstOrDefaultAsync();
        }
    }
}