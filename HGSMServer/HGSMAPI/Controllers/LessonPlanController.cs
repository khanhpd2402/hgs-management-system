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
                // Trả về thông tin giáo án vừa tạo và đường dẫn để truy cập nó
                return CreatedAtAction(nameof(GetLessonPlanById), new { planId = createdPlan.PlanId }, createdPlan);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = "Invalid input for creating lesson plan.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception ex details here
                return StatusCode(500, new { message = "An error occurred while creating the lesson plan.", error = ex.Message });
            }
        }

        [HttpPut("{planId}/update")]
        [Authorize(Roles = "Giáo viên")] 
        public async Task<IActionResult> UpdateMyLessonPlan(int planId, [FromBody] LessonPlanUpdateDto updateDto)
        {
            if (planId <= 0)
            {
                return BadRequest(new { message = "Invalid Plan ID." });
            }
            try
            {
                await _lessonPlanService.UpdateMyLessonPlanAsync(planId, updateDto);
                return Ok(new { message = "Lesson plan updated successfully.", planId = planId });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = "Lesson plan not found.", error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = "Cannot update lesson plan at this time.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception ex details here
                return StatusCode(500, new { message = "An error occurred while updating the lesson plan.", error = ex.Message });
            }
        }


        [HttpPost("review")]
        [Authorize(Roles = "Cán bộ văn thư,Trưởng bộ môn,Hiệu trưởng")]
        public async Task<IActionResult> ReviewLessonPlan([FromBody] LessonPlanReviewDto reviewDto)
        {
            try
            {
                await _lessonPlanService.ReviewLessonPlanAsync(reviewDto);
                return Ok(new { message = "Lesson plan reviewed successfully.", planId = reviewDto.PlanId, status = reviewDto.Status });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = "Lesson plan not found.", error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = "Invalid input for reviewing lesson plan.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception ex details here
                return StatusCode(500, new { message = "Error reviewing lesson plan.", error = ex.Message });
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
            catch (Exception ex)
            {
                // Log the exception ex details here
                return StatusCode(500, new { message = "Error retrieving lesson plans.", error = ex.Message });
            }
        }

        [HttpGet("teacher/{teacherId}")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng, Cán bộ văn thư")]
        public async Task<IActionResult> GetLessonPlansByTeacher(int teacherId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (teacherId <= 0) return BadRequest("Invalid Teacher ID.");
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            // Optional: Add logic to ensure a teacher can only view their own plans unless they are admin/head
            // var currentTeacherId = GetCurrentTeacherIdFromClaims(); // Helper needed
            // if (User.IsInRole("Giáo viên") && currentTeacherId != teacherId) return Forbid();

            try
            {
                var (lessonPlans, totalCount) = await _lessonPlanService.GetLessonPlansByTeacherAsync(teacherId, pageNumber, pageSize);
                return Ok(new { lessonPlans, totalCount, pageNumber, pageSize });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving lesson plans for the teacher.", error = ex.Message });
            }
        }


        [HttpGet("{planId}")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng, Cán bộ văn thư")]
        public async Task<IActionResult> GetLessonPlanById(int planId)
        {
            if (planId <= 0) return BadRequest("Invalid Plan ID.");

            try
            {
                var lessonPlan = await _lessonPlanService.GetLessonPlanByIdAsync(planId);
                // Optional: Check if the current user is authorized to view this specific plan
                // var currentTeacherId = GetCurrentTeacherIdFromClaims();
                // if (User.IsInRole("Giáo viên") && lessonPlan.TeacherId != currentTeacherId) return Forbid();
                return Ok(lessonPlan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = "Lesson plan not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception ex details here
                return StatusCode(500, new { message = "Error retrieving lesson plan.", error = ex.Message });
            }
        }

        [HttpGet("filter-by-status")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn,Hiệu trưởng, Cán bộ văn thư")]
        public async Task<IActionResult> GetLessonPlansByStatus([FromQuery] string status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var (lessonPlans, totalCount) = await _lessonPlanService.GetLessonPlansByStatusAsync(status, pageNumber, pageSize);
                // Optional: Filter further based on user role (e.g., teachers only see their own plans with this status)
                return Ok(new { lessonPlans, totalCount, pageNumber, pageSize });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = "Invalid status filter.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception ex details here
                return StatusCode(500, new { message = "Error filtering lesson plans.", error = ex.Message });
            }
        }

    }
}