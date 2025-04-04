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
        public async Task<IActionResult> Update(int id, GradeBatchToCreateDto gradeBatchDto)
        {
            if (id != gradeBatchDto.GradeBatch.BatchId)
            {
                return BadRequest(new { message = "BatchId không khớp." });
            }

            var result = await _gradeBatchService.UpdateAsync(gradeBatchDto);

            if (result == null)
            {
                return NotFound(new { message = "GradeBatch không tồn tại." });
            }

            return Ok(result);
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
        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetGradeBatchDetail(int id)
        {
            var result = await _gradeBatchService.GetGradeBatchDetailAsync(id);
            return Ok(result);
        }
    }
}

