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
    }
}

