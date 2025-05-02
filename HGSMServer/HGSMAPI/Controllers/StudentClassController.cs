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
                Console.WriteLine("Fetching all student classes...");
                var studentClasses = await _studentClassService.GetAllStudentClassesAsync();
                return Ok(studentClasses);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching student classes: {ex.Message}");
                return BadRequest("Lỗi khi lấy danh sách phân công lớp.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching student classes: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách phân công lớp.");
            }
        }

        [HttpGet("last-academic-year")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư,Giáo viên,Phụ huynh")]
        public async Task<ActionResult<StudentClassByLastAcademicYearResponseDto>> GetAllStudentClassByLastAcademicYear([FromQuery] int currentAcademicYearId)
        {
            try
            {
                if (currentAcademicYearId <= 0)
                {
                    Console.WriteLine("Invalid AcademicYearId.");
                    return BadRequest("AcademicYearId phải là một số nguyên dương.");
                }

                Console.WriteLine("Fetching student classes by last academic year...");
                var result = await _studentClassService.GetAllStudentClassByLastAcademicYearAsync(currentAcademicYearId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching student classes: {ex.Message}");
                return NotFound("Không tìm thấy phân công lớp.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching student classes: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách phân công lớp năm học vừa kết thúc.");
            }
        }

        [HttpGet("non-eligible-students")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư,Giáo viên,Phụ huynh")]
        public async Task<ActionResult<IEnumerable<StudentClassResponseDto>>> GetNonEligibleStudentsByLastAcademicYear([FromQuery] int currentAcademicYearId)
        {
            try
            {
                if (currentAcademicYearId <= 0)
                {
                    Console.WriteLine("Invalid AcademicYearId.");
                    return BadRequest("AcademicYearId phải là một số nguyên dương.");
                }

                Console.WriteLine("Fetching non-eligible students by last academic year...");
                var nonEligibleStudents = await _studentClassService.GetNonEligibleStudentsByLastAcademicYearAsync(currentAcademicYearId);
                return Ok(nonEligibleStudents);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching non-eligible students: {ex.Message}");
                return NotFound("Không tìm thấy học sinh không đủ điều kiện lên lớp.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching non-eligible students: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách học sinh không đủ điều kiện lên lớp.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> CreateStudentClass([FromBody] StudentClassAssignmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Invalid input data.");
                return BadRequest("Dữ liệu đầu vào không hợp lệ.");
            }

            try
            {
                Console.WriteLine("Creating student class...");
                await _studentClassService.CreateStudentClassAsync(dto);
                return Ok("Phân công lớp thành công.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating student class: {ex.Message}");
                return BadRequest("Lỗi khi tạo phân công lớp.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error creating student class: {ex.Message}");
                return NotFound("Không tìm thấy dữ liệu liên quan.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error creating student class: {ex.Message}");
                return Conflict("Lỗi khi tạo phân công lớp.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error creating student class: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo phân công lớp.");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateStudentClasses([FromBody] List<StudentClassAssignmentDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                Console.WriteLine("Empty student class list.");
                return BadRequest("Danh sách phân công lớp không được rỗng.");
            }

            try
            {
                Console.WriteLine("Updating student classes...");
                await _studentClassService.UpdateStudentClassesAsync(dtos);
                return Ok("Cập nhật phân công lớp thành công.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating student classes: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating student classes: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating student classes: {ex.Message}");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating student classes: {ex.Message}");
                return StatusCode(500, "Lỗi hệ thống khi cập nhật phân công lớp.");
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
                    Console.WriteLine("Invalid student class ID.");
                    return BadRequest("ID phân công lớp phải là một số nguyên dương.");
                }

                Console.WriteLine("Deleting student class...");
                await _studentClassService.DeleteStudentClassAsync(id);
                return Ok("Xóa phân công lớp thành công.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error deleting student class: {ex.Message}");
                return NotFound("Không tìm thấy phân công lớp.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error deleting student class: {ex.Message}");
                return BadRequest("Lỗi khi xóa phân công lớp.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error deleting student class: {ex.Message}");
                return StatusCode(500, "Lỗi khi xóa phân công lớp.");
            }
        }

        [HttpGet("filter-data")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư,Giáo viên,Phụ huynh,Trưởng bộ môn")]
        public async Task<IActionResult> GetFilterData([FromQuery] int? classId = null, [FromQuery] int? semesterId = null)
        {
            try
            {
                Console.WriteLine("Fetching filter data...");
                var filterData = await _studentClassService.GetFilterDataAsync(classId, semesterId);
                return Ok(filterData);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error fetching filter data: {ex.Message}");
                return NotFound("Không tìm thấy dữ liệu liên quan.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching filter data: {ex.Message}");
                return BadRequest("Lỗi khi lấy dữ liệu lọc.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching filter data: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy dữ liệu lọc.");
            }
        }

        [HttpPost("bulk-transfer")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<ActionResult<List<BulkTransferResultDto>>> BulkTransferClass([FromBody] List<BulkClassTransferDto> dtos)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Invalid input data.");
                return BadRequest("Dữ liệu đầu vào không hợp lệ.");
            }

            try
            {
                if (dtos == null || !dtos.Any())
                {
                    Console.WriteLine("Empty transfer list.");
                    return BadRequest("Danh sách chuyển lớp không được rỗng.");
                }

                Console.WriteLine("Processing bulk class transfer...");
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
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error processing bulk transfer: {ex.Message}");
                return BadRequest("Lỗi khi chuyển lớp hàng loạt.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error processing bulk transfer: {ex.Message}");
                return NotFound("Không tìm thấy dữ liệu liên quan.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error processing bulk transfer: {ex.Message}");
                return Conflict("Lỗi khi chuyển lớp hàng loạt.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error processing bulk transfer: {ex.Message}");
                return StatusCode(500, "Lỗi khi chuyển lớp hàng loạt.");
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
                    Console.WriteLine("Invalid AcademicYearId.");
                    return BadRequest("AcademicYearId phải là một số nguyên dương.");
                }

                Console.WriteLine("Processing graduation...");
                await _studentClassService.ProcessGraduationAsync(academicYearId);
                return Ok("Xử lý tốt nghiệp thành công.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error processing graduation: {ex.Message}");
                return BadRequest("Lỗi khi xử lý tốt nghiệp.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error processing graduation: {ex.Message}");
                return NotFound("Không tìm thấy dữ liệu liên quan.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error processing graduation: {ex.Message}");
                return Conflict("Lỗi khi xử lý tốt nghiệp.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error processing graduation: {ex.Message}");
                return StatusCode(500, "Lỗi khi xử lý tốt nghiệp.");
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
                    Console.WriteLine("Invalid AcademicYearId.");
                    return BadRequest("AcademicYearId phải là một số nguyên dương.");
                }

                Console.WriteLine("Fetching classes with student count...");
                var classes = await _studentClassService.GetClassesWithStudentCountAsync(academicYearId);
                return Ok(classes);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching classes: {ex.Message}");
                return BadRequest("Lỗi khi lấy thông tin lớp.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching classes: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin lớp.");
            }
        }
    }
}