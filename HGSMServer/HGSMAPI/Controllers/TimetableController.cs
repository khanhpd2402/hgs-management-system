using Application.Features.Timetables.DTOs;
using Application.Features.Timetables.Interfaces;
using Application.Features.Timetables.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> CreateTimetable([FromBody] CreateTimetableDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Timetable data is required.");
            }

            try
            {
                var createdTimetable = await _service.CreateTimetableAsync(dto);
                return Ok(createdTimetable);
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                return StatusCode(500, $"An error occurred while creating the timetable: {ex.Message}");
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetTimetableByStudent(int studentId, [FromQuery] int? semesterId = null, [FromQuery] DateOnly? effectiveDate = null)
        {
            try
            {
                var timetables = await _service.GetTimetableByStudentAsync(studentId, semesterId, effectiveDate);
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetTimetableByTeacher(int teacherId, [FromQuery] int? semesterId = null, [FromQuery] DateOnly? effectiveDate = null)
        {
            try
            {
                var timetables = await _service.GetTimetableByTeacherAsync(teacherId, semesterId, effectiveDate);
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetTimetableByClass(int classId, [FromQuery] int? semesterId = null, [FromQuery] DateOnly? effectiveDate = null)
        {
            try
            {
                var timetables = await _service.GetTimetableByClassAsync(classId, semesterId, effectiveDate);
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPut("detail")]
        public async Task<IActionResult> UpdateDetail(TimetableDetailDto dto)
        {
            var success = await _service.UpdateDetailAsync(dto);
            return success ? Ok() : BadRequest();
        }

        [HttpDelete("detail/{detailId}")]
        public async Task<IActionResult> DeleteDetail(int detailId)
        {
            var success = await _service.DeleteDetailAsync(detailId);
            return success ? Ok() : NotFound();
        }

        [HttpPost("check-conflict")]
        public async Task<IActionResult> CheckConflict(TimetableDetailDto dto)
        {
            var conflict = await _service.IsConflictAsync(dto);
            return Ok(new { conflict });
        }
    }
}