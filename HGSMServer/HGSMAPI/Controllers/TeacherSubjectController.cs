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
                var result = await _service.GetByIdAsync(id);
                return Ok(new ApiResponse(true, $"Lấy thông tin phân công môn học với ID {id} thành công.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(false, ex.Message));
            }
        }

        [HttpGet("by-teacher/{teacherId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Teacher")]
        public async Task<IActionResult> GetByTeacherId(int teacherId)
        {
            try
            {
                var result = await _service.GetByTeacherIdAsync(teacherId);
                return Ok(new ApiResponse(true, $"Lấy danh sách môn học của giáo viên với ID {teacherId} thành công.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(false, ex.Message));
            }
        }

        [HttpGet("by-subject/{subjectId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư,Teacher")]
        public async Task<IActionResult> GetBySubjectId(int subjectId)
        {
            try
            {
                var result = await _service.GetBySubjectIdAsync(subjectId);
                return Ok(new ApiResponse(true, $"Lấy danh sách giáo viên dạy môn học với ID {subjectId} thành công.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(false, ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> Create([FromBody] CreateTeacherSubjectDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new ApiResponse(false, "Dữ liệu phân công môn học không được để trống."));

                await _service.CreateAsync(dto);
                return Ok(new ApiResponse(true, "Thêm phân công môn học thành công."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(false, ex.Message));
            }
        }

        [HttpPut("by-teacher/{teacherId}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateByTeacherId(int teacherId, [FromBody] UpdateTeacherSubjectDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new ApiResponse(false, "Dữ liệu cập nhật phân công môn học không được để trống."));
                if (teacherId != dto.TeacherId)
                    return BadRequest(new ApiResponse(false, "ID giáo viên không khớp."));

                await _service.UpdateAsync(dto);
                return Ok(new ApiResponse(true, "Cập nhật phân công môn học thành công."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(false, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new ApiResponse(true, "Xóa phân công môn học thành công."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(false, ex.Message));
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