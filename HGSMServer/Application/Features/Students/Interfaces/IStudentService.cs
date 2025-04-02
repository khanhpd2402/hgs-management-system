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
        Task<StudentListResponseDto> GetAllStudentsWithParentsAsync(int academicYearId);
        Task<StudentDto?> GetStudentByIdAsync(int id, int academicYearId);
        Task<int> AddStudentAsync(CreateStudentDto createStudentDto);
        Task UpdateStudentAsync(UpdateStudentDto updateStudentDto);
        Task DeleteStudentAsync(int id);
        //Task<byte[]> ExportStudentsFullToExcelAsync();
        //Task<byte[]> ExportStudentsSelectedToExcelAsync(List<string> selectedColumns);

        Task<List<string>> ImportStudentsFromExcelAsync(IFormFile file);
    }
}