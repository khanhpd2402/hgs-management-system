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
        Console.WriteLine("Fetching all teachers...");
        var result = await _teacherService.GetAllTeachersAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> GetTeacherById(int id)
    {
        try
        {
            Console.WriteLine("Fetching teacher...");
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                Console.WriteLine("Teacher not found.");
                return NotFound("Không tìm thấy giáo viên.");
            }

            return Ok(teacher);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching teacher: {ex.Message}");
            return BadRequest("Lỗi khi lấy thông tin giáo viên.");
        }
    }

    [HttpGet("{id}/email")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> GetEmailByTeacherId(int id)
    {
        try
        {
            Console.WriteLine("Fetching teacher email...");
            var email = await _teacherService.GetEmailByTeacherIdAsync(id);
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Email not found for teacher.");
                return NotFound("Không tìm thấy email của giáo viên.");
            }

            return Ok(new { Email = email });
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Error fetching email: {ex.Message}");
            return NotFound("Không tìm thấy email của giáo viên.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error fetching email: {ex.Message}");
            return BadRequest("Lỗi khi lấy email giáo viên.");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> AddTeacher([FromBody] TeacherListDto teacherDto)
    {
        try
        {
            if (teacherDto == null)
            {
                Console.WriteLine("Teacher data is null.");
                return BadRequest("Dữ liệu giáo viên không được để trống.");
            }

            Console.WriteLine("Adding teacher...");
            await _teacherService.AddTeacherAsync(teacherDto);
            return CreatedAtAction(nameof(GetTeacherById), new { id = teacherDto.TeacherId }, "Thêm giáo viên thành công.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error adding teacher: {ex.Message}");
            return BadRequest("Lỗi khi thêm giáo viên.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error adding teacher: {ex.Message}");
            return BadRequest("Lỗi khi thêm giáo viên.");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> UpdateTeacher(int id, [FromBody] TeacherDetailDto teacherDto)
    {
        try
        {
            if (teacherDto == null || id != teacherDto.TeacherId)
            {
                Console.WriteLine("Invalid teacher data or ID mismatch.");
                return BadRequest("Dữ liệu không hợp lệ hoặc ID không khớp.");
            }

            Console.WriteLine("Updating teacher...");
            await _teacherService.UpdateTeacherAsync(id, teacherDto);
            var updatedTeacher = await _teacherService.GetTeacherByIdAsync(id);
            if (updatedTeacher == null)
            {
                Console.WriteLine("Teacher not found after update.");
                return NotFound("Không tìm thấy giáo viên.");
            }

            return Ok("Cập nhật giáo viên thành công.");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Error updating teacher: {ex.Message}");
            return NotFound("Không tìm thấy giáo viên.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error updating teacher: {ex.Message}");
            return BadRequest("Lỗi khi cập nhật giáo viên.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error updating teacher: {ex.Message}");
            return BadRequest("Lỗi khi cập nhật giáo viên.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
        try
        {
            Console.WriteLine("Deleting teacher...");
            var result = await _teacherService.DeleteTeacherAsync(id);
            if (!result)
            {
                Console.WriteLine("Teacher not found for deletion.");
                return NotFound("Không tìm thấy giáo viên để xóa.");
            }

            return Ok("Xóa giáo viên thành công.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting teacher: {ex.Message}");
            return BadRequest("Lỗi khi xóa giáo viên.");
        }
    }

    [HttpPost("import")]
    [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Cán bộ văn thư")]
    public async Task<IActionResult> ImportTeachersFromExcel(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("Excel file is empty or not provided.");
                return BadRequest("Vui lòng chọn file Excel!");
            }

            Console.WriteLine("Importing teachers from Excel...");
            var (success, errors) = await _teacherService.ImportTeachersFromExcelAsync(file);
            if (!success)
            {
                Console.WriteLine("Error importing teachers from Excel.");
                return BadRequest("Lỗi khi nhập giáo viên từ Excel.");
            }

            return Ok("Nhập giáo viên từ Excel thành công.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error importing teachers: {ex.Message}");
            return BadRequest("Lỗi khi nhập giáo viên từ Excel.");
        }
    }
}