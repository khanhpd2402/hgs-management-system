using Application.Features.TeacherSubjects.DTOs;
using Application.Features.TeacherSubjects.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherSubjectController : ControllerBase
    {
        private readonly ITeacherSubjectService _service;

        public TeacherSubjectController(ITeacherSubjectService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new ApiResponse(true, "Lấy danh sách phân công môn học thành công.", result));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                Console.WriteLine("Fetching teacher subject...");
                var result = await _service.GetByIdAsync(id);
                return Ok(new ApiResponse(true, "Lấy thông tin phân công môn học thành công.", result));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error fetching teacher subject: {ex.Message}");
                return BadRequest(new ApiResponse(false, "Không tìm thấy phân công môn học."));
            }
        }

        [HttpGet("by-teacher/{teacherId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Teacher")]
        public async Task<IActionResult> GetByTeacherId(int teacherId)
        {
            try
            {
                Console.WriteLine("Fetching subjects by teacher...");
                var result = await _service.GetByTeacherIdAsync(teacherId);
                return Ok(new ApiResponse(true, "Lấy danh sách môn học của giáo viên thành công.", result));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error fetching subjects by teacher: {ex.Message}");
                return BadRequest(new ApiResponse(false, "Không tìm thấy giáo viên."));
            }
        }

        [HttpGet("by-subject/{subjectId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Teacher")]
        public async Task<IActionResult> GetBySubjectId(int subjectId)
        {
            try
            {
                Console.WriteLine("Fetching teachers by subject...");
                var result = await _service.GetBySubjectIdAsync(subjectId);
                return Ok(new ApiResponse(true, "Lấy danh sách giáo viên dạy môn học thành công.", result));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error fetching teachers by subject: {ex.Message}");
                return BadRequest(new ApiResponse(false, "Không tìm thấy môn học."));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> Create([FromBody] CreateTeacherSubjectDto dto)
        {
            try
            {
                if (dto == null)
                {
                    Console.WriteLine("Teacher subject data is null.");
                    return BadRequest(new ApiResponse(false, "Dữ liệu phân công môn học không được để trống."));
                }

                Console.WriteLine("Creating teacher subject assignment...");
                await _service.CreateAsync(dto);
                return Ok(new ApiResponse(true, "Thêm phân công môn học thành công."));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating teacher subject: {ex.Message}");
                return BadRequest(new ApiResponse(false, "Lỗi khi thêm phân công môn học."));
            }
        }

        [HttpPut("by-teacher/{teacherId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateByTeacherId(int teacherId, [FromBody] UpdateTeacherSubjectDto dto)
        {
            try
            {
                if (dto == null)
                {
                    Console.WriteLine("Update teacher subject data is null.");
                    return BadRequest(new ApiResponse(false, "Dữ liệu cập nhật phân công môn học không được để trống."));
                }
                if (teacherId != dto.TeacherId)
                {
                    Console.WriteLine("Teacher ID mismatch.");
                    return BadRequest(new ApiResponse(false, "ID giáo viên không khớp."));
                }

                Console.WriteLine("Updating teacher subject assignment...");
                await _service.UpdateAsync(dto);
                return Ok(new ApiResponse(true, "Cập nhật phân công môn học thành công."));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating teacher subject: {ex.Message}");
                return BadRequest(new ApiResponse(false, "Lỗi khi cập nhật phân công môn học."));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("Deleting teacher subject assignment...");
                await _service.DeleteAsync(id);
                return Ok(new ApiResponse(true, "Xóa phân công môn học thành công."));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error deleting teacher subject: {ex.Message}");
                return BadRequest(new ApiResponse(false, "Không tìm thấy phân công môn học."));
            }
        }

        public class ApiResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }

            public ApiResponse(bool success, string message, object data = null)
            {
                Success = success;
                Message = message;
                Data = data;
            }
        }
    }
}