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
        private readonly IStudentService _studentService;
        private readonly HgsdbContext _context;

        public StudentController(IStudentService studentService, HgsdbContext context)
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("{academicYearId}")]
        public async Task<IActionResult> GetAllStudentsWithParents(int academicYearId)
        {
            try
            {
                Console.WriteLine("Fetching all students with parents...");
                var students = await _studentService.GetAllStudentsWithParentsAsync(academicYearId);
                return Ok(students);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching students: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách học sinh.");
            }
        }

        [HttpGet("{id}/{academicYearId}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id, int academicYearId)
        {
            try
            {
                Console.WriteLine("Fetching student...");
                var student = await _studentService.GetStudentByIdAsync(id, academicYearId);
                if (student == null)
                {
                    Console.WriteLine("Student not found.");
                    return NotFound("Không tìm thấy học sinh.");
                }
                return Ok(student);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching student: {ex.Message}");
                return BadRequest("Lỗi khi lấy thông tin học sinh.");
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto createStudentDto)
        {
            if (createStudentDto == null)
            {
                Console.WriteLine("Student data is null.");
                return BadRequest("Dữ liệu học sinh không được để trống.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                Console.WriteLine($"Validation errors: {string.Join(", ", errors)}");
                return BadRequest("Dữ liệu đầu vào không hợp lệ.");
            }

            try
            {
                Console.WriteLine("Creating student...");
                var studentId = await _studentService.AddStudentAsync(createStudentDto);
                var academicYearId = createStudentDto.AcademicYearId ?? await GetCurrentAcademicYearIdAsync();
                var createdStudent = await _studentService.GetStudentByIdAsync(studentId, academicYearId);
                if (createdStudent == null)
                {
                    Console.WriteLine("Student not found after creation.");
                    return NotFound("Không tìm thấy học sinh sau khi tạo.");
                }
                return CreatedAtAction(nameof(GetStudent), new { id = studentId, academicYearId }, createdStudent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating student: {ex.Message}");
                return BadRequest("Lỗi khi thêm học sinh.");
            }
        }

        private async Task<int> GetCurrentAcademicYearIdAsync()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var currentAcademicYear = await _context.AcademicYears
                .Where(ay => ay.StartDate <= currentDate && ay.EndDate >= currentDate)
                .Select(ay => ay.AcademicYearId)
                .FirstOrDefaultAsync();

            if (currentAcademicYear == 0)
            {
                Console.WriteLine("Current academic year not found.");
                throw new Exception("Không tìm thấy năm học hiện tại.");
            }

            return currentAcademicYear;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto updateStudentDto)
        {
            if (updateStudentDto == null)
            {
                Console.WriteLine("Student data is null.");
                return BadRequest("Dữ liệu học sinh không được để trống.");
            }

            try
            {
                Console.WriteLine("Updating student...");
                await _studentService.UpdateStudentAsync(id, updateStudentDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating student: {ex.Message}");
                return NotFound("Không tìm thấy học sinh.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating student: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật học sinh.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                Console.WriteLine("Deleting student...");
                await _studentService.DeleteStudentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting student: {ex.Message}");
                return BadRequest("Lỗi khi xóa học sinh.");
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportStudentsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("Excel file is empty or not provided.");
                return BadRequest(new { message = "Vui lòng chọn file Excel!" });
            }

            try
            {
                Console.WriteLine("Importing students from Excel...");
                var results = await _studentService.ImportStudentsFromExcelAsync(file);

                // Kiểm tra nội dung results để xác định có lỗi hay không
                if (results.Any(r => r.StartsWith("Lỗi")))
                {
                    return BadRequest(new { message = "Có lỗi xảy ra khi nhập học sinh từ Excel", errors = results });
                }

                return Ok(new { message = "Nhập học sinh thành công", results });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing students: {ex.Message}\nStackTrace: {ex.StackTrace}");
                // Trả về mã 400 nếu lỗi liên quan đến dữ liệu đầu vào, hoặc 500 nếu lỗi hệ thống
                if (ex.Message.Contains("Thiếu") || ex.Message.Contains("không hợp lệ") || ex.Message.Contains("đã tồn tại"))
                {
                    return BadRequest(new { message = "Lỗi khi nhập học sinh từ Excel", error = ex.Message });
                }
                return StatusCode(500, new { message = "Lỗi hệ thống khi nhập học sinh từ Excel", error = ex.Message });
            }
        }
    }
}