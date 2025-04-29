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
                Console.WriteLine("Creating teaching assignments...");
                await _teachingAssignmentService.CreateTeachingAssignmentsAsync(dtos);
                return Ok("Phân công giảng dạy thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating teaching assignments: {ex.Message}");
                return BadRequest($"Lỗi khi phân công giảng dạy: {ex.Message}");
            }
        }

        [HttpGet("filter-data")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> GetFilterData()
        {
            try
            {
                Console.WriteLine("Fetching filter data...");
                var filterData = await _teachingAssignmentService.GetFilterDataAsync();
                return Ok(filterData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching filter data: {ex.Message}");
                return BadRequest($"Lỗi khi lấy dữ liệu lọc: {ex.Message}");
            }
        }

        [HttpPut("teacher/{teacherId}/semester/{semesterId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateTeachingAssignments(int teacherId, int semesterId, [FromBody] List<TeachingAssignmentUpdateDto> dtos)
        {
            try
            {
                Console.WriteLine("Updating teaching assignments...");
                await _teachingAssignmentService.UpdateTeachingAssignmentsAsync(teacherId, semesterId, dtos);
                return Ok("Cập nhật phân công giảng dạy thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating teaching assignments: {ex.Message}");
                return BadRequest($"Lỗi khi cập nhật phân công giảng dạy: {ex.Message}");
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Giáo viên")]
        public async Task<IActionResult> GetAllTeachingAssignments([FromQuery] int semesterId)
        {
            try
            {
                Console.WriteLine("Fetching all teaching assignments...");
                var result = await _teachingAssignmentService.GetAllTeachingAssignmentsAsync(semesterId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all teaching assignments: {ex.Message}");
                return BadRequest($"Lỗi khi lấy danh sách phân công giảng dạy: {ex.Message}");
            }
        }

        [HttpGet("teacher/{teacherId}/semester/{semesterId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Giáo viên")]
        public async Task<IActionResult> GetTeachingAssignmentsByTeacherId(int teacherId, int semesterId)
        {
            try
            {
                Console.WriteLine("Fetching teaching assignments by teacher...");
                var result = await _teachingAssignmentService.GetTeachingAssignmentsByTeacherIdAsync(teacherId, semesterId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching teaching assignments: {ex.Message}");
                return BadRequest($"Lỗi khi lấy phân công giảng dạy: {ex.Message}");
            }
        }

        [HttpDelete("teacher/{teacherId}/semester/{semesterId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Giáo viên")]
        public async Task<IActionResult> DeleteTeachingAssignmentsByTeacherIdAndSemesterId(int teacherId, int semesterId)
        {
            try
            {
                Console.WriteLine("Deleting teaching assignments...");
                await _teachingAssignmentService.DeleteTeachingAssignmentsByTeacherIdAndSemesterIdAsync(teacherId, semesterId);
                return Ok("Xóa phân công giảng dạy thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting teaching assignments: {ex.Message}");
                return BadRequest($"Lỗi khi xóa phân công giảng dạy: {ex.Message}");
            }
        }
    }
}