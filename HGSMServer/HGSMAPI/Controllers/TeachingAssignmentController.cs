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
                return Ok("Phân công giảng dạy thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating teaching assignments: {ex.Message}");
                return BadRequest("Lỗi khi phân công giảng dạy.");
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
                Console.WriteLine($"Error fetching filter data: {ex.Message}");
                return BadRequest("Lỗi khi lấy dữ liệu lọc.");
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
                Console.WriteLine($"Error fetching assignments for creation: {ex.Message}");
                return BadRequest("Lỗi khi lấy dữ liệu phân công giảng dạy.");
            }
        }

        [HttpPut("teaching-assignments")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateTeachingAssignments([FromBody] List<TeachingAssignmentUpdateDto> dtos)
        {
            try
            {
                await _teachingAssignmentService.UpdateTeachingAssignmentsAsync(dtos);
                return Ok("Cập nhật phân công giảng dạy thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating teaching assignments: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật phân công giảng dạy.");
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Giáo viên")]
        public async Task<IActionResult> GetAllTeachingAssignments([FromQuery] int semesterId)
        {
            try
            {
                var result = await _teachingAssignmentService.GetAllTeachingAssignmentsAsync(semesterId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all teaching assignments: {ex.Message}");
                return BadRequest("Lỗi khi lấy danh sách phân công giảng dạy.");
            }
        }

        [HttpGet("teacher/{teacherId}/semester/{semesterId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Giáo viên")]
        public async Task<IActionResult> GetTeachingAssignmentsByTeacherId(int teacherId, int semesterId)
        {
            try
            {
                var result = await _teachingAssignmentService.GetTeachingAssignmentsByTeacherIdAsync(teacherId, semesterId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching teaching assignments: {ex.Message}");
                return BadRequest("Lỗi khi lấy phân công giảng dạy.");
            }
        }

        [HttpDelete("teacher/{teacherId}/semester/{semesterId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Giáo viên")]
        public async Task<IActionResult> DeleteTeachingAssignmentsByTeacherIdAndSemesterId(int teacherId, int semesterId)
        {
            try
            {
                await _teachingAssignmentService.DeleteTeachingAssignmentsByTeacherIdAndSemesterIdAsync(teacherId, semesterId);
                return Ok("Xóa phân công giảng dạy thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting teaching assignments: {ex.Message}");
                return BadRequest("Lỗi khi xóa phân công giảng dạy.");
            }
        }
    }
}