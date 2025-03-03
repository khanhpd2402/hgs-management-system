using Application.Features.Students.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDTO>> GetAllStudentsAsync();
        Task<StudentDTO?> GetStudentByIdAsync(int id);
        Task AddStudentAsync(StudentDTO studentDto);
        Task UpdateStudentAsync(StudentDTO studentDto);
        Task DeleteStudentAsync(int id);
    }
}
