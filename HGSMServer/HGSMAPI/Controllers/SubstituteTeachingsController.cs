using Application.Features.SubstituteTeachings.DTOs;
using Application.Features.SubstituteTeachings.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubstituteTeachingsController : ControllerBase
    {
        private readonly ISubstituteTeachingService _service;

        public SubstituteTeachingsController(ISubstituteTeachingService service)
        {
            _service = service;
        }

        [HttpGet("{substituteId}")]
        public async Task<IActionResult> GetById(int substituteId)
        {
            try
            {
                Console.WriteLine("Fetching substitute teaching...");
                var result = await _service.GetByIdAsync(substituteId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error fetching substitute teaching: {ex.Message}");
                return NotFound("Không tìm thấy phân công dạy thay.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error retrieving substitute teaching: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin phân công dạy thay.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? timetableDetailId = null, [FromQuery] int? OriginalTeacherId = null, [FromQuery] int? SubstituteTeacherId = null, [FromQuery] DateOnly? date = null)
        {
            try
            {
                Console.WriteLine("Fetching all substitute teachings...");
                var results = await _service.GetAllAsync(timetableDetailId, OriginalTeacherId, SubstituteTeacherId, date);
                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error retrieving substitute teachings: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách phân công dạy thay.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] SubstituteTeachingCreateDto dto)
        {
            try
            {
                Console.WriteLine("Creating or updating substitute teaching...");
                var result = await _service.CreateOrUpdateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating or updating substitute teaching: {ex.Message}");
                return BadRequest("Lỗi khi tạo hoặc cập nhật phân công dạy thay.");
            }
        }

        [HttpDelete("{substituteId}")]
        public async Task<IActionResult> Delete(int substituteId)
        {
            try
            {
                Console.WriteLine("Deleting substitute teaching...");
                await _service.DeleteAsync(substituteId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error deleting substitute teaching: {ex.Message}");
                return NotFound("Không tìm thấy phân công dạy thay.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error deleting substitute teaching: {ex.Message}");
                return StatusCode(500, "Lỗi khi xóa phân công dạy thay.");
            }
        }
    }
}