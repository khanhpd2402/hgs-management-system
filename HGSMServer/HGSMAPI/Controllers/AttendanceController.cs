using Application.Features.Attendances.DTOs;
using Application.Features.Attendances.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttendance([FromBody] AttendanceRequest request)
        {
            var attendance = await _attendanceService.CreateAttendance(request);
            return CreatedAtAction(nameof(GetAttendanceById), new { id = attendance.AttendanceId }, attendance);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] AttendanceRequest request)
        {
            var attendance = await _attendanceService.UpdateAttendance(id, request);
            return Ok(attendance);
        }

        [HttpGet("{id}", Name = "GetAttendanceById")]
        public async Task<IActionResult> GetAttendanceById(int id)
        {
            var attendance = await _attendanceService.GetAttendanceById(id);
            if (attendance == null) return NotFound();
            return Ok(attendance);
        }

        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetAttendancesByClass(
            int classId,
            [FromQuery] string date,
            [FromQuery] string shift)
        {
            if (!DateOnly.TryParse(date, out DateOnly parsedDate))
                return BadRequest("Invalid date format. Use yyyy-MM-dd.");

            var attendances = await _attendanceService.GetAttendancesByClass(classId, parsedDate, shift);
            return Ok(attendances);
        }
    }
}
