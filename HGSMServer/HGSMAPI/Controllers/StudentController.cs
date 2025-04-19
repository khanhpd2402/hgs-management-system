using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using ClosedXML.Excel;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService; private readonly HgsdbContext _context;

        public StudentController(IStudentService studentService, HgsdbContext context)
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Lấy danh sách học sinh theo năm học
        [HttpGet("{academicYearId}")]
        public async Task<IActionResult> GetAllStudentsWithParents(int academicYearId)
        {
            var students = await _studentService.GetAllStudentsWithParentsAsync(academicYearId);
            return Ok(students);
        }

        // Lấy chi tiết một học sinh theo ID và năm học
        [HttpGet("{id}/{academicYearId}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id, int academicYearId)
        {
            var student = await _studentService.GetStudentByIdAsync(id, academicYearId);
            if (student == null) return NotFound();

            return Ok(student);
        }

        // POST: api/Student

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto createStudentDto)
        {
            if (createStudentDto == null) return BadRequest("Student data cannot be null.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid input data.", errors });
            }

            try
            {
                var studentId = await _studentService.AddStudentAsync(createStudentDto);
                // Sử dụng AcademicYearId từ DTO nếu có, nếu không lấy từ GetCurrentAcademicYearIdAsync
                var academicYearId = createStudentDto.AcademicYearId ?? await GetCurrentAcademicYearIdAsync();
                var createdStudent = await _studentService.GetStudentByIdAsync(studentId, academicYearId);
                if (createdStudent == null)
                {
                    return NotFound(new { message = $"Student with ID {studentId} not found after creation." });
                }
                return CreatedAtAction(nameof(GetStudent), new { id = studentId, academicYearId }, createdStudent);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Phương thức phụ để lấy AcademicYearId hiện tại 
        private async Task<int> GetCurrentAcademicYearIdAsync()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var currentAcademicYear = await _context.AcademicYears
                .Where(ay => ay.StartDate <= currentDate && ay.EndDate >= currentDate)
                .Select(ay => ay.AcademicYearId)
                .FirstOrDefaultAsync();

            if (currentAcademicYear == 0)
                throw new Exception("Không tìm thấy năm học hiện tại.");

            return currentAcademicYear;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto updateStudentDto)
        {
            if (updateStudentDto == null) return BadRequest("Student data cannot be null.");

            try
            {
                // Truyền id từ URL trực tiếp vào UpdateStudentAsync
                await _studentService.UpdateStudentAsync(id, updateStudentDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportStudentsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file Excel!");

            try
            {
                var results = await _studentService.ImportStudentsFromExcelAsync(file);
                return Ok(new ApiResponse(true, "Import danh sách học sinh hoàn tất", results));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(false, $"Lỗi khi import: {ex.Message}"));
            }
        }

        public class ApiResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }

            public ApiResponse(bool success, string message, object data = null)
            {
                Success = success;
                Message = message;
                Data = data;
            }
        }
        



    }
}