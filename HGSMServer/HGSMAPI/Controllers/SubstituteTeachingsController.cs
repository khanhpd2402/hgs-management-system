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

        // POST: api/SubstituteTeachings
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubstituteTeachingCreateDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { substituteId = result.SubstituteId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating substitute teaching: {ex.Message}");
            }
        }

        // GET: api/SubstituteTeachings/5
        [HttpGet("{substituteId}")]
        public async Task<IActionResult> GetById(int substituteId)
        {
            try
            {
                var result = await _service.GetByIdAsync(substituteId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving substitute teaching: {ex.Message}");
            }
        }

        // GET: api/SubstituteTeachings?timetableDetailId=1&teacherId=2&date=2025-04-15
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? timetableDetailId = null, [FromQuery] int? OriginalTeacherId = null, [FromQuery] int? SubstituteTeacherId = null, [FromQuery] DateOnly? date = null)
        {
            try
            {
                var results = await _service.GetAllAsync(timetableDetailId, OriginalTeacherId, SubstituteTeacherId, date);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving substitute teachings: {ex.Message}");
            }
        }

        // PUT: api/SubstituteTeachings/5
        [HttpPut("{substituteId}")]
        public async Task<IActionResult> Update(int substituteId, [FromBody] SubstituteTeachingUpdateDto dto)
        {
            if (substituteId != dto.SubstituteId)
                return BadRequest("SubstituteId mismatch.");

            try
            {
                var result = await _service.UpdateAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating substitute teaching: {ex.Message}");
            }
        }

        // DELETE: api/SubstituteTeachings/5
        [HttpDelete("{substituteId}")]
        public async Task<IActionResult> Delete(int substituteId)
        {
            try
            {
                await _service.DeleteAsync(substituteId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting substitute teaching: {ex.Message}");
            }
        }
    }
}
