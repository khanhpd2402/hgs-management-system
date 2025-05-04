using Application.Features.LessonPlans.DTOs;
using Application.Features.LessonPlans.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonPlanController : ControllerBase
    {
        private readonly ILessonPlanService _lessonPlanService;

        public LessonPlanController(ILessonPlanService lessonPlanService)
        {
            _lessonPlanService = lessonPlanService ?? throw new ArgumentNullException(nameof(lessonPlanService));
        }

        [HttpPost("create")]
        [Authorize(Roles = "Cán bộ văn thư,Trưởng bộ môn,Hiệu trưởng")]
        public async Task<IActionResult> CreateLessonPlan([FromBody] LessonPlanCreateDto createDto)
        {
            try
            {
                Console.WriteLine("Creating lesson plan...");
                var createdPlan = await _lessonPlanService.CreateLessonPlanAsync(createDto);
                return CreatedAtAction(nameof(GetLessonPlanById), new { planId = createdPlan.PlanId }, createdPlan);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating lesson plan: {ex.Message}");
                return BadRequest("Lỗi khi tạo giáo án.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error creating lesson plan: {ex.Message}");
                return BadRequest("Lỗi khi tạo giáo án.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error creating lesson plan: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo giáo án.");
            }
        }

        [HttpPut("{planId}/update")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn")]
        public async Task<IActionResult> UpdateMyLessonPlan(int planId, [FromBody] LessonPlanUpdateDto updateDto)
        {
            try
            {
                if (planId <= 0)
                {
                    Console.WriteLine("Invalid plan ID.");
                    return BadRequest("Plan ID không hợp lệ.");
                }

                Console.WriteLine("Updating lesson plan...");
                await _lessonPlanService.UpdateMyLessonPlanAsync(planId, updateDto);
                return Ok("Cập nhật giáo án thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating lesson plan: {ex.Message}");
                return NotFound("Không tìm thấy giáo án.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating lesson plan: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật giáo án.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating lesson plan: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật giáo án.");
            }
        }

        [HttpPost("review")]
        [Authorize(Roles = "Cán bộ văn thư,Trưởng bộ môn,Hiệu trưởng")]
        public async Task<IActionResult> ReviewLessonPlan([FromBody] LessonPlanReviewDto reviewDto)
        {
            try
            {
                Console.WriteLine("Reviewing lesson plan...");
                await _lessonPlanService.ReviewLessonPlanAsync(reviewDto);
                return Ok("Duyệt giáo án thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error reviewing lesson plan: {ex.Message}");
                return NotFound("Không tìm thấy giáo án.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized("Không có quyền truy cập.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error reviewing lesson plan: {ex.Message}");
                return BadRequest("Lỗi khi duyệt giáo án.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error reviewing lesson plan: {ex.Message}");
                return BadRequest("Lỗi khi duyệt giáo án.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error reviewing lesson plan: {ex.Message}");
                return StatusCode(500, "Lỗi khi duyệt giáo án.");
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Cán bộ văn thư,Trưởng bộ môn,Hiệu trưởng")]
        public async Task<IActionResult> GetAllLessonPlans([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                Console.WriteLine("Fetching all lesson plans...");
                var (lessonPlans, totalCount) = await _lessonPlanService.GetAllLessonPlansAsync(pageNumber, pageSize);
                return Ok(new { lessonPlans, totalCount, pageNumber, pageSize });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching lesson plans: {ex.Message}");
                return BadRequest("Lỗi khi lấy danh sách giáo án.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching lesson plans: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách giáo án.");
            }
        }

        [HttpGet("teacher/{teacherId}")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> GetLessonPlansByTeacher(int teacherId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (teacherId <= 0)
                {
                    Console.WriteLine("Invalid teacher ID.");
                    return BadRequest("Teacher ID không hợp lệ.");
                }

                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                Console.WriteLine("Fetching lesson plans by teacher...");
                var (lessonPlans, totalCount) = await _lessonPlanService.GetLessonPlansByTeacherAsync(teacherId, pageNumber, pageSize);
                return Ok(new { lessonPlans, totalCount, pageNumber, pageSize });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching lesson plans: {ex.Message}");
                return BadRequest("Lỗi khi lấy danh sách giáo án của giáo viên.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching lesson plans: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách giáo án của giáo viên.");
            }
        }

        [HttpGet("{planId}")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> GetLessonPlanById(int planId)
        {
            try
            {
                if (planId <= 0)
                {
                    Console.WriteLine("Invalid plan ID.");
                    return BadRequest("Plan ID không hợp lệ.");
                }

                Console.WriteLine("Fetching lesson plan...");
                var lessonPlan = await _lessonPlanService.GetLessonPlanByIdAsync(planId);
                return Ok(lessonPlan);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error fetching lesson plan: {ex.Message}");
                return NotFound("Không tìm thấy giáo án.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching lesson plan: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin giáo án.");
            }
        }

        [HttpGet("filter-by-status")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> GetLessonPlansByStatus([FromQuery] string status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                Console.WriteLine("Filtering lesson plans by status...");
                var (lessonPlans, totalCount) = await _lessonPlanService.GetLessonPlansByStatusAsync(status, pageNumber, pageSize);
                return Ok(new { lessonPlans, totalCount, pageNumber, pageSize });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error filtering lesson plans: {ex.Message}");
                return BadRequest("Lỗi khi lọc giáo án theo trạng thái.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error filtering lesson plans: {ex.Message}");
                return BadRequest("Lỗi khi lọc giáo án theo trạng thái.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error filtering lesson plans: {ex.Message}");
                return StatusCode(500, "Lỗi khi lọc giáo án theo trạng thái.");
            }
        }
        [HttpGet("department-head-statistics")]
        [Authorize(Roles = "Trưởng bộ môn")]
        public async Task<IActionResult> GetDepartmentHeadLessonPlanStatistics()
        {
            try
            {
                Console.WriteLine("Fetching lesson plan statistics for department head...");
                var statistics = await _lessonPlanService.GetDepartmentHeadLessonPlanStatisticsAsync();
                return Ok(statistics);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching lesson plan statistics: {ex.Message}");
                return BadRequest("Lỗi khi lấy thống kê giáo án.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching lesson plan statistics: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thống kê giáo án.");
            }
        }
    }
}