using Application.Features.Timetables.DTOs;
using Application.Features.Timetables.Interfaces;
using Application.Features.Timetables.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly TimetableService _timetableService;

        public TimetableController(TimetableService timetableService)
        {
            _timetableService = timetableService;
        }

        [HttpPost("generate")]
        public async Task<ActionResult<ScheduleResponseDto>> GenerateTimetable([FromBody] TimetableRequest request)
        {
            try
            {
                var timetable = await _timetableService.GenerateTimetableAsync(request);
                return Ok(timetable);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }
        [HttpPost("save-generated")]
        public async Task<IActionResult> SaveGeneratedTimetable([FromBody] List<ManualTimetableDto> timetableDtos)
        {
            try
            {
                await _timetableService.SaveGeneratedTimetableAsync(timetableDtos);
                return Ok(new { Message = "Timetable saved successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        } 
        // Thêm thủ công
        [HttpPost("manual")]
        public async Task<IActionResult> AddManualTimetable([FromBody] ManualTimetableDto dto)
        {
            try
            {
                await _timetableService.AddManualTimetableAsync(dto);
                return Ok(new { Message = "Timetable added successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }

        // Sửa thời khóa biểu
        [HttpPut("{timetableId}")]
        public async Task<IActionResult> UpdateTimetable(int timetableId, [FromBody] ManualTimetableDto dto)
        {
            try
            {
                await _timetableService.UpdateTimetableAsync(timetableId, dto);
                return Ok(new { Message = "Timetable updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }

        // Xóa thời khóa biểu
        [HttpDelete("{timetableId}")]
        public async Task<IActionResult> DeleteTimetable(int timetableId)
        {
            try
            {
                await _timetableService.DeleteTimetableAsync(timetableId);
                return Ok(new { Message = "Timetable deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("parent/{studentId}")]
        public async Task<ActionResult<List<RoleBasedTimetableDto>>> GetTimetableForParent(int studentId, [FromQuery] string schoolYear, [FromQuery] int semester, [FromQuery] string? effectiveDate = null)
        {
            try
            {
                DateOnly? date = effectiveDate != null ? DateOnly.Parse(effectiveDate) : null;
                var timetable = await _timetableService.GetTimetableForParentAsync(studentId, schoolYear, semester, date);
                return Ok(timetable);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<List<RoleBasedTimetableDto>>> GetTimetableForTeacher(int teacherId, [FromQuery] string schoolYear, [FromQuery] int semester, [FromQuery] string? effectiveDate = null)
        {
            try
            {
                DateOnly? date = effectiveDate != null ? DateOnly.Parse(effectiveDate) : null;
                var timetable = await _timetableService.GetTimetableForTeacherAsync(teacherId, schoolYear, semester, date);
                return Ok(timetable);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("principal")]
        public async Task<ActionResult<List<RoleBasedTimetableDto>>> GetTimetableForPrincipal([FromQuery] string schoolYear, [FromQuery] int semester, [FromQuery] string? effectiveDate = null)
        {
            try
            {
                DateOnly? date = effectiveDate != null ? DateOnly.Parse(effectiveDate) : null;
                var timetable = await _timetableService.GetTimetableForPrincipalAsync(schoolYear, semester, date);
                return Ok(timetable);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }
    }
}