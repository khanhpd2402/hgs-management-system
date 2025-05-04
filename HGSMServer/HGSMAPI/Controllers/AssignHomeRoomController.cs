using Application.Features.HomeRooms.DTOs;
using Application.Features.HomeRooms.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Invalid input data for homeroom assignment.");
                    return BadRequest("Dữ liệu đầu vào không hợp lệ.");
                }

                Console.WriteLine("Assigning homeroom...");
                await _assignHomeRoomService.AssignHomeroomAsync(dto);
                return Ok("Phân công giáo viên chủ nhiệm thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error assigning homeroom: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error assigning homeroom: {ex.Message}");
                return NotFound("Không tìm thấy giáo viên, lớp học hoặc học kỳ.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access for homeroom assignment: {ex.Message}");
                return Unauthorized("Bạn không có quyền phân công giáo viên chủ nhiệm.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error assigning homeroom: {ex.Message}");
                return Conflict("Lỗi khi phân công giáo viên chủ nhiệm.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error assigning homeroom: {ex.Message}");
                return StatusCode(500, "Lỗi khi phân công giáo viên chủ nhiệm.");
            }
        }

        [HttpPut("update")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateHomeroomAssignments([FromBody] List<UpdateHomeroomDto> dtos)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Invalid input data for updating homeroom assignments.");
                    return BadRequest("Dữ liệu đầu vào không hợp lệ.");
                }

                Console.WriteLine("Updating homeroom assignments...");
                await _assignHomeRoomService.UpdateHomeroomAssignmentsAsync(dtos);
                return Ok("Cập nhật phân công giáo viên chủ nhiệm thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating homeroom assignments: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật phân công giáo viên chủ nhiệm.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating homeroom assignments: {ex.Message}");
                return NotFound("Không tìm thấy giáo viên, lớp học hoặc học kỳ.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access for updating homeroom assignments: {ex.Message}");
                return Unauthorized("Bạn không có quyền cập nhật phân công giáo viên chủ nhiệm.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating homeroom assignments: {ex.Message}");
                return Conflict("Lỗi khi cập nhật phân công giáo viên chủ nhiệm.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating homeroom assignments: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật phân công giáo viên chủ nhiệm.");
            }
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllHomeroomAssignments()
        {
            try
            {
                Console.WriteLine("Fetching all homeroom assignments...");
                var assignments = await _assignHomeRoomService.GetAllHomeroomAssignmentsAsync();
                return Ok(assignments);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching homeroom assignments: {ex.Message}");
                return BadRequest("Lỗi khi lấy danh sách phân công giáo viên chủ nhiệm.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching homeroom assignments: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách phân công giáo viên chủ nhiệm.");
            }
        }
    }
}