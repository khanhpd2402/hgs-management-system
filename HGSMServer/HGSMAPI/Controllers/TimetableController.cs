﻿using Application.Features.Timetables;
using Application.Features.Timetables.DTOs;
using Application.Features.Timetables.Interfaces;
using Common.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetablesController : ControllerBase
    {
        private readonly ITimetableService _service;

        public TimetablesController(ITimetableService service)
        {
            _service = service;
        }

        [HttpPost("create-timetable")]
        public async Task<IActionResult> CreateTimetable([FromBody] CreateTimetableDto dto)
        {
            if (!ModelState.IsValid || dto == null)
            {
                Console.WriteLine("Invalid timetable data.");
                return BadRequest("Dữ liệu thời khóa biểu không hợp lệ.");
            }

            try
            {
                Console.WriteLine("Creating timetable...");
                var createdTimetable = await _service.CreateTimetableAsync(dto);
                return Ok(createdTimetable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating timetable: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo thời khóa biểu.");
            }
        }

        [HttpGet("student/{studentId}/semester/{semesterId}")]
        public async Task<IActionResult> GetTimetableByStudent(int studentId, int semesterId)
        {
            try
            {
                Console.WriteLine("Fetching student timetable...");
                var timetables = await _service.GetTimetableByStudentAsync(studentId, semesterId);
                if (timetables == null || !timetables.Any())
                {
                    Console.WriteLine("No timetables found for student.");
                    return NotFound("Không tìm thấy thời khóa biểu cho học sinh.");
                }
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving student timetable: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thời khóa biểu học sinh.");
            }
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetTimetableByTeacher(int teacherId)
        {
            try
            {
                Console.WriteLine("Fetching teacher timetable...");
                var timetables = await _service.GetTimetableByTeacherAsync(teacherId);
                if (timetables == null || !timetables.Any())
                {
                    Console.WriteLine("No timetables found for teacher.");
                    return NotFound("Không tìm thấy thời khóa biểu cho giáo viên.");
                }
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving teacher timetable: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thời khóa biểu giáo viên.");
            }
        }

        [HttpGet("TimetablesForPrincipal/{timetableId}")]
        public async Task<IActionResult> GetTimetablesForPrincipalAsync(int timetableId, [FromQuery] string? status = null)
        {
            try
            {
                Console.WriteLine("Fetching timetables for principal...");
                var timetables = await _service.GetTimetablesForPrincipalAsync(timetableId, status);
                if (timetables == null || !timetables.Any())
                {
                    Console.WriteLine("No timetables found for principal.");
                    return NotFound("Không tìm thấy thời khóa biểu.");
                }
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving semester timetables: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thời khóa biểu.");
            }
        }

        [HttpGet("semester/{semesterId}")]
        public async Task<IActionResult> GetTimetablesBySemester(int semesterId)
        {
            try
            {
                Console.WriteLine("Fetching timetables by semester...");
                var timetables = await _service.GetTimetablesBySemesterAsync(semesterId);
                if (timetables == null || !timetables.Any())
                {
                    Console.WriteLine("No timetables found for semester.");
                    return NotFound("Không tìm thấy thời khóa biểu cho học kỳ.");
                }
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving timetables: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thời khóa biểu.");
            }
        }

        [HttpPut("info")]
        public async Task<IActionResult> UpdateTimetableInfo([FromBody] UpdateTimetableInfoDto dto)
        {
            if (!ModelState.IsValid || dto == null)
            {
                Console.WriteLine("Invalid timetable data.");
                return BadRequest("Dữ liệu thời khóa biểu không hợp lệ.");
            }

            try
            {
                Console.WriteLine("Updating timetable info...");
                var updatedTimetable = await _service.UpdateTimetableInfoAsync(dto);
                return Ok(updatedTimetable);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating timetable: {ex.Message}");
                return NotFound("Không tìm thấy thời khóa biểu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating timetable info: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật thời khóa biểu.");
            }
        }

        [HttpPut("details")]
        public async Task<IActionResult> UpdateMultipleDetails([FromBody] UpdateTimetableDetailsDto dto)
        {
            if (!ModelState.IsValid || dto == null || !dto.Details.Any())
            {
                Console.WriteLine("Invalid timetable details data.");
                return BadRequest("Dữ liệu chi tiết thời khóa biểu không hợp lệ.");
            }

            try
            {
                Console.WriteLine("Updating timetable details...");
                var success = await _service.UpdateMultipleDetailsAsync(dto);
                if (!success)
                {
                    Console.WriteLine("No details were updated.");
                    return BadRequest("Không có chi tiết nào được cập nhật.");
                }
                return Ok("Cập nhật chi tiết thời khóa biểu thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating timetable details: {ex.Message}");
                return NotFound("Không tìm thấy thời khóa biểu.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating timetable details: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật chi tiết thời khóa biểu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating timetable details: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật chi tiết thời khóa biểu.");
            }
        }

        [HttpDelete("detail/{detailId}")]
        public async Task<IActionResult> DeleteDetail(int detailId)
        {
            try
            {
                Console.WriteLine("Deleting timetable detail...");
                var success = await _service.DeleteDetailAsync(detailId);
                if (!success)
                {
                    Console.WriteLine("Timetable detail not found.");
                    return NotFound("Không tìm thấy chi tiết thời khóa biểu.");
                }
                return Ok("Xóa chi tiết thời khóa biểu thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting timetable detail: {ex.Message}");
                return StatusCode(500, "Lỗi khi xóa chi tiết thời khóa biểu.");
            }
        }

        //[HttpPost("check-conflict")]
        //public async Task<IActionResult> CheckConflict([FromBody] TimetableDetailDto dto)
        //{
        //    if (!ModelState.IsValid || dto == null)
        //    {
        //        Console.WriteLine("Invalid timetable detail data.");
        //        return BadRequest("Dữ liệu chi tiết thời khóa biểu không hợp lệ.");
        //    }

        //    try
        //    {
        //        Console.WriteLine("Checking timetable conflict...");
        //        var conflict = await _service.IsConflictAsync(dto);
        //        return Ok(new { Conflict = conflict });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error checking conflict: {ex.Message}");
        //        return StatusCode(500, "Lỗi khi kiểm tra xung đột thời khóa biểu.");
        //    }
        //}
        [HttpPost("import")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)] // Sửa typeof nếu cần
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // Đảm bảo endpoint này không yêu cầu [Authorize] nếu test qua Swagger UI mà chưa login
        public async Task<IActionResult> ImportTimetableAsync( IFormFile file,
           [FromForm] int semesterId,               // Giữ lại [FromForm]
           [FromForm] string effectiveDateString)   // Giữ lại [FromForm]
        {
            // --- Validation đầu vào ---
            if (file == null || file.Length == 0)
            {
                return BadRequest("Vui lòng chọn file Excel để import.");
            }
            if (semesterId <= 0)
            {
                return BadRequest("Học kỳ không hợp lệ.");
            }
            if (!DateOnly.TryParseExact(effectiveDateString, AppConstants.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var effectiveDate))
            {
                return BadRequest($"Định dạng ngày hiệu lực không đúng. Vui lòng sử dụng định dạng '{AppConstants.DATE_FORMAT}'.");
            }
            // --- Kết thúc Validation ---

            // --- Gọi Service xử lý ---
            try
            {
                var createdTimetable = await _service.ImportTimetableAsync(file, semesterId, effectiveDate);
                return Ok(new { message = "Import thời khóa biểu thành công.", timetableId = createdTimetable.TimetableId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Lỗi dữ liệu import: {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest($"Lỗi dữ liệu import: Không tìm thấy thông tin tham chiếu - {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing timetable: {ex}"); // Log lỗi ra console để debug
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi hệ thống trong quá trình import thời khóa biểu.");
            }
            // --- Kết thúc gọi Service ---
        }

        [HttpPost("create-timetable-detail")]
        public async Task<IActionResult> Create([FromBody] CreateTimetableDetailRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _service.CreateDetailAsync(request);
                return Ok(new { message = "Tạo tiết học thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}