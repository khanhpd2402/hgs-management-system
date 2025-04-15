using Application.Features.StudentClass.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.StudentClass.Interfaces
{
    public interface IStudentClassService
    {
        Task CreateStudentClassAsync(StudentClassAssignmentDto dto);
        Task UpdateStudentClassesAsync(List<StudentClassAssignmentDto> dtos);
        Task DeleteStudentClassAsync(int id);
        Task<StudentClassFilterDataDto> GetFilterDataAsync(int? classId = null, int? semesterId = null);
        Task BulkTransferClassAsync(BulkClassTransferDto dto);
        Task ProcessGraduationAsync(int academicYearId);
        Task<List<ClassDto>> GetClassesWithStudentCountAsync(int? academicYearId = null);
    }
}
