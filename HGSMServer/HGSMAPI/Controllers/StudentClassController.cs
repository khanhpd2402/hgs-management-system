using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Features.StudentClass.DTOs;
using Application.Features.StudentClass.Interfaces;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentClassController : ControllerBase
    {
        private readonly IStudentClassService _studentClassService;

        public StudentClassController(IStudentClassService studentClassService)
        {
            _studentClassService = studentClassService;
        }

        [HttpGet]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư,Giáo viên,Phụ huynh")]
        public async Task<ActionResult<IEnumerable<StudentClassResponseDto>>> GetAllStudentClasses()
        {
            try
            {
                var studentClasses = await _studentClassService.GetAllStudentClassesAsync();
                return Ok(studentClasses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi lấy danh sách phân công lớp." });
            }
        }

        [HttpGet("last-academic-year")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư,Giáo viên,Phụ huynh")]
        public async Task<ActionResult<IEnumerable<StudentClassResponseDto>>> GetAllStudentClassByLastAcademicYear([FromQuery] int currentAcademicYearId)
        {
            try
            {
                if (currentAcademicYearId <= 0)
                {
                    return BadRequest(new { message = "AcademicYearId phải là một số nguyên dương." });
                }

                var studentClasses = await _studentClassService.GetAllStudentClassByLastAcademicYearAsync(currentAcademicYearId);
                return Ok(studentClasses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi lấy danh sách phân công lớp năm học vừa kết thúc." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> CreateStudentClass([FromBody] StudentClassAssignmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ." });
            }

            try
            {
                await _studentClassService.CreateStudentClassAsync(dto);
                return Ok(new { message = "Phân công lớp thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi tạo phân công lớp." });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateStudentClasses([FromBody] List<StudentClassAssignmentDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest(new { message = "Danh sách phân công lớp không được rỗng." });
            }

            try
            {
                await _studentClassService.UpdateStudentClassesAsync(dtos);
                return Ok(new
                {
                    message = "Cập nhật phân công lớp thành công.",
                    UpdatedCount = dtos.Count,
                    Assignments = dtos.Select(d => new { d.StudentId, d.ClassId, d.AcademicYearId })
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi cập nhật phân công lớp." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> DeleteStudentClass(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID phân công lớp phải là một số nguyên dương." });
                }

                await _studentClassService.DeleteStudentClassAsync(id);
                return Ok(new { message = "Xóa phân công lớp thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi xóa phân công lớp." });
            }
        }

        [HttpGet("filter-data")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư,Giáo viên,Phụ huynh,Trưởng bộ môn")]
        public async Task<IActionResult> GetFilterData([FromQuery] int? classId = null, [FromQuery] int? semesterId = null)
        {
            try
            {
                var filterData = await _studentClassService.GetFilterDataAsync(classId, semesterId);
                return Ok(filterData);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi lấy dữ liệu lọc." });
            }
        }

        [HttpPost("bulk-transfer")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<ActionResult<List<BulkTransferResultDto>>> BulkTransferClass([FromBody] List<BulkClassTransferDto> dtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ." });
            }

            try
            {
                if (dtos == null || !dtos.Any())
                {
                    return BadRequest(new { message = "Danh sách chuyển lớp không được rỗng." });
                }

                var results = new List<BulkTransferResultDto>();
                foreach (var dto in dtos)
                {
                    var result = await _studentClassService.BulkTransferClassAsync(dto);
                    results.Add(result);
                }
                return Ok(results);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình chuyển lớp hàng loạt." });
            }
        }

        [HttpPost("process-graduation/{academicYearId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> ProcessGraduation(int academicYearId)
        {
            try
            {
                if (academicYearId <= 0)
                {
                    return BadRequest(new { message = "AcademicYearId phải là một số nguyên dương." });
                }

                await _studentClassService.ProcessGraduationAsync(academicYearId);
                return Ok(new { message = "Xử lý tốt nghiệp thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình xử lý tốt nghiệp." });
            }
        }

        [HttpGet("classes-with-student-count")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư,Giáo viên,Phụ huynh")]
        public async Task<IActionResult> GetClassesWithStudentCount([FromQuery] int? academicYearId = null)
        {
            try
            {
                if (academicYearId.HasValue && academicYearId <= 0)
                {
                    return BadRequest(new { message = "AcademicYearId phải là một số nguyên dương." });
                }

                var classes = await _studentClassService.GetClassesWithStudentCountAsync(academicYearId);
                return Ok(classes);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi lấy thông tin lớp." });
            }
        }
    }
}