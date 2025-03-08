using Application.Features.Teachers.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Application.Features.Teachers.Interfaces
{
    public interface ITeacherService
    {

        Task<IEnumerable<TeacherDetailDto>> GetAllTeachersAsync();
        Task<TeacherDetailDto?> GetTeacherByIdAsync(int id);
        Task AddTeacherAsync(TeacherDetailDto teacherDto);
        Task UpdateTeacherAsync(int id, TeacherDetailDto teacherDto);
        Task DeleteTeacherAsync(int id);
        Task<byte[]> ExportTeachersToExcelAsync(); // API xuất full
        Task<byte[]> ExportTeachersSelectedToExcelAsync(List<string> selectedColumns);
        Task ImportTeachersFromExcelAsync(IFormFile file);
    }

}