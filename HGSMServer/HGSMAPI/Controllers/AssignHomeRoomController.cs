using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.HomeRooms.DTOs;
using Application.Features.HomeRooms.Interfaces;
using Application.Features.TeachingAssignments.DTOs;
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

        /// <summary>
        /// Gán giáo viên chủ nhiệm cho một lớp trong một học kỳ.
        /// </summary>
        /// <param name="dto">Thông tin phân công giáo viên chủ nhiệm.</param>
        /// <returns>Trạng thái thành công hoặc lỗi.</returns>
        [HttpPost("assign")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> AssignHomeroom([FromBody] AssignHomeroomDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _assignHomeRoomService.AssignHomeroomAsync(dto);
                return Ok(new { Message = "Homeroom teacher assigned successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while assigning homeroom teacher.", Detail = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật danh sách phân công giáo viên chủ nhiệm.
        /// </summary>
        /// <param name="dtos">Danh sách thông tin cập nhật phân công.</param>
        /// <returns>Trạng thái thành công hoặc lỗi.</returns>
        [HttpPut("update")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateHomeroomAssignments([FromBody] List<UpdateHomeroomDto> dtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _assignHomeRoomService.UpdateHomeroomAssignmentsAsync(dtos);
                return Ok(new { Message = "Homeroom assignments updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating homeroom assignments.", Detail = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả các phân công giáo viên chủ nhiệm.
        /// </summary>
        /// <returns>Danh sách phân công giáo viên chủ nhiệm.</returns>
        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllHomeroomAssignments()
        {
                var assignments = await _assignHomeRoomService.GetAllHomeroomAssignmentsAsync();
                return Ok(assignments);
        }
    }
}