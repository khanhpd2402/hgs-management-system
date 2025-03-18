using Application.Features.Teachers.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Application.Features.Teachers.Interfaces
{
    public interface ITeacherService
    {
        Task<List<TeacherListDto>> GetAllTeachersAsync(bool exportToExcel = false, List<string> selectedColumns = null);
        Task<TeacherDetailDto?> GetTeacherByIdAsync(int id); // Phải có dòng này
        Task AddTeacherAsync(TeacherListDto teacherDto);
        Task UpdateTeacherAsync(int id, TeacherDetailDto teacherDto);
        Task DeleteTeacherAsync(int id);
        Task ImportTeachersFromExcelAsync(IFormFile file);
    }

}