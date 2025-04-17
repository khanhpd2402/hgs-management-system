using Application.Features.Classes.DTOs;
using Application.Features.Classes.Interfaces;
using Microsoft.AspNetCore.Http;
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
            var classes = await _classService.GetAllClassesAsync();
            return Ok(classes);
        }
        [HttpGet("GetActiveAll")]
        public async Task<ActionResult<IEnumerable<ClassDto>>> GetActiveAll([FromQuery] string? status = null)
        {
            var classes = await _classService.GetAllClassesActiveAsync(status);
            return Ok(classes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClassDto>> GetById(int id)
        {
            try
            {
                var classDto = await _classService.GetClassByIdAsync(id);
                return Ok(classDto);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClassDto>> Create([FromBody] ClassDto classDto)
        {
            try
            {
                var createdClass = await _classService.CreateClassAsync(classDto);
                return CreatedAtAction(nameof(GetById), new { id = createdClass.ClassId }, createdClass);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClassDto>> Update(int id, [FromBody] ClassDto classDto)
        {
            try
            {
                var updatedClass = await _classService.UpdateClassAsync(id, classDto);
                return Ok(updatedClass);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _classService.DeleteClassAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
