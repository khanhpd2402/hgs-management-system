using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Features.StudentClass.DTOs;
using Application.Features.StudentClass.Interfaces;

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

        /// <summary>
        /// Cập nhật phân lớp hiện tại
        /// </summary>
        /// <param name="id">ID của phân lớp</param>
        /// <param name="dto">Thông tin phân lớp mới</param>
        /// <returns>Thông báo thành công hoặc lỗi</returns>
        [HttpPut]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateStudentClasses([FromBody] List<StudentClassAssignmentDto> dtos)
        {
            if (!ModelState.IsValid || dtos == null || !dtos.Any())
            {
                return BadRequest(new { Message = "Invalid or empty request data." });
            }

            try
            {
                await _studentClassService.UpdateStudentClassesAsync(dtos);
                return Ok(new { Message = "Student class assignments updated successfully." });
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
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the assignments.", Detail = ex.Message });
            }
        }

        /// <summary>
        /// Xóa phân lớp
        /// </summary>
        /// <param name="id">ID của phân lớp</param>
        /// <returns>Thông báo thành công hoặc lỗi</returns>
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

        /// <summary>
        /// Tìm kiếm phân lớp theo bộ lọc
        /// </summary>
        /// <param name="filter">Bộ lọc tìm kiếm</param>
        /// <returns>Danh sách phân lớp phù hợp</returns>


        /// <summary>
        /// Lấy dữ liệu lọc (danh sách học sinh, lớp, năm học)
        /// </summary>
        /// <returns>Danh sách dữ liệu để lọc</returns>
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
                return Ok(new { Message = "Class transferred successfully." });
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
        /// <summary>
        /// Lấy thông tin tất cả các lớp cùng số lượng học sinh
        /// </summary>
        /// <param name="academicYearId">ID của năm học (tùy chọn)</param>
        /// <returns>Danh sách các lớp với thông tin và số học sinh</returns>
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