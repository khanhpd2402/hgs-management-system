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

        // POST: api/Timetables
        [HttpPost]
        public async Task<IActionResult> CreateTimetable([FromBody] CreateTimetableDto dto)
        {
            if (!ModelState.IsValid || dto == null)
            {
                return BadRequest("Invalid timetable data.");
            }

            try
            {
                var createdTimetable = await _service.CreateTimetableAsync(dto);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the timetable: {ex.Message}");
            }
        }

        // GET: api/Timetables/student/{studentId}/semester/{semesterId}
        [HttpGet("student/{studentId}/semester/{semesterId}")]
        public async Task<IActionResult> GetTimetableByStudent(int studentId, int semesterId)
        {
            try
            {
                var timetables = await _service.GetTimetableByStudentAsync(studentId, semesterId);
                if (timetables == null || !timetables.Any())
                {
                    return NotFound($"No timetables found for student ID {studentId} in semester {semesterId}");
                }
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving student timetable: {ex.Message}");
            }
        }

        /// GET: api/Timetables/teacher/{teacherId}
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetTimetableByTeacher(int teacherId)
        {
            try
            {
                var timetables = await _service.GetTimetableByTeacherAsync(teacherId);
                if (timetables == null || !timetables.Any())
                {
                    return NotFound($"No timetables found for teacher ID {teacherId}");
                }
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving teacher timetable: {ex.Message}");
            }
        }

        // GET: api/Timetables/semester/{timetableId}
        [HttpGet("TimetablesForPrincipal/{timetableId}")]
        public async Task<IActionResult> GetTimetablesForPrincipalAsync(int timetableId, [FromQuery] string? status = null)
        {
            try
            {
                var timetables = await _service.GetTimetablesForPrincipalAsync(timetableId, status);
                if (timetables == null || !timetables.Any())
                {
                    return NotFound($"No timetables found for semester {timetableId} with status '{status ?? "any"}'");
                }
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving semester timetables: {ex.Message}");
            }
        }

        [HttpGet("semester/{semesterId}")]
        public async Task<IActionResult> GetTimetablesBySemester(int semesterId)
        {
            try
            {
                var timetables = await _service.GetTimetablesBySemesterAsync(semesterId);
                if (timetables == null || !timetables.Any())
                {
                    return NotFound($"No timetables found for SemesterId {semesterId}.");
                }
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving timetables: {ex.Message}");
            }
        }
        [HttpPut("info")]
        public async Task<IActionResult> UpdateTimetableInfo([FromBody] UpdateTimetableInfoDto dto)
        {
            if (!ModelState.IsValid || dto == null)
            {
                return BadRequest("Invalid timetable data.");
            }

            try
            {
                var updatedTimetable = await _service.UpdateTimetableInfoAsync(dto);
                return Ok(updatedTimetable);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating timetable info: {ex.Message}");
            }
        }

        [HttpPut("details")]
        public async Task<IActionResult> UpdateMultipleDetails([FromBody] UpdateTimetableDetailsDto dto)
        {
            if (!ModelState.IsValid || dto == null || !dto.Details.Any())
            {
                return BadRequest("Invalid timetable details data.");
            }

            try
            {
                var success = await _service.UpdateMultipleDetailsAsync(dto);
                if (!success)
                {
                    return BadRequest("No details were updated.");
                }
                return Ok("Timetable details updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Conflict
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating timetable details: {ex.Message}");
            }
        }

        // DELETE: api/Timetables/detail/{detailId}
        [HttpDelete("detail/{detailId}")]
        public async Task<IActionResult> DeleteDetail(int detailId)
        {
            try
            {
                var success = await _service.DeleteDetailAsync(detailId);
                if (!success)
                {
                    return NotFound($"Timetable detail with ID {detailId} not found.");
                }
                return Ok("Timetable detail deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting timetable detail: {ex.Message}");
            }
        }

        // POST: api/Timetables/check-conflict
        [HttpPost("check-conflict")]
        public async Task<IActionResult> CheckConflict([FromBody] TimetableDetailDto dto)
        {
            if (!ModelState.IsValid || dto == null)
            {
                return BadRequest("Invalid timetable detail data.");
            }

            try
            {
                var conflict = await _service.IsConflictAsync(dto);
                return Ok(new { Conflict = conflict });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error checking conflict: {ex.Message}");
            }
        }
    }
}