using Application.Features.Periods.DTOs;
using Application.Features.Periods.Interfaces;
using Microsoft.AspNetCore.Http;
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

        // GET: api/Periods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PeriodDto>>> GetAll()
        {
            try
            {
                var periods = await _service.GetAllAsync();
                return Ok(periods);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Periods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PeriodDto>> GetById(int id)
        {
            try
            {
                var period = await _service.GetByIdAsync(id);
                if (period == null)
                {
                    return NotFound($"Period with ID {id} not found");
                }
                return Ok(period);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Periods
        [HttpPost]
        public async Task<ActionResult<PeriodCreateAndUpdateDto>> Create([FromBody] PeriodCreateAndUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdDto = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdDto.PeriodName }, createdDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Periods/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PeriodCreateAndUpdateDto>> Update(int id, [FromBody] PeriodCreateAndUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedDto = await _service.UpdateAsync(id, dto);
                return Ok(updatedDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(ex.Message);
                }
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Periods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(ex.Message);
                }
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
