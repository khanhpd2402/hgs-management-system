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
        // GET: api/GradeBatch/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _gradeBatchService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/GradeBatch/by-academicyear/{academicYearId}
        [HttpGet("by-academicyear/{academicYearId}")]
        public async Task<IActionResult> GetByAcademicYear(int academicYearId)
        {
            var result = await _gradeBatchService.GetByAcademicYearIdAsync(academicYearId);
            return Ok(result);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateBatch([FromBody] GradeBatchToCreateDto request)
        {
            if (request == null) return BadRequest("Request is null.");

            try
            {
                var batchId = await _gradeBatchService.CreateBatchAndInsertGradesAsync(
                    request.BatchName,
                    request.SemesterId,
                    request.StartDate,
                    request.EndDate,
                    request.Status
                );

                return Ok(new { BatchId = batchId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi tạo đợt nhập điểm: {ex.Message}");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGradeBatch(int id, [FromBody] UpdateGradeBatchDto dto)
        {
            var updated = await _gradeBatchService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }
    }
}

