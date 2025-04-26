using Application.Features.StudentClass.DTOs;
using Application.Features.Students.DTOs;
using Application.Features.Classes.DTOs;
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
        Task<BulkTransferResultDto> BulkTransferClassAsync(BulkClassTransferDto dto);
        Task ProcessGraduationAsync(int academicYearId);
        Task<List<StudentClass.DTOs.ClassDto>> GetClassesWithStudentCountAsync(int? academicYearId = null);
        Task<IEnumerable<StudentClassResponseDto>> GetAllStudentClassesAsync();
        Task<IEnumerable<StudentClassResponseDto>> GetAllStudentClassByLastAcademicYearAsync(int currentAcademicYearId);
    }
}
