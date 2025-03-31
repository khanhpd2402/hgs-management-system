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
        public async Task<IActionResult> Add(TimetableDto dto)
        {
            var result = await _service.AddAsync(dto);
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudent(int studentId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var result = await _service.GetTimetableByStudentAsync(studentId, semesterId, effectiveDate);
            return Ok(result);
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetByTeacher(int teacherId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var result = await _service.GetTimetableByTeacherAsync(teacherId, semesterId, effectiveDate);
            return Ok(result);
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