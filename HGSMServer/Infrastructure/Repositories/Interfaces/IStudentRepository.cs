using Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllWithParentsAsync(int academicYearId);
        Task<int> GetAcademicYearIdAsync(int semesterId);
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByIdWithParentsAsync(int id, int academicYearId);
        //Task<List<Student>> GetStudentsByIdsAsync(List<int> studentIds);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string idCard); // Kiểm tra xem học sinh có tồn tại không
        Task AddRangeAsync(IEnumerable<Student> students); // Thêm danh sách học sinh
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
