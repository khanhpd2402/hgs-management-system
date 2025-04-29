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

        [HttpGet("student")]
        public async Task<IActionResult> GetGradesForStudent(int studentId, int semesterId)
        {
            try
            {
                Console.WriteLine("Fetching grades for student...");
                var result = await _gradeService.GetGradesForStudentAsync(studentId, semesterId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grades for student: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy điểm của học sinh.");
            }
        }

        [HttpGet("teacher")]
        public async Task<IActionResult> GetGradesForTeacher(int teacherId, int classId, int subjectId, int semesterId)
        {
            try
            {
                Console.WriteLine("Fetching grades for teacher...");
                var result = await _gradeService.GetGradesForTeacherAsync(teacherId, classId, subjectId, semesterId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grades for teacher: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy điểm của giáo viên.");
            }
        }

        [HttpGet("school")]
        public async Task<IActionResult> GetGradesForPrincipal(int classId, int subjectId, int semesterId)
        {
            try
            {
                Console.WriteLine("Fetching grades for principal...");
                var result = await _gradeService.GetGradesForPrincipalAsync(classId, subjectId, semesterId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grades for principal: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy điểm toàn trường.");
            }
        }

        [HttpPut("update-multiple-scores")]
        public async Task<IActionResult> UpdateMultipleGrades([FromBody] UpdateMultipleGradesDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Invalid grade data.");
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                Console.WriteLine("Updating multiple grades...");
                var result = await _gradeService.UpdateMultipleGradesAsync(dto);
                if (!result)
                {
                    Console.WriteLine("No grades found to update.");
                    return NotFound("Không tìm thấy điểm để cập nhật.");
                }
                return Ok("Cập nhật điểm thành công.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating grades: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật điểm.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating grades: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật điểm.");
            }
        }

        [HttpGet("student/{studentId}/grades-by-subject")]
        public async Task<IActionResult> GetGradeSummaryEachSubjectByStudentAsync(int studentId, [FromQuery] int semesterId)
        {
            var result = await _gradeService.GetGradeSummaryEachSubjectByStudentAsync(studentId, semesterId);

            if (result == null || !result.Any())
                return NotFound("No grades found for the given student and semester.");

            return Ok(result);
        }
        [HttpGet("student/{studentId}/grade-summary")]
        public async Task<IActionResult> GetTotalGradeSummaryByStudentAsync(int studentId, [FromQuery] int semesterId)
        {
            var result = await _gradeService.GetTotalGradeSummaryByStudentAsync(studentId, semesterId);

            if (result == null)
                return NotFound("No grade summary found for the given student and semester.");

            return Ok(result);
        }


    }
}