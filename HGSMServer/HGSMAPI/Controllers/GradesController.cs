using Application.Features.Grades.DTOs;
using Application.Features.Grades.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;

        public GradesController(IGradeService gradesService)
        {
            _gradeService = gradesService;
        }

        // Học sinh xem điểm
        [HttpGet("student")]
        public async Task<IActionResult> GetGradesForStudent(int studentId, int semesterId)
        {
            var result = await _gradeService.GetGradesForStudentAsync(studentId, semesterId);
            return Ok(result);
        }

        // Giáo viên xem điểm theo lớp - môn - học kỳ
        [HttpGet("teacher")]
        public async Task<IActionResult> GetGradesForTeacher(int teacherId, int classId, int subjectId, int semesterId)
        {
            var result = await _gradeService.GetGradesForTeacherAsync(teacherId, classId, subjectId, semesterId);
            return Ok(result);
        }

        // Hiệu trưởng xem điểm toàn trường theo lớp - môn - học kỳ
        [HttpGet("school")]
        public async Task<IActionResult> GetGradesForPrincipal(int classId, int subjectId, int semesterId)
        {
            var result = await _gradeService.GetGradesForPrincipalAsync(classId, subjectId, semesterId);
            return Ok(result);
        }
        [HttpPut("update-multiple-scores")]
        public async Task<IActionResult> UpdateMultipleGrades([FromBody] UpdateMultipleGradesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _gradeService.UpdateMultipleGradesAsync(dto);
                if (!result)
                    return NotFound("No grades found to update");

                return Ok("Grades updated successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("summary/{studentId}/{semesterId}")]
        public async Task<IActionResult> GetGradeSummaryForStudent(int studentId, int semesterId)
        {
            try
            {
                var summaries = await _gradeService.GetGradeSummaryByStudentAsync(studentId, semesterId);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
