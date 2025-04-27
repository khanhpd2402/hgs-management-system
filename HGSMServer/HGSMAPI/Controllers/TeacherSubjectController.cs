using Application.Features.TeacherSubjects.DTOs;
using Application.Features.TeacherSubjects.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherSubjectController : ControllerBase
    {
        private readonly ITeacherSubjectService _service;

        public TeacherSubjectController(ITeacherSubjectService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> GetAll()
        {
            Console.WriteLine("Fetching all teacher subjects...");
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                Console.WriteLine("Fetching teacher subject...");
                var result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error fetching teacher subject: {ex.Message}");
                return BadRequest("Không tìm thấy phân công môn học.");
            }
        }

        [HttpGet("by-teacher/{teacherId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Teacher")]
        public async Task<IActionResult> GetByTeacherId(int teacherId)
        {
            try
            {
                Console.WriteLine("Fetching subjects by teacher...");
                var result = await _service.GetByTeacherIdAsync(teacherId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error fetching subjects by teacher: {ex.Message}");
                return BadRequest("Không tìm thấy giáo viên.");
            }
        }

        [HttpGet("by-subject/{subjectId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Teacher")]
        public async Task<IActionResult> GetBySubjectId(int subjectId)
        {
            try
            {
                Console.WriteLine("Fetching teachers by subject...");
                var result = await _service.GetBySubjectIdAsync(subjectId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error fetching teachers by subject: {ex.Message}");
                return BadRequest("Không tìm thấy môn học.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> Create([FromBody] CreateTeacherSubjectDto dto)
        {
            try
            {
                if (dto == null)
                {
                    Console.WriteLine("Teacher subject data is null.");
                    return BadRequest("Dữ liệu phân công môn học không được để trống.");
                }

                Console.WriteLine("Creating teacher subject assignment...");
                await _service.CreateAsync(dto);
                return Ok("Thêm phân công môn học thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating teacher subject: {ex.Message}");
                return BadRequest("Lỗi khi thêm phân công môn học.");
            }
        }

        [HttpPut("by-teacher/{teacherId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateByTeacherId(int teacherId, [FromBody] UpdateTeacherSubjectDto dto)
        {
            try
            {
                if (dto == null)
                {
                    Console.WriteLine("Update teacher subject data is null.");
                    return BadRequest("Dữ liệu cập nhật phân công môn học không được để trống.");
                }
                if (teacherId != dto.TeacherId)
                {
                    Console.WriteLine("Teacher ID mismatch.");
                    return BadRequest("ID giáo viên không khớp.");
                }

                Console.WriteLine("Updating teacher subject assignment...");
                await _service.UpdateAsync(dto);
                return Ok("Cập nhật phân công môn học thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating teacher subject: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật phân công môn học.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("Deleting teacher subject assignment...");
                await _service.DeleteAsync(id);
                return Ok("Xóa phân công môn học thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error deleting teacher subject: {ex.Message}");
                return BadRequest("Không tìm thấy phân công môn học.");
            }
        }

        [HttpDelete("by-teacher/{teacherId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> DeleteByTeacherId(int teacherId)
        {
            try
            {
                Console.WriteLine("Deleting all teacher subject assignments for teacher...");
                await _service.DeleteByTeacherIdAsync(teacherId);
                return Ok("Xóa tất cả phân công môn học của giáo viên thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error deleting teacher subjects: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}