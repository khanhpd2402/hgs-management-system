using Application.Features.GradeBatchs.DTOs;
using Application.Features.GradeBatchs.Interfaces;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                Console.WriteLine("Fetching grade batch...");
                var result = await _gradeBatchService.GetByIdAsync(id);
                if (result == null)
                {
                    Console.WriteLine("Grade batch not found.");
                    return NotFound("Không tìm thấy đợt nhập điểm.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grade batch: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin đợt nhập điểm.");
            }
        }

        [HttpGet("by-academicyear/{academicYearId}")]
        public async Task<IActionResult> GetByAcademicYear(int academicYearId)
        {
            try
            {
                Console.WriteLine("Fetching grade batches by academic year...");
                var result = await _gradeBatchService.GetByAcademicYearIdAsync(academicYearId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grade batches: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách đợt nhập điểm theo năm học.");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBatch([FromBody] GradeBatchToCreateDto request)
        {
            try
            {
                if (request == null)
                {
                    Console.WriteLine("Grade batch data is null.");
                    return BadRequest("Dữ liệu đợt nhập điểm không được để trống.");
                }

                Console.WriteLine("Creating grade batch...");
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
                Console.WriteLine($"Error creating grade batch: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo đợt nhập điểm.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGradeBatch(int id, [FromBody] UpdateGradeBatchDto dto)
        {
            try
            {
                Console.WriteLine("Updating grade batch...");
                var updated = await _gradeBatchService.UpdateAsync(id, dto);
                if (updated == null)
                {
                    Console.WriteLine("Grade batch not found.");
                    return NotFound("Không tìm thấy đợt nhập điểm.");
                }
                return Ok(updated);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating grade batch: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật đợt nhập điểm.");
            }
        }
    }
}