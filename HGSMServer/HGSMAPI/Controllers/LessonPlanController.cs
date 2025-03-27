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
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UploadLessonPlan([FromBody] LessonPlanUploadDto lessonPlanDto)
        {
            try
            {
                await _lessonPlanService.UploadLessonPlanAsync(lessonPlanDto);
                return Ok(new { message = "Lesson plan uploaded successfully.", planId = 1 }); // Cần lấy ID thực tế từ repository
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error uploading lesson plan.", error = ex.Message });
            }
        }

        [HttpPost("review")]
        [Authorize(Roles = "HeadOfDepartment")]
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
    }
}