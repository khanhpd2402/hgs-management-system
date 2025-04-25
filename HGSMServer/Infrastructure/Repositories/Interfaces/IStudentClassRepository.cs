using Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IStudentClassRepository
    {
        Task<IEnumerable<StudentClass>> GetByClassIdAndAcademicYearAsync(int classId, int academicYearId);
        Task<StudentClass?> GetWithClassAndStudentAsync(int studentClassId);
        Task<StudentClass?> GetByIdAsync(int id);
        Task<IEnumerable<StudentClass>> GetAllAsync();
        Task<StudentClass> AddAsync(StudentClass entity);
        Task<StudentClass> UpdateAsync(StudentClass entity);
        Task DeleteAsync(int id);
        Task<StudentClass> GetStudentClassByStudentAndClassIdAsync(int studentId, int classId);
        Task<StudentClass> GetByStudentAndAcademicYearAsync(int studentId, int academicYearId);
        Task<IEnumerable<StudentClass>> SearchAsync(int? studentId, int? classId, int? academicYearId, string studentName);
        Task AddRangeAsync(IEnumerable<StudentClass> entities);
        Task UpdateRangeAsync(List<StudentClass> assignments);
        Task DeleteRangeAsync(List<int> ids);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<List<StudentClass>> GetByGradeLevelAndAcademicYearAsync(int gradeLevelId, int academicYearId);
        //history of class assignment
        Task<List<StudentClass>> GetByStudentIdAsync(int studentId);
    }
}
