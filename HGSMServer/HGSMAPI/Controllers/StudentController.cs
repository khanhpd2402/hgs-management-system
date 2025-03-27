using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
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
        [Consumes("application/json")] // Chỉ chấp nhận Content-Type là application/json
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
                return CreatedAtAction(nameof(GetStudent), new { id = studentId, academicYearId = 1 }, createStudentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Student/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto updateStudentDto)
        {
            if (id != updateStudentDto.StudentId) return BadRequest("Student ID mismatch.");

            try
            {
                await _studentService.UpdateStudentAsync(updateStudentDto);
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
                await _studentService.ImportStudentsFromExcelAsync(file);
                return Ok("Import danh sách học sinh thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}