using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.HomeRooms.DTOs;
using Application.Features.HomeRooms.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Application.Features.HomeRooms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignHomeRoom : ControllerBase
    {
        private readonly IAssignHomeRoomService _assignHomeRoomService;

        public AssignHomeRoom(IAssignHomeRoomService assignHomeRoomService)
        {
            _assignHomeRoomService = assignHomeRoomService;
        }

        [HttpPost("assign")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> AssignHomeroom([FromBody] AssignHomeroomDto dto)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Invalid input data for homeroom assignment.");
                return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ." });
            }

            try
            {
                await _assignHomeRoomService.AssignHomeroomAsync(dto);
                return Ok(new { message = "Phân công giáo viên chủ nhiệm thành công." });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error assigning homeroom: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi phân công giáo viên chủ nhiệm." });
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error assigning homeroom: {ex.Message}");
                return NotFound(new { message = "Không tìm thấy giáo viên, lớp học hoặc học kỳ." });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access for homeroom assignment: {ex.Message}");
                return Forbid("Bạn không có quyền phân công giáo viên chủ nhiệm.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error assigning homeroom: {ex.Message}");
                return Conflict(new { message = "Lỗi khi phân công giáo viên chủ nhiệm." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error assigning homeroom: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi phân công giáo viên chủ nhiệm." });
            }
        }

        [HttpPut("update")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateHomeroomAssignments([FromBody] List<UpdateHomeroomDto> dtos)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Invalid input data for updating homeroom assignments.");
                return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ." });
            }

            try
            {
                await _assignHomeRoomService.UpdateHomeroomAssignmentsAsync(dtos);
                return Ok(new { message = "Cập nhật phân công giáo viên chủ nhiệm thành công." });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating homeroom assignments: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi cập nhật phân công giáo viên chủ nhiệm." });
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating homeroom assignments: {ex.Message}");
                return NotFound(new { message = "Không tìm thấy giáo viên, lớp học hoặc học kỳ." });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access for updating homeroom assignments: {ex.Message}");
                return Forbid("Bạn không có quyền cập nhật phân công giáo viên chủ nhiệm.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating homeroom assignments: {ex.Message}");
                return Conflict(new { message = "Lỗi khi cập nhật phân công giáo viên chủ nhiệm." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating homeroom assignments: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật phân công giáo viên chủ nhiệm." });
            }
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllHomeroomAssignments()
        {
            try
            {
                var assignments = await _assignHomeRoomService.GetAllHomeroomAssignmentsAsync();
                return Ok(assignments);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching homeroom assignments: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi lấy danh sách phân công giáo viên chủ nhiệm." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching homeroom assignments: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy danh sách phân công giáo viên chủ nhiệm." });
            }
        }
    }
}