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
        Task UpdateStudentClassAsync(int id, StudentClassAssignmentDto dto);
        Task DeleteStudentClassAsync(int id);
        Task<List<StudentClassResponseDto>> SearchStudentClassesAsync(StudentClassFilterDto filter);
        Task<StudentClassFilterDataDto> GetFilterDataAsync();
    }
}
