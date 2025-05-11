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
                return StatusCode(500, "Lỗi khi lấy điểm của học sinh." + ex.Message);
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
                return StatusCode(500, "Lỗi khi lấy điểm của giáo viên." + ex.Message);
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
                return StatusCode(500, "Lỗi khi lấy điểm toàn trường." + ex.Message);
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
                return BadRequest("Lỗi khi cập nhật điểm." + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating grades: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật điểm." + ex.Message);
            }
        }

        [HttpGet("student/{studentId}/grades-by-subject")]
        public async Task<IActionResult> GetGradeSummaryEachSubjectByStudentAsync(int studentId, [FromQuery] int semesterId)
        {
            var result = await _gradeService.GetGradeSummaryEachSubjectByStudentAsync(studentId, semesterId);

            if (result == null || !result.Any())
                return NotFound("Không tìm thấy.");

            return Ok(result);
        }
        [HttpGet("student/{studentId}/grade-summary")]
        public async Task<IActionResult> GetTotalGradeSummaryByStudentAsync(int studentId, [FromQuery] int semesterId)
        {
            var result = await _gradeService.GetTotalGradeSummaryByStudentAsync(studentId, semesterId);

            if (result == null)
                return NotFound("Không tìm thấy.");

            return Ok(result);
        }
        [HttpPost("import/{classId}/{subjectId}/{semesterId}")]
        [ProducesResponseType(typeof(ImportGradesResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ImportGradesResultDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImportGrades([FromRoute] int classId, [FromRoute] int subjectId, [FromRoute] int semesterId, IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                // _logger?.LogWarning("File nhập điểm không được cung cấp hoặc file trống.");
                return BadRequest(new ImportGradesResultDto
                {
                    IsSuccess = false,
                    Message = "File Excel không được cung cấp hoặc file trống.",
                    Errors = new List<string> { "Vui lòng chọn một file Excel để nhập điểm." }
                });
            }


            string[] permittedExtensions = { ".xlsx" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return BadRequest(new ImportGradesResultDto { IsSuccess = false, Message = $"Loại file không hợp lệ. Chỉ chấp nhận file {string.Join(", ", permittedExtensions)}." });
            }

            try
            {
                var result = await _gradeService.ImportGradesFromExcelAsync(classId, subjectId, semesterId, file);

                if (result.IsSuccess)
                {
                    // _logger?.LogInformation($"Nhập điểm thành công cho Lớp ID: {classId}, Môn học ID: {subjectId}. {result.Message}");
                    return Ok(result);
                }
                else
                {
                    // Các lỗi nghiệp vụ đã được GradeService xử lý và đưa vào result.Errors
                    // Trả về BadRequest nếu IsSuccess là false để client biết có vấn đề với request hoặc dữ liệu.
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                // Lỗi không mong muốn xảy ra trong service mà không được catch cụ thể
                return StatusCode(StatusCodes.Status500InternalServerError, new ImportGradesResultDto
                {
                    IsSuccess = false,
                    Message = "Đã có lỗi không mong muốn xảy ra ở máy chủ.",
                    Errors = new List<string> { $"Chi tiết lỗi: {ex.Message}" } // Chỉ trả về ex.Message trong môi trường dev
                });

            }
        }
        [HttpGet("class-summary/{classId}/{semesterId}")]
        [ProducesResponseType(typeof(ClassGradesSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetClassGradesSummary(int classId, int semesterId)
        {
            // _logger?.LogInformation($"Yêu cầu xuất điểm tổng kết cho Lớp ID: {classId}, Học kỳ ID: {semesterId}");
            try
            {
                var summary = await _gradeService.GetClassGradesSummaryAsync(classId, semesterId);
                if (summary == null)
                {
                    // _logger?.LogWarning($"Không tìm thấy dữ liệu tổng kết cho Lớp ID: {classId}, Học kỳ ID: {semesterId}.");
                    return NotFound(new ProblemDetails
                    {
                        Title = "Không tìm thấy dữ liệu",
                        Detail = $"Không tìm thấy dữ liệu tổng kết cho Lớp ID: {classId} và Học kỳ ID: {semesterId}.",
                        Status = StatusCodes.Status404NotFound,
                        Instance = HttpContext.Request.Path
                    });
                }
                return Ok(summary);
            }
            catch (Exception ex)
            {
                // _logger?.LogError(ex, $"Lỗi khi xuất điểm tổng kết cho Lớp ID: {classId}, Học kỳ ID: {semesterId}.");
                return Problem(
                    detail: ex.StackTrace, // Chỉ bao gồm StackTrace trong môi trường Development
                    title: "Đã có lỗi không mong muốn xảy ra ở máy chủ khi xử lý yêu cầu xuất điểm tổng kết.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: HttpContext.Request.Path
                );
            }
        }
    }
}