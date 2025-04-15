using Application.Features.HomeRooms.DTOs;
using Application.Features.TeachingAssignments.DTOs;
using Application.Features.TeachingAssignments.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachingAssignmentController : ControllerBase
    {
        private readonly ITeachingAssignmentService _teachingAssignmentService;

        public TeachingAssignmentController(ITeachingAssignmentService teachingAssignmentService)
        {
            _teachingAssignmentService = teachingAssignmentService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> CreateTeachingAssignments([FromBody] List<TeachingAssignmentCreateDto> dtos)
        {
            try
            {
                await _teachingAssignmentService.CreateTeachingAssignmentsAsync(dtos);
                return Ok("Teaching assignments created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       

        [HttpGet("filter-data")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> GetFilterData()
        {
            try
            {
                var filterData = await _teachingAssignmentService.GetFilterDataAsync();
                return Ok(filterData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("get-assignments-for-creation")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> GetAssignmentsForCreation([FromBody] TeachingAssignmentCreateDto dto)
        {
            try
            {
                var result = await _teachingAssignmentService.GetAssignmentsForCreationAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("teaching-assignments/bulk")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateTeachingAssignments([FromBody] List<TeachingAssignmentUpdateDto> dtos)
        {
            await _teachingAssignmentService.UpdateTeachingAssignmentsAsync(dtos);
            return Ok();
        }

        [HttpGet("all")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Teacher")]
        public async Task<IActionResult> GetAllTeachingAssignments()
        {
            try
            {
                var result = await _teachingAssignmentService.GetAllTeachingAssignmentsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        
    }
}