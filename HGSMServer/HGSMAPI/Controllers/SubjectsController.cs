using Application.Features.Subjects.DTOs;
using Application.Features.Subjects.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _service;

        public SubjectsController(ISubjectService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            Console.WriteLine("Fetching all subjects...");
            var subjects = await _service.GetAllAsync();
            return Ok(subjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            Console.WriteLine("Fetching subject...");
            var subject = await _service.GetByIdAsync(id);
            if (subject == null)
            {
                Console.WriteLine("Subject not found.");
                return NotFound("Không tìm thấy môn học.");
            }
            return Ok(subject);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubjectCreateAndUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Invalid subject data.");
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                Console.WriteLine("Creating subject...");
                var createdSubject = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdSubject.SubjectID }, createdSubject);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error creating subject: {ex.Message}");
                return BadRequest("Tên môn học đã tồn tại.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error creating subject: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo môn học.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SubjectCreateAndUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Invalid subject data.");
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                Console.WriteLine("Updating subject...");
                var updatedSubject = await _service.UpdateAsync(id, dto);
                return Ok(updatedSubject);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating subject: {ex.Message}");
                return NotFound("Không tìm thấy môn học.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating subject: {ex.Message}");
                return BadRequest("Tên môn học đã tồn tại.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating subject: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật môn học.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("Deleting subject...");
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error deleting subject: {ex.Message}");
                return NotFound("Không tìm thấy môn học.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error deleting subject: {ex.Message}");
                return StatusCode(500, "Lỗi khi xóa môn học.");
            }
        }
    }
}