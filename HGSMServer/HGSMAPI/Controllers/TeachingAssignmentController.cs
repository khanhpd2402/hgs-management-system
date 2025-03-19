using Application.Features.TeachingAssignment.DTOs;
using Application.Features.TeachingAssignment.Interfaces;
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
        [Authorize(Roles = "Principal,VicePrincipal,HeadOfDepartment,AdministrativeOfficer")]
        public async Task<IActionResult> CreateTeachingAssignment([FromBody] TeachingAssignmentCreateDto dto)
        {
            try
            {
                await _teachingAssignmentService.CreateTeachingAssignmentAsync(dto);
                return Ok("Teaching assignments created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("filter-data")]
        [Authorize(Roles = "Principal,VicePrincipal,HeadOfDepartment,AdministrativeOfficer")]
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
        [Authorize(Roles = "Principal,VicePrincipal,HeadOfDepartment,AdministrativeOfficer")]
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

        [HttpGet("search")]
        [Authorize(Roles = "Principal,VicePrincipal,HeadOfDepartment,AdministrativeOfficer,Teacher")]
        public async Task<IActionResult> SearchTeachingAssignments([FromQuery] TeachingAssignmentFilterDto filter)
        {
            try
            {
                var result = await _teachingAssignmentService.SearchTeachingAssignmentsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
