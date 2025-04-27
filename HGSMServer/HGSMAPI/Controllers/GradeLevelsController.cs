using Application.Features.GradeLevels.DTOs;
using Application.Features.GradeLevels.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeLevelsController : ControllerBase
    {
        private readonly IGradeLevelService _service;

        public GradeLevelsController(IGradeLevelService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GradeLevelDto>>> GetAll()
        {
            try
            {
                Console.WriteLine("Fetching all grade levels...");
                var gradeLevels = await _service.GetAllAsync();
                return Ok(gradeLevels);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grade levels: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách khối lớp.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GradeLevelDto>> GetById(int id)
        {
            try
            {
                Console.WriteLine("Fetching grade level...");
                var gradeLevel = await _service.GetByIdAsync(id);
                if (gradeLevel == null)
                {
                    Console.WriteLine("Grade level not found.");
                    return NotFound("Không tìm thấy khối lớp.");
                }
                return Ok(gradeLevel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grade level: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin khối lớp.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<GradeLevelCreateAndUpdateDto>> Create([FromBody] GradeLevelCreateAndUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Invalid grade level data.");
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                Console.WriteLine("Creating grade level...");
                var createdDto = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdDto.GradeName }, createdDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating grade level: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo khối lớp.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GradeLevelCreateAndUpdateDto>> Update(int id, [FromBody] GradeLevelCreateAndUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Invalid grade level data.");
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                Console.WriteLine("Updating grade level...");
                var updatedDto = await _service.UpdateAsync(id, dto);
                if (updatedDto == null)
                {
                    Console.WriteLine("Grade level not found.");
                    return NotFound("Không tìm thấy khối lớp.");
                }
                return Ok(updatedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating grade level: {ex.Message}");
                if (ex.Message.Contains("not found"))
                {
                    return NotFound("Không tìm thấy khối lớp.");
                }
                return StatusCode(500, "Lỗi khi cập nhật khối lớp.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("Deleting grade level...");
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting grade level: {ex.Message}");
                if (ex.Message.Contains("not found"))
                {
                    return NotFound("Không tìm thấy khối lớp.");
                }
                return StatusCode(500, "Lỗi khi xóa khối lớp.");
            }
        }
    }
}