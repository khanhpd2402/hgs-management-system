using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using Microsoft.AspNetCore.Http;
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
            _studentService = studentService;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IQueryable<StudentDto>> GetStudents()
        {
            var students = _studentService.GetAllStudents();
            return Ok(students.AsQueryable());
        }

        // GET: api/Student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();

            return Ok(student);
        }

        // POST: api/Student
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDto studentDto)
        {
            if (studentDto == null) return BadRequest();

            await _studentService.AddStudentAsync(studentDto);
            return CreatedAtAction(nameof(GetStudent), new { id = studentDto.StudentId }, studentDto);
        }

        // PUT: api/Student/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDto studentDto)
        {
            if (id != studentDto.StudentId) return BadRequest();

            await _studentService.UpdateStudentAsync(studentDto);
            return NoContent();
        }

        // DELETE: api/Student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            return NoContent();
        }
        [HttpGet("export-full-excel")]
        public async Task<IActionResult> ExportStudentsFullToExcel()
        {
            var fileBytes = await _studentService.ExportStudentsFullToExcelAsync();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students_Full.xlsx");
        }

        [HttpPost("export-selected-excel")]
        public async Task<IActionResult> ExportStudentsSelectedToExcel([FromBody] List<string> selectedColumns)
        {
            var fileBytes = await _studentService.ExportStudentsSelectedToExcelAsync(selectedColumns);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students_Selected.xlsx");
        }



        [HttpPost("import")]
        public async Task<IActionResult> ImportStudentsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file Excel!");

            await _studentService.ImportStudentsFromExcelAsync(file);
            return Ok("Import danh sách học sinh thành công!");
        }

    }
}