using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITeacherSubjectRepository
    {
        Task<List<TeacherSubject>> GetAllAsync();
        Task<TeacherSubject> GetByIdAsync(int id);
        Task<List<TeacherSubject>> GetByTeacherIdAsync(int teacherId);
        Task AddAsync(TeacherSubject teacherSubject);
        Task UpdateAsync(TeacherSubject teacherSubject);
        Task DeleteAsync(int id);
        Task DeleteByTeacherIdAsync(int teacherId);
    }
}