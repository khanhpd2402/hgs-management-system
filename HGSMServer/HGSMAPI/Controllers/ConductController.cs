using Application.Features.Conducts.DTOs;
using Application.Features.Conducts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConductController : ControllerBase
    {
        private readonly IConductService _conductService;

        public ConductController(IConductService conductService)
        {
            _conductService = conductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                Console.WriteLine("Fetching all conducts...");
                var conducts = await _conductService.GetAllAsync();
                return Ok(conducts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching conducts: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách hạnh kiểm.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                Console.WriteLine("Fetching conduct...");
                var conduct = await _conductService.GetByIdAsync(id);
                if (conduct == null)
                {
                    Console.WriteLine("Conduct not found.");
                    return NotFound("Không tìm thấy hạnh kiểm.");
                }
                return Ok(conduct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching conduct: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin hạnh kiểm.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateConductDto dto)
        {
            try
            {
                if (dto == null)
                {
                    Console.WriteLine("Conduct data is null.");
                    return BadRequest("Dữ liệu hạnh kiểm không được để trống.");
                }

                Console.WriteLine("Creating conduct...");
                var createdConduct = await _conductService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdConduct.Id }, createdConduct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating conduct: {ex.Message}");
                return BadRequest("Lỗi khi tạo hạnh kiểm.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateConductDto dto)
        {
            try
            {
                Console.WriteLine("Updating conduct...");
                var updatedConduct = await _conductService.UpdateAsync(id, dto);
                if (updatedConduct == null)
                {
                    Console.WriteLine("Conduct not found.");
                    return NotFound("Không tìm thấy hạnh kiểm.");
                }
                return Ok(updatedConduct);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating conduct: {ex.Message}");
                return NotFound("Không tìm thấy hạnh kiểm.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating conduct: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật hạnh kiểm.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("Deleting conduct...");
                var result = await _conductService.DeleteAsync(id);
                if (!result)
                {
                    Console.WriteLine("Conduct not found.");
                    return NotFound("Không tìm thấy hạnh kiểm.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting conduct: {ex.Message}");
                return StatusCode(500, "Lỗi khi xóa hạnh kiểm.");
            }
        }
    }
}