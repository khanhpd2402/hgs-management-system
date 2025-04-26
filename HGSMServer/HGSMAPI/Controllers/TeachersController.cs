using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet]
    [EnableQuery]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> GetAllTeachers()
    {
        var result = await _teacherService.GetAllTeachersAsync();
        return Ok(new ApiResponse(true, "Lấy danh sách giáo viên thành công.", result));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> GetTeacherById(int id)
    {
        try
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
                return NotFound(new ApiResponse(false, $"Không tìm thấy giáo viên với ID {id}."));

            return Ok(new ApiResponse(true, $"Lấy thông tin giáo viên với ID {id} thành công.", teacher));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, ex.Message));
        }
    }

    [HttpGet("{id}/email")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> GetEmailByTeacherId(int id)
    {
        try
        {
            var email = await _teacherService.GetEmailByTeacherIdAsync(id);
            if (string.IsNullOrEmpty(email))
                return NotFound(new ApiResponse(false, $"Không tìm thấy email cho giáo viên với ID {id}."));

            return Ok(new ApiResponse(true, $"Lấy email giáo viên với ID {id} thành công.", new { Email = email }));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(false, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse(false, ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> AddTeacher([FromBody] TeacherListDto teacherDto)
    {
        try
        {
            if (teacherDto == null)
                return BadRequest(new ApiResponse(false, "Dữ liệu giáo viên không được để trống."));

            await _teacherService.AddTeacherAsync(teacherDto);
            return CreatedAtAction(nameof(GetTeacherById), new { id = teacherDto.TeacherId },
                new ApiResponse(true, "Thêm giáo viên thành công."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse(false, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse(false, ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> UpdateTeacher(int id, [FromBody] TeacherDetailDto teacherDto)
    {
        try
        {
            if (teacherDto == null || id != teacherDto.TeacherId)
                return BadRequest(new ApiResponse(false, "Dữ liệu không hợp lệ hoặc ID không khớp."));

            await _teacherService.UpdateTeacherAsync(id, teacherDto);
            var updatedTeacher = await _teacherService.GetTeacherByIdAsync(id);
            if (updatedTeacher == null)
                return NotFound(new ApiResponse(false, $"Không tìm thấy giáo viên với ID {id}."));

            return Ok(new ApiResponse(true, "Cập nhật giáo viên thành công.", updatedTeacher));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(false, ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse(false, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse(false, ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
        try
        {
            var result = await _teacherService.DeleteTeacherAsync(id);
            if (!result)
                return NotFound(new ApiResponse(false, $"Không tìm thấy giáo viên với ID {id} để xóa."));

            return Ok(new ApiResponse(true, $"Xóa giáo viên với ID {id} thành công."));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, ex.Message));
        }
    }

    [HttpPost("import")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> ImportTeachersFromExcel(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse(false, "Vui lòng chọn file Excel!"));

            var (success, errors) = await _teacherService.ImportTeachersFromExcelAsync(file);
            if (!success)
                return BadRequest(new ApiResponse(false, "Lỗi khi nhập giáo viên từ Excel.", errors));

            return Ok(new ApiResponse(true, "Nhập giáo viên từ Excel thành công."));
        }
        catch (Exception ex)
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