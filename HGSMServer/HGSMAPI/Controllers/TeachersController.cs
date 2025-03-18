using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

     
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetAllTeachers([FromQuery] bool exportToExcel = false, [FromQuery] string[] selectedColumns = null)
        {
            try
            {
                var result = await _teacherService.GetAllTeachersAsync(exportToExcel, selectedColumns?.ToList());

                return Ok(result);
            }
            catch (CustomExportException ex)
            {
                return File(ex.ExcelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "teachers.xlsx");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDetailDto>> GetTeacherById(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
                return NotFound("Không tìm thấy giáo viên");

            return Ok(teacher);
        }

        [HttpPost]
        public async Task<ActionResult> AddTeacher([FromBody] TeacherListDto teacherDto)
        {
            if (teacherDto == null)
                return BadRequest("Dữ liệu không hợp lệ");

            await _teacherService.AddTeacherAsync(teacherDto);
            return CreatedAtAction(nameof(GetTeacherById), new { id = teacherDto.TeacherId }, teacherDto);
        }

        /// <summary>
        /// Cập nhật thông tin giáo viên
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTeacher(int id, [FromBody] TeacherDetailDto teacherDto)
        {
            if (teacherDto == null)
                return BadRequest("Dữ liệu không hợp lệ");

            await _teacherService.UpdateTeacherAsync(id, teacherDto);
            return NoContent();
        }

        /// <summary>
        /// Xóa giáo viên theo ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeacher(int id)
        {
            await _teacherService.DeleteTeacherAsync(id);
            return NoContent();
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportTeachersFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file Excel!");

            await _teacherService.ImportTeachersFromExcelAsync(file);
            return Ok("Import thành công!");
        }

    }
}