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

        // GET: api/attendance/weekly?classId=1&weekStart=2025-04-21
        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeeklyAttendance([FromQuery] int classId, [FromQuery] DateOnly weekStart)
        {
            var data = await _service.GetWeeklyAttendanceAsync(classId, weekStart);
            return Ok(data);
        }

        // POST: api/attendance/upsert
        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertAttendances([FromBody] List<AttendanceDto> dtos)
        {
            await _service.UpsertAttendancesAsync(dtos);
            return Ok(new { message = "Điểm danh thành công." });
        }
    }

}
