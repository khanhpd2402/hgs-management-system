using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        IQueryable<Teacher> GetAll();
        Task<Teacher?> GetByIdAsync(int id);
        Task AddAsync(Teacher teacher);
        Task UpdateAsync(Teacher teacher);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string idCard);
        Task AddRangeAsync(IEnumerable<Teacher> teachers);
        Task<Teacher> GetByUserIdAsync(int userId);
        Task<List<TeacherSubject>?> GetTeacherSubjectsAsync(int teacherId);
        Task DeleteTeacherSubjectsAsync(int teacherId);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsEmailOrPhoneExistsAsync(string email, string phoneNumber);
        Task AddTeacherSubjectAsync(TeacherSubject teacherSubject);
        Task AddTeacherSubjectsRangeAsync(IEnumerable<TeacherSubject> teacherSubjects);
        Task<Teacher?> GetByIdWithUserAsync(int id);
        Task<IEnumerable<Teacher>> GetAllWithUserAsync();
        Task UpdateTeacherSubjectAsync(TeacherSubject teacherSubject);
        Task DeleteTeacherSubjectsRangeAsync(IEnumerable<TeacherSubject> teacherSubjects);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int userId);

    }
}