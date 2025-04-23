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
        private readonly IAttendanceService _service;

        public AttendanceController(IAttendanceService service)
        {
            _service = service;
        }

        // GET: api/attendance/weekly?teacherId=123&classId=1&semesterId=2&weekStart=2025-04-21
        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeeklyAttendance(
            [FromQuery] int teacherId,
            [FromQuery] int classId,
            [FromQuery] int semesterId,
            [FromQuery] DateOnly weekStart)
        {
            var data = await _service.GetWeeklyAttendanceAsync(teacherId, classId, semesterId, weekStart);
            return Ok(data);
        }

        // POST: api/attendance/upsert?teacherId=123&classId=1&semesterId=2
        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertAttendances(
            [FromQuery] int teacherId,
            [FromQuery] int classId,
            [FromQuery] int semesterId,
            [FromBody] List<AttendanceDto> dtos)
        {
            await _service.UpsertAttendancesAsync(teacherId, classId, semesterId, dtos);
            return Ok(new { message = "Điểm danh thành công." });
        }
    }

}
