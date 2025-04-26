using Application.Features.LessonPlans.DTOs;
using Application.Features.LessonPlans.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
                var createdPlan = await _lessonPlanService.CreateLessonPlanAsync(createDto);
                return CreatedAtAction(nameof(GetLessonPlanById), new { planId = createdPlan.PlanId }, new { message = "Tạo giáo án thành công!", plan = createdPlan });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình tạo giáo án." });
            }
        }

        [HttpPut("{planId}/update")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn")]
        public async Task<IActionResult> UpdateMyLessonPlan(int planId, [FromBody] LessonPlanUpdateDto updateDto)
        {
            if (planId <= 0)
            {
                return BadRequest(new { message = "Plan ID không hợp lệ." });
            }

            try
            {
                await _lessonPlanService.UpdateMyLessonPlanAsync(planId, updateDto);
                return Ok(new { message = "Cập nhật giáo án thành công.", planId });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình cập nhật giáo án." });
            }
        }

        [HttpPost("review")]
        [Authorize(Roles = "Cán bộ văn thư,Trưởng bộ môn,Hiệu trưởng")]
        public async Task<IActionResult> ReviewLessonPlan([FromBody] LessonPlanReviewDto reviewDto)
        {
            try
            {
                await _lessonPlanService.ReviewLessonPlanAsync(reviewDto);
                return Ok(new { message = "Duyệt giáo án thành công.", planId = reviewDto.PlanId, status = reviewDto.Status });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình duyệt giáo án." });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Cán bộ văn thư,Trưởng bộ môn,Hiệu trưởng")]
        public async Task<IActionResult> GetAllLessonPlans([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var (lessonPlans, totalCount) = await _lessonPlanService.GetAllLessonPlansAsync(pageNumber, pageSize);
                return Ok(new { lessonPlans, totalCount, pageNumber, pageSize });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy danh sách giáo án." });
            }
        }

        [HttpGet("teacher/{teacherId}")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> GetLessonPlansByTeacher(int teacherId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (teacherId <= 0)
            {
                return BadRequest(new { message = "Teacher ID không hợp lệ." });
            }

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var (lessonPlans, totalCount) = await _lessonPlanService.GetLessonPlansByTeacherAsync(teacherId, pageNumber, pageSize);
                return Ok(new { lessonPlans, totalCount, pageNumber, pageSize });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy danh sách giáo án của giáo viên." });
            }
        }

        [HttpGet("{planId}")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> GetLessonPlanById(int planId)
        {
            if (planId <= 0)
            {
                return BadRequest(new { message = "Plan ID không hợp lệ." });
            }

            try
            {
                var lessonPlan = await _lessonPlanService.GetLessonPlanByIdAsync(planId);
                return Ok(lessonPlan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy thông tin giáo án." });
            }
        }

        [HttpGet("filter-by-status")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> GetLessonPlansByStatus([FromQuery] string status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var (lessonPlans, totalCount) = await _lessonPlanService.GetLessonPlansByStatusAsync(status, pageNumber, pageSize);
                return Ok(new { lessonPlans, totalCount, pageNumber, pageSize });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lọc giáo án theo trạng thái." });
            }
        }
    }
}