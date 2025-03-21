using Application.Features.AcademicYears.DTOs;
using Application.Features.AcademicYears.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicYearController : ControllerBase
    {
        private readonly IAcademicYearService _service;

        public AcademicYearController(IAcademicYearService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAcademicYearDto academicYearDto)
        {
            await _service.AddAsync(academicYearDto);
            return Ok(new { message = "Academic Year created successfully!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AcademicYearDto academicYearDto)
        {
            if (id != academicYearDto.AcademicYearID) return BadRequest();
            await _service.UpdateAsync(academicYearDto);
            return Ok(new { message = "Academic Year updated successfully!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Academic Year deleted successfully!" });
        }
    }
}
