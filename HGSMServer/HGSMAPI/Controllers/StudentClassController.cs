using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Features.StudentClass.DTOs;
using Application.Features.StudentClass.Interfaces;
using System.Linq;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentClassesController : ControllerBase
    {
        private readonly IStudentClassService _studentClassService;

        public StudentClassesController(IStudentClassService studentClassService)
        {
            _studentClassService = studentClassService;
        }

        [HttpPost]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> CreateStudentClass([FromBody] StudentClassAssignmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _studentClassService.CreateStudentClassAsync(dto);
                return Ok(new { Message = "Student class assignment created successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the assignment.", Detail = ex.Message });
            }
        }

        
        [HttpPut]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateStudentClasses([FromBody] List<StudentClassAssignmentDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest(new { Message = "The list of class assignments cannot be empty." });
            }

            foreach (var dto in dtos)
            {
                if (dto.StudentId <= 0 || dto.ClassId <= 0 || dto.AcademicYearId <= 0)
                {
                    return BadRequest(new { Message = $"Invalid data in assignment: StudentId, ClassId, and AcademicYearId must be positive. (StudentId: {dto.StudentId}, ClassId: {dto.ClassId}, AcademicYearId: {dto.AcademicYearId})" });
                }
            }

            var duplicateStudentIds = dtos.GroupBy(d => d.StudentId)
                                         .Where(g => g.Count() > 1)
                                         .Select(g => g.Key)
                                         .ToList();
            if (duplicateStudentIds.Any())
            {
                return BadRequest(new { Message = $"Duplicate StudentIds found in the request: {string.Join(", ", duplicateStudentIds)}" });
            }

            try
            {
                await _studentClassService.UpdateStudentClassesAsync(dtos);
                return Ok(new
                {
                    Message = "Student class assignments updated successfully.",
                    UpdatedCount = dtos.Count,
                    Assignments = dtos.Select(d => new { d.StudentId, d.ClassId, d.AcademicYearId })
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message, Detail = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the assignments.", Detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> DeleteStudentClass(int id)
        {
            try
            {
                await _studentClassService.DeleteStudentClassAsync(id);
                return Ok(new { Message = "Student class assignment deleted successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the assignment.", Detail = ex.Message });
            }
        }

        [HttpGet("filter-data")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> GetFilterData([FromQuery] int? classId = null, [FromQuery] int? semesterId = null)
        {
            try
            {
                var filterData = await _studentClassService.GetFilterDataAsync(classId, semesterId);
                return Ok(filterData);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving filter data.", Detail = ex.Message });
            }
        }

        [HttpPost("bulk-transfer")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> BulkTransferClass([FromBody] BulkClassTransferDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _studentClassService.BulkTransferClassAsync(dto);
                return Ok(new
                {
                    Message = "Class transferred successfully.",
                    SourceClassId = dto.ClassId,
                    TargetClassId = dto.TargetClassId,
                    AcademicYearId = dto.AcademicYearId,
                    TargetAcademicYearId = dto.TargetAcademicYearId
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message, Detail = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while transferring the class.", Detail = ex.Message });
            }
        }

        [HttpPost("process-graduation/{academicYearId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> ProcessGraduation(int academicYearId)
        {
            try
            {
                await _studentClassService.ProcessGraduationAsync(academicYearId);
                return Ok(new { Message = "Graduation processed successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Detail = ex.Message });
            }
        }

        [HttpGet("classes-with-student-count")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> GetClassesWithStudentCount([FromQuery] int? academicYearId = null)
        {
            try
            {
                var classes = await _studentClassService.GetClassesWithStudentCountAsync(academicYearId);
                return Ok(classes);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy thông tin lớp.", Detail = ex.Message });
            }
        }
    }
}