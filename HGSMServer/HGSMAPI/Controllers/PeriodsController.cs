using Application.Features.Periods.DTOs;
using Application.Features.Periods.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodsController : ControllerBase
    {
        private readonly IPeriodService _service;

        public PeriodsController(IPeriodService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PeriodDto>>> GetAll()
        {
            try
            {
                Console.WriteLine("Fetching all periods...");
                var periods = await _service.GetAllAsync();
                return Ok(periods);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching periods: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách tiết học.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PeriodDto>> GetById(int id)
        {
            try
            {
                Console.WriteLine("Fetching period...");
                var period = await _service.GetByIdAsync(id);
                if (period == null)
                {
                    Console.WriteLine("Period not found.");
                    return NotFound("Không tìm thấy tiết học.");
                }
                return Ok(period);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching period: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin tiết học.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PeriodCreateAndUpdateDto>> Create([FromBody] PeriodCreateAndUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Invalid period data.");
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                Console.WriteLine("Creating period...");
                var createdDto = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdDto.PeriodName }, createdDto);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating period: {ex.Message}");
                return BadRequest("Lỗi khi tạo tiết học.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error creating period: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo tiết học.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PeriodCreateAndUpdateDto>> Update(int id, [FromBody] PeriodCreateAndUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Invalid period data.");
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                Console.WriteLine("Updating period...");
                var updatedDto = await _service.UpdateAsync(id, dto);
                if (updatedDto == null)
                {
                    Console.WriteLine("Period not found.");
                    return NotFound("Không tìm thấy tiết học.");
                }
                return Ok(updatedDto);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating period: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật tiết học.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating period: {ex.Message}");
                if (ex.Message.Contains("not found"))
                {
                    return NotFound("Không tìm thấy tiết học.");
                }
                return StatusCode(500, "Lỗi khi cập nhật tiết học.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("Deleting period...");
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting period: {ex.Message}");
                if (ex.Message.Contains("not found"))
                {
                    return NotFound("Không tìm thấy tiết học.");
                }
                return StatusCode(500, "Lỗi khi xóa tiết học.");
            }
        }
    }
}