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
                var gradeLevelSubjects = await _service.GetBySubjectIdAsync(subjectId);
                if (gradeLevelSubjects == null || !gradeLevelSubjects.Any())
                {
                    return NotFound($"No GradeLevelSubjects found for Subject ID {subjectId}");
                }
                return Ok(gradeLevelSubjects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // GET: api/GradeLevelSubjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GradeLevelSubjectDto>>> GetAll()
        {
            try
            {
                var gradeLevelSubjects = await _service.GetAllAsync();
                return Ok(gradeLevelSubjects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/GradeLevelSubjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GradeLevelSubjectDto>> GetById(int id)
        {
            try
            {
                var gradeLevelSubject = await _service.GetByIdAsync(id);
                if (gradeLevelSubject == null)
                {
                    return NotFound($"GradeLevelSubject with ID {id} not found");
                }
                return Ok(gradeLevelSubject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/GradeLevelSubjects
        [HttpPost]
        public async Task<ActionResult<GradeLevelSubjectCreateAndUpdateDto>> Create([FromBody] GradeLevelSubjectCreateAndUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdDto = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById),
                    new { id = createdDto.GradeLevelId }, // Giả sử muốn trả về ID chính
                    createdDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/GradeLevelSubjects/5
        [HttpPut("{id}")]
        public async Task<ActionResult<GradeLevelSubjectCreateAndUpdateDto>> Update(int id, [FromBody] GradeLevelSubjectCreateAndUpdateDto dto)
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

        // DELETE: api/GradeLevelSubjects/5
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