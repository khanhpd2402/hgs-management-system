using Application.Features.StudentClass.DTOs;
using Application.Features.Subjects.DTOs;
using Application.Features.Teachers.DTOs;
using System;
using System.Collections.Generic;
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
        Task<List<ClassDto>> GetClassesWithStudentCountAsync(int? academicYearId = null);
        Task<IEnumerable<StudentClassResponseDto>> GetAllStudentClassesAsync();
        Task<StudentClassByLastAcademicYearResponseDto> GetAllStudentClassByLastAcademicYearAsync(int currentAcademicYearId);
        Task<IEnumerable<StudentClassResponseDto>> GetNonEligibleStudentsByLastAcademicYearAsync(int currentAcademicYearId);
        Task<IEnumerable<StudentClassResponseDto>> GetRepeatStudentsByAcademicYearAsync(int academicYearId);
        Task<IEnumerable<SubjectDto>> GetSubjectsByClassIdAsync(int classId, int semesterId);
        Task<TeacherListDto> GetTeacherByClassAndSubjectAsync(int classId, int subjectId, int semesterId);
        Task<HomeroomClassInfoDto> GetHomeroomClassInfoAsync(int teacherId, int semesterId);
    }
}