using Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<IEnumerable<Student>> GetAllWithParentsAsync(int academicYearId);
        Task<int> GetAcademicYearIdAsync(int semesterId);
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByIdWithParentsAsync(int id, int academicYearId);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string idCard);
        Task AddRangeAsync(IEnumerable<Student> students);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<AcademicYear> GetCurrentAcademicYearAsync(DateOnly currentDate);
    }
}