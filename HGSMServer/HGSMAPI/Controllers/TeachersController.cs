using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;
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
    public async Task<IActionResult> GetAllTeachers()
    {
        var result = await _teacherService.GetAllTeachersAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeacherDetailDto>> GetTeacherById(int id)
    {
        var teacher = await _teacherService.GetTeacherByIdAsync(id);
        if (teacher == null) return NotFound("Không tìm thấy giáo viên");
        return Ok(teacher);
    }

    [HttpPost]
    public async Task<ActionResult> AddTeacher([FromBody] TeacherListDto teacherDto)
    {
        if (teacherDto == null) return BadRequest("Dữ liệu không hợp lệ");
        await _teacherService.AddTeacherAsync(teacherDto);
        return CreatedAtAction(nameof(GetTeacherById), new { id = teacherDto.TeacherId }, teacherDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTeacher(int id, [FromBody] TeacherDetailDto teacherDto)
    {
        if (teacherDto == null) return BadRequest("Dữ liệu không hợp lệ");
        await _teacherService.UpdateTeacherAsync(id, teacherDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTeacher(int id)
    {
        var result = await _teacherService.DeleteTeacherAsync(id);
        if (!result) return NotFound("Không tìm thấy giáo viên với ID này");
        return Ok(new { Message = "Xóa giáo viên thành công" });
    }

    
    [HttpPost("import")]
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