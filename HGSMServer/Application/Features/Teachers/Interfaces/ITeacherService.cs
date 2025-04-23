using Application.Features.Teachers.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Application.Features.Teachers.Interfaces
{
    public interface ITeacherService
    {
        Task<TeacherListResponseDto> GetAllTeachersAsync();
        Task<TeacherDetailDto?> GetTeacherByIdAsync(int id);
        Task AddTeacherAsync(TeacherListDto teacherDto);
        Task UpdateTeacherAsync(int id, TeacherDetailDto teacherDto);
        Task<bool> DeleteTeacherAsync(int id);
        Task<(bool Success, List<string> Errors)> ImportTeachersFromExcelAsync(IFormFile file);
        Task<string> GenerateUniqueUsernameAsync(string fullName);
        Task<string?> GetEmailByTeacherIdAsync(int teacherId);
    }

}