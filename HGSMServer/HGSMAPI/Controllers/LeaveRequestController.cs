using Application.Features.LeaveRequests.DTOs;
using Application.Features.LeaveRequests.DTOs.Application.Features.LeaveRequests.DTOs;
using Application.Features.LeaveRequests.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            try
            {
                Console.WriteLine("Fetching all leave requests...");
                var list = await _service.GetAllAsync(teacherId, status);
                return Ok(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching leave requests: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách yêu cầu nghỉ phép.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                Console.WriteLine("Fetching leave request...");
                var dto = await _service.GetByIdAsync(id);
                if (dto == null)
                {
                    Console.WriteLine("Leave request not found.");
                    return NotFound("Không tìm thấy yêu cầu nghỉ phép.");
                }
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching leave request: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin yêu cầu nghỉ phép.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
        {
            try
            {
                if (dto == null)
                {
                    Console.WriteLine("Leave request data is null.");
                    return BadRequest("Dữ liệu yêu cầu nghỉ phép không được để trống.");
                }

                Console.WriteLine("Creating leave request...");
                var created = await _service.CreateAsync(dto);
                return Ok(created);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating leave request: {ex.Message}");
                return BadRequest("Lỗi khi tạo yêu cầu nghỉ phép.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLeaveRequest dto)
        {
            try
            {
                if (id != dto.RequestId)
                {
                    Console.WriteLine("Mismatched ID.");
                    return BadRequest("ID không khớp.");
                }

                Console.WriteLine("Updating leave request...");
                var updated = await _service.UpdateAsync(dto);
                if (!updated)
                {
                    Console.WriteLine("Leave request not found.");
                    return NotFound("Không tìm thấy yêu cầu nghỉ phép.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating leave request: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật yêu cầu nghỉ phép.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("Deleting leave request...");
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                {
                    Console.WriteLine("Leave request not found.");
                    return NotFound("Không tìm thấy yêu cầu nghỉ phép.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting leave request: {ex.Message}");
                return StatusCode(500, "Lỗi khi xóa yêu cầu nghỉ phép.");
            }
        }

        [HttpPost("find-substitute-teachers")]
        public async Task<IActionResult> FindSubstituteTeachers([FromBody] FindSubstituteTeacherRequestDto request)
        {
            try
            {
                Console.WriteLine("Finding substitute teachers...");
                var availableTeachers = await _service.FindAvailableSubstituteTeachersAsync(request);
                return Ok(availableTeachers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding substitute teachers: {ex.Message}");
                return BadRequest("Lỗi khi tìm giáo viên dạy thay.");
            }
        }

        [HttpPost("check-available-teachers")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
        public async Task<IActionResult> CheckAvailableTeachers([FromBody] FindSubstituteTeacherRequestDto request)
        {
            try
            {
                Console.WriteLine("Checking available teachers...");
                var availableTeachers = await _service.CheckAvailableTeachersAsync(request);
                return Ok(availableTeachers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking available teachers: {ex.Message}");
                return BadRequest("Lỗi khi kiểm tra giáo viên sẵn có.");
            }
        }
    }
}