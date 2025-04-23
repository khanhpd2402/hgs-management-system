using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

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
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<ActionResult<TeacherDetailDto>> GetTeacherById(int id)
    {
        var teacher = await _teacherService.GetTeacherByIdAsync(id);
        if (teacher == null) return NotFound(new { Message = "Không tìm thấy giáo viên" });
        return Ok(teacher);
    }

    [HttpGet("{id}/email")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> GetEmailByTeacherId(int id)
    {
        try
        {
            var email = await _teacherService.GetEmailByTeacherIdAsync(id);
            if (string.IsNullOrEmpty(email))
            {
                return NotFound(new { Message = $"Không tìm thấy email cho giáo viên với ID {id}." });
            }
            return Ok(new { Email = email });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = $"Lỗi khi lấy email: {ex.Message}" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<ActionResult> AddTeacher([FromBody] TeacherListDto teacherDto)
    {
        if (teacherDto == null) return BadRequest("Dữ liệu không hợp lệ");
        await _teacherService.AddTeacherAsync(teacherDto);
        return CreatedAtAction(nameof(GetTeacherById), new { id = teacherDto.TeacherId }, teacherDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeacher(int id, [FromBody] TeacherDetailDto teacherDto)
    {
        await _teacherService.UpdateTeacherAsync(id, teacherDto);

        var updatedTeacher = await _teacherService.GetTeacherByIdAsync(id);
        if (updatedTeacher == null)
        {
            return NotFound();
        }

        return Ok(updatedTeacher);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
        var result = await _teacherService.DeleteTeacherAsync(id);
        if (!result)
        {
            return NotFound(new { Message = $"Không tìm thấy giáo viên với ID {id} để xóa." });
        }

        return Ok(new { Message = $"Giáo viên với ID {id} và tài khoản User liên quan đã được xóa thành công." });
    }

    [HttpPost("import")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> ImportTeachersFromExcel(IFormFile file)
    {
        var (success, errors) = await _teacherService.ImportTeachersFromExcelAsync(file);
        if (!success)
        {
            return BadRequest(new { Errors = errors });
        }

        return Ok(new { Message = "Import giáo viên thành công." });
    }
}