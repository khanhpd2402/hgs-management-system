using Application.Features.GradeLevels.DTOs;
using Application.Features.GradeLevels.Interfaces;
using Microsoft.AspNetCore.Http;
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

        // GET: api/GradeLevels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GradeLevelDto>>> GetAll()
        {
            try
            {
                var gradeLevels = await _service.GetAllAsync();
                return Ok(gradeLevels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/GradeLevels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GradeLevelDto>> GetById(int id)
        {
            try
            {
                var gradeLevel = await _service.GetByIdAsync(id);
                if (gradeLevel == null)
                {
                    return NotFound($"GradeLevel with ID {id} not found");
                }
                return Ok(gradeLevel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/GradeLevels
        [HttpPost]
        public async Task<ActionResult<GradeLevelCreateAndUpdateDto>> Create([FromBody] GradeLevelCreateAndUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdDto = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdDto.GradeName }, createdDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/GradeLevels/5
        [HttpPut("{id}")]
        public async Task<ActionResult<GradeLevelCreateAndUpdateDto>> Update(int id, [FromBody] GradeLevelCreateAndUpdateDto dto)
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
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(ex.Message);
                }
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/GradeLevels/5
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
