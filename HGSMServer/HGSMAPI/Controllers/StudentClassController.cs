using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Features.StudentClass.DTOs;
using Application.Features.StudentClass.Interfaces;
using System.Linq;
using System;

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
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách phân công lớp.", Detail = ex.Message });
            }
        }

        [HttpGet("last-academic-year")] 
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư,Giáo viên,Phụ huynh")]
        public async Task<ActionResult<IEnumerable<StudentClassResponseDto>>> GetAllStudentClassByLastAcademicYear([FromQuery] int currentAcademicYearId) // Sử dụng [FromQuery]
        {
            try
            {
                var studentClasses = await _studentClassService.GetAllStudentClassByLastAcademicYearAsync(currentAcademicYearId);

                if (!studentClasses.Any())
                {
                    // Có thể trả về NotFound nếu không tìm thấy năm học hoặc không có học sinh trong năm đó
                    return NotFound($"Không tìm thấy bản ghi phân công lớp cho năm học vừa kết thúc (trước năm học có Id {currentAcademicYearId}).");
                }

                return Ok(studentClasses);
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
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách phân công lớp năm học vừa kết thúc.", Detail = ex.Message });
            }
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
                return Ok(new { Message = "Phân công lớp thành công." });
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
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi tạo phân công lớp.", Detail = ex.Message });
            }
        }

        [HttpPut] 
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateStudentClasses([FromBody] List<StudentClassAssignmentDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest(new { Message = "Danh sách phân công lớp không được rỗng." });
            }


            try
            {
                await _studentClassService.UpdateStudentClassesAsync(dtos);
                return Ok(new
                {
                    Message = "Cập nhật phân công lớp thành công.",
                    UpdatedCount = dtos.Count,
                    Assignments = dtos.Select(d => new { d.StudentId, d.ClassId, d.AcademicYearId }) // Trả về thông tin các assignment đã cập nhật
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
                return Conflict(new { Message = ex.Message, Detail = ex.InnerException?.Message }); // Giữ lại InnerException detail nếu có
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật phân công lớp.", Detail = ex.Message });
            }
        }

        
        [HttpDelete("{id}")] 
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> DeleteStudentClass(int id)
        {
            try
            {
                await _studentClassService.DeleteStudentClassAsync(id);
                return Ok(new { Message = "Phân công lớp đã được xóa thành công." });
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
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xóa phân công lớp.", Detail = ex.Message });
            }
        }

      
        [HttpGet("filter-data")] 
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư,Giáo viên,Phụ huynh")] 
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
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy dữ liệu lọc.", Detail = ex.Message });
            }
        }

        [HttpPost("bulk-transfer")] 
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")] 
        public async Task<ActionResult<BulkTransferResultDto>> BulkTransferClass([FromBody] BulkClassTransferDto dto) // Kiểu trả về là ActionResult<BulkTransferResultDto>
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _studentClassService.BulkTransferClassAsync(dto); 
                return Ok(result);
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
                return StatusCode(500, new { Message = "Đã xảy ra lỗi trong quá trình chuyển lớp hàng loạt.", Detail = ex.Message });
            }
        }

        [HttpPost("process-graduation/{academicYearId}")] 
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")] 
        public async Task<IActionResult> ProcessGraduation(int academicYearId)
        {
            try
            {
                await _studentClassService.ProcessGraduationAsync(academicYearId);
                return Ok(new { Message = "Xử lý tốt nghiệp thành công." });
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
                return StatusCode(500, new { Message = "Đã xảy ra lỗi.", Detail = ex.Message });
            }
        }

        [HttpGet("classes-with-student-count")] 
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư,Giáo viên,Phụ huynh")] 
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
