using Application.Features.Teachers.DTOs;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Features.Teachers.Interfaces
{
    public interface ITeacherService
    {
        Task<TeacherListResponseDto> GetAllTeachersAsync(bool exportToExcel = false, List<string> selectedColumns = null);
        Task<TeacherDetailDto?> GetTeacherByIdAsync(int id);
        Task AddTeacherAsync(TeacherListDto teacherDto);
        Task UpdateTeacherAsync(int id, TeacherDetailDto teacherDto);
        Task DeleteTeacherAsync(int id);
        Task ImportTeachersFromExcelAsync(IFormFile file);
        Task AssignHomeroomAsync(AssignHomeroomDto assignHomeroomDto);
        Task<bool> IsHomeroomAssignedAsync(int teacherId, int classId, int academicYearId);
        Task<bool> HasHomeroomTeacherAsync(int classId, int academicYearId);
    }
}