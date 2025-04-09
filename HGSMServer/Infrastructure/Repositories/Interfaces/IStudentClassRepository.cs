using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IStudentClassRepository
    {
        Task<IEnumerable<StudentClass>> GetByClassIdAsync(int classId);
        Task<StudentClass?> GetByIdAsync(int id);
        Task<IEnumerable<StudentClass>> GetAllAsync();
        Task<StudentClass> AddAsync(StudentClass entity);
        Task<StudentClass> UpdateAsync(StudentClass entity);
        Task DeleteAsync(int id);
        Task<StudentClass> GetStudentClassByStudentAndClassIdAsync(int studentId, int classId);
    }
}
