using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Lấy danh sách giáo viên (chỉ thông tin quan trọng)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherListDto>>> GetAllTeachers()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            return Ok(teachers);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của giáo viên theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDetailDto>> GetTeacherById(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
                return NotFound("Không tìm thấy giáo viên");

            return Ok(teacher);
        }

        /// <summary>
        /// Thêm mới một giáo viên
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AddTeacher([FromBody] TeacherDetailDto teacherDto)
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
    }
}