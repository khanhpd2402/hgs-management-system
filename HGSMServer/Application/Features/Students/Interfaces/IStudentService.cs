using Application.Features.Students.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.Interfaces
{
    public interface IStudentService
    {
        IQueryable<StudentDto> GetAllStudents();
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task AddStudentAsync(StudentDto studentDto);
        Task UpdateStudentAsync(StudentDto studentDto);
        Task DeleteStudentAsync(int id);
        Task<byte[]> ExportStudentsFullToExcelAsync();
        Task<byte[]> ExportStudentsSelectedToExcelAsync(List<string> selectedColumns);

        Task ImportStudentsFromExcelAsync(IFormFile file);
    }
}