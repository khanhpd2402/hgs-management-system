using Application.Features.Teachers.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Teachers.Interfaces
{
    public interface ITeacherService
    {
        Task<IEnumerable<TeacherDetailDto>> GetAllTeachersAsync();
        Task<TeacherDetailDto?> GetTeacherByIdAsync(int id);
        Task AddTeacherAsync(TeacherDetailDto teacherDto);
        Task UpdateTeacherAsync(int id, TeacherDetailDto teacherDto);
        Task DeleteTeacherAsync(int id);
        Task<byte[]> ExportTeachersToExcelAsync();
        Task ImportTeachersFromExcelAsync(IFormFile file);
    }
}
