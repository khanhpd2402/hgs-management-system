using Application.Features.LeaveRequests.DTOs;
using Application.Features.LeaveRequests.DTOs.Application.Features.LeaveRequests.DTOs;
using Application.Features.LeaveRequests.Interfaces;
using Application.Features.LeaveRequests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _service;

        public LeaveRequestController(ILeaveRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? teacherId, [FromQuery] string? status)
        {
            var list = await _service.GetAllAsync(teacherId, status);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLeaveRequest dto)
        {
            if (id != dto.RequestId)
                return BadRequest("Mismatched ID");

            var updated = await _service.UpdateAsync(dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        [HttpPost("find-substitute-teachers")]
        //[Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> FindSubstituteTeachers([FromBody] FindSubstituteTeacherRequestDto request)
        {
            var availableTeachers = await _service.FindAvailableSubstituteTeachersAsync(request);
            return Ok(availableTeachers);
        }
        [HttpPost("check-available-teachers")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> CheckAvailableTeachers([FromBody] FindSubstituteTeacherRequestDto request)
        {
            var availableTeachers = await _service.CheckAvailableTeachersAsync(request);
            return Ok(availableTeachers);
        }
    }
}
