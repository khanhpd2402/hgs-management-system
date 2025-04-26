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
                return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ." });
            }

            try
            {
                await _assignHomeRoomService.AssignHomeroomAsync(dto);
                return Ok(new { message = "Phân công giáo viên chủ nhiệm thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình phân công giáo viên chủ nhiệm." });
            }
        }

        [HttpPut("update")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateHomeroomAssignments([FromBody] List<UpdateHomeroomDto> dtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ." });
            }

            try
            {
                await _assignHomeRoomService.UpdateHomeroomAssignmentsAsync(dtos);
                return Ok(new { message = "Cập nhật phân công giáo viên chủ nhiệm thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình cập nhật phân công giáo viên chủ nhiệm." });
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
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy danh sách phân công giáo viên chủ nhiệm." });
            }
        }
    }
}