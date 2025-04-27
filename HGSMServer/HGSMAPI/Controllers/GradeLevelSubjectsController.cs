using Application.Features.GradeLevelSubjects.DTOs;
using Application.Features.GradeLevelSubjects.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeLevelSubjectsController : ControllerBase
    {
        private readonly IGradeLevelSubjectService _service;

        public GradeLevelSubjectsController(IGradeLevelSubjectService service)
        {
            _service = service;
        }

        [HttpGet("subject/{subjectId}")]
        public async Task<ActionResult<IEnumerable<GradeLevelSubjectDto>>> GetBySubjectId(int subjectId)
        {
            try
            {
                Console.WriteLine("Fetching grade level subjects by subject...");
                var gradeLevelSubjects = await _service.GetBySubjectIdAsync(subjectId);
                if (gradeLevelSubjects == null || !gradeLevelSubjects.Any())
                {
                    Console.WriteLine("No grade level subjects found.");
                    return NotFound("Không tìm thấy môn học theo khối lớp.");
                }
                return Ok(gradeLevelSubjects);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grade level subjects: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách môn học theo khối lớp.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GradeLevelSubjectDto>>> GetAll()
        {
            try
            {
                Console.WriteLine("Fetching all grade level subjects...");
                var gradeLevelSubjects = await _service.GetAllAsync();
                return Ok(gradeLevelSubjects);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grade level subjects: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách môn học theo khối lớp.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GradeLevelSubjectDto>> GetById(int id)
        {
            try
            {
                Console.WriteLine("Fetching grade level subject...");
                var gradeLevelSubject = await _service.GetByIdAsync(id);
                if (gradeLevelSubject == null)
                {
                    Console.WriteLine("Grade level subject not found.");
                    return NotFound("Không tìm thấy môn học theo khối lớp.");
                }
                return Ok(gradeLevelSubject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grade level subject: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin môn học theo khối lớp.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<GradeLevelSubjectCreateAndUpdateDto>> Create([FromBody] GradeLevelSubjectCreateAndUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Invalid grade level subject data.");
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                Console.WriteLine("Creating grade level subject...");
                var createdDto = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdDto.GradeLevelId }, createdDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating grade level subject: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo môn học theo khối lớp.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GradeLevelSubjectCreateAndUpdateDto>> Update(int id, [FromBody] GradeLevelSubjectCreateAndUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Invalid grade level subject data.");
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                Console.WriteLine("Updating grade level subject...");
                var updatedDto = await _service.UpdateAsync(id, dto);
                if (updatedDto == null)
                {
                    Console.WriteLine("Grade level subject not found.");
                    return NotFound("Không tìm thấy môn học theo khối lớp.");
                }
                return Ok(updatedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating grade level subject: {ex.Message}");
                if (ex.Message.Contains("not found"))
                {
                    return NotFound("Không tìm thấy môn học theo khối lớp.");
                }
                return StatusCode(500, "Lỗi khi cập nhật môn học theo khối lớp.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("Deleting grade level subject...");
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting grade level subject: {ex.Message}");
                if (ex.Message.Contains("not found"))
                {
                    return NotFound("Không tìm thấy môn học theo khối lớp.");
                }
                return StatusCode(500, "Lỗi khi xóa môn học theo khối lớp.");
            }
        }
    }
}