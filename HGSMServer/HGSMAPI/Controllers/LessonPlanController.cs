using Application.Features.LessonPlans.DTOs;
using Application.Features.LessonPlans.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("upload")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn")]
        //[Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> UploadLessonPlan([FromBody] LessonPlanUploadDto lessonPlanDto)
        {
            try
            {
                await _lessonPlanService.UploadLessonPlanAsync(lessonPlanDto);
                return Ok(new { message = "Lesson plan uploaded successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error uploading lesson plan.", error = ex.Message });
            }
        }


        [HttpPost("review")]
        [Authorize(Roles = "Trưởng bộ môn,Hiệu trưởng")]
        public async Task<IActionResult> ReviewLessonPlan([FromBody] LessonPlanReviewDto reviewDto)
        {
            try
            {
                await _lessonPlanService.ReviewLessonPlanAsync(reviewDto);
                return Ok(new { message = "Lesson plan reviewed successfully.", planId = reviewDto.PlanId, status = reviewDto.Status });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error reviewing lesson plan.", error = ex.Message });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng")]
        public async Task<IActionResult> GetAllLessonPlans([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (lessonPlans, totalCount) = await _lessonPlanService.GetAllLessonPlansAsync(pageNumber, pageSize);
                return Ok(new { lessonPlans, totalCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving lesson plans.", error = ex.Message });
            }
        }

        [HttpGet("{planId}")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng")] 
        public async Task<IActionResult> GetLessonPlanById(int planId)
        {
            try
            {
                var lessonPlan = await _lessonPlanService.GetLessonPlanByIdAsync(planId);
                return Ok(lessonPlan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving lesson plan.", error = ex.Message });
            }
        }

        [HttpGet("filter-by-status")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng")]
        public async Task<IActionResult> GetLessonPlansByStatus([FromQuery] string status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (lessonPlans, totalCount) = await _lessonPlanService.GetLessonPlansByStatusAsync(status, pageNumber, pageSize);
                return Ok(new { lessonPlans, totalCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error filtering lesson plans.", error = ex.Message });
            }
        }
    }
}