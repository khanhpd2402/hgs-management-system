using Application.Features.Classes.DTOs;
using Application.Features.Classes.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassDto>>> GetAll()
        {
            try
            {
                Console.WriteLine("Fetching all classes...");
                var classes = await _classService.GetAllClassesAsync();
                return Ok(classes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching classes: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách lớp học.");
            }
        }

        [HttpGet("GetActiveAll")]
        public async Task<ActionResult<IEnumerable<ClassDto>>> GetActiveAll([FromQuery] string? status = null)
        {
            try
            {
                Console.WriteLine("Fetching active classes...");
                var classes = await _classService.GetAllClassesActiveAsync(status);
                return Ok(classes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching active classes: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách lớp học đang hoạt động.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClassDto>> GetById(int id)
        {
            try
            {
                Console.WriteLine("Fetching class...");
                var classDto = await _classService.GetClassByIdAsync(id);
                if (classDto == null)
                {
                    Console.WriteLine("Class not found.");
                    return NotFound("Không tìm thấy lớp học.");
                }
                return Ok(classDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching class: {ex.Message}");
                return NotFound("Không tìm thấy lớp học.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClassDto>> Create([FromBody] ClassDto classDto)
        {
            try
            {
                if (classDto == null)
                {
                    Console.WriteLine("Class data is null.");
                    return BadRequest("Dữ liệu lớp học không được để trống.");
                }

                Console.WriteLine("Creating class...");
                var createdClass = await _classService.CreateClassAsync(classDto);
                return CreatedAtAction(nameof(GetById), new { id = createdClass.ClassId }, createdClass);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating class: {ex.Message}");
                return BadRequest("Lỗi khi tạo lớp học.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error creating class: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo lớp học.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClassDto>> Update(int id, [FromBody] ClassDto classDto)
        {
            try
            {
                Console.WriteLine("Updating class...");
                var updatedClass = await _classService.UpdateClassAsync(id, classDto);
                if (updatedClass == null)
                {
                    Console.WriteLine("Class not found.");
                    return NotFound("Không tìm thấy lớp học.");
                }
                return Ok(updatedClass);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating class: {ex.Message}");
                return NotFound("Lỗi Khi cập nhật lớp học!");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("Deleting class...");
                await _classService.DeleteClassAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting class: {ex.Message}");
                return NotFound("Không tìm thấy lớp học.");
            }
        }
    }
}