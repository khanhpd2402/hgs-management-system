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
            _studentService = studentService;
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
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto createStudentDto)
        {
            if (createStudentDto == null) return BadRequest();

            var studentId = await _studentService.AddStudentAsync(createStudentDto);

            return CreatedAtAction(nameof(GetStudent), new { id = studentId }, createStudentDto);
        }

        // PUT: api/Student/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto updateStudentDto)
        {
            if (id != updateStudentDto.StudentId) return BadRequest();

            await _studentService.UpdateStudentAsync(updateStudentDto);
            return NoContent();
        }

        // DELETE: api/Student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            return NoContent();
        }
        //[HttpGet("export-full-excel")]
        //public async Task<IActionResult> ExportStudentsFullToExcel()
        //{
        //    var fileBytes = await _studentService.ExportStudentsFullToExcelAsync();
        //    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students_Full.xlsx");
        //}

        //[HttpPost("export-selected-excel")]
        //public async Task<IActionResult> ExportStudentsSelectedToExcel([FromBody] List<string> selectedColumns)
        //{
        //    var fileBytes = await _studentService.ExportStudentsSelectedToExcelAsync(selectedColumns);
        //    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students_Selected.xlsx");
        //}



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