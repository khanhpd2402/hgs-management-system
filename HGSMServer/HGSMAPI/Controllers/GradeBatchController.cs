using Application.Features.GradeBatchs.DTOs;
using Application.Features.GradeBatchs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeBatchController : ControllerBase
    {
        private readonly IGradeBatchService _gradeBatchService;

        public GradeBatchController(IGradeBatchService gradeBatchService)
        {
            _gradeBatchService = gradeBatchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GradeBatchDto>>> GetAll()
        {
            var batches = await _gradeBatchService.GetAllAsync();
            return Ok(batches);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GradeBatchDto>> GetById(int id)
        {
            var batch = await _gradeBatchService.GetByIdAsync(id);
            if (batch == null)
            {
                return NotFound();
            }
            return Ok(batch);
        }

        [HttpPost]
        public async Task<ActionResult<GradeBatchDto>> Create([FromBody] GradeBatchToCreateDto request)
        {
            var createdBatch = await _gradeBatchService.CreateAsync(request.GradeBatch, request.SubjectIds, request.AssessmentTypes);
            return CreatedAtAction(nameof(GetById), new { id = createdBatch.BatchId }, createdBatch);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GradeBatchDto gradeBatchDto)
        {
            if (id != gradeBatchDto.BatchId)
            {
                return BadRequest();
            }
            var updated = await _gradeBatchService.UpdateAsync(gradeBatchDto);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _gradeBatchService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}

