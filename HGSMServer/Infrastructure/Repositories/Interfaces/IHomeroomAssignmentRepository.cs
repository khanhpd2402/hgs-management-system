using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IHomeroomAssignmentRepository
    {
        Task<HomeroomAssignment> GetByTeacherAndSemesterAsync(int teacherId, int semesterId);
    }
}
