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
        public async Task<IActionResult> GetAllTeachers()
        {        
                var result = await _teacherService.GetAllTeachersAsync();
                return Ok(result);   
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
            var result = await _teacherService.DeleteTeacherAsync(id);
            if (!result)
                return NotFound("Không tìm thấy giáo viên với ID này"); // 404

            return Ok(new { Message = "Xóa giáo viên thành công" });
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