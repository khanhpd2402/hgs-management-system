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

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] SubstituteTeachingCreateDto dto)
        {
            var result = await _service.CreateOrUpdateAsync(dto);
            return Ok(result);
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
