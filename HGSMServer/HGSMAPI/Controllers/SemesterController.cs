using Application.Features.Semesters.DTOs;
using Application.Features.Semesters.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _service;

        public SemesterController(ISemesterService service)
        {
            _service = service;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllSemesters()
        {
            try
            {
                var result = await _service.GetAllSemester();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("by-academic-year/{academicYearId}")]
        public async Task<IActionResult> GetByAcademicYear(int academicYearId)
        {
            var result = await _service.GetByAcademicYearIdAsync(academicYearId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSemesterDto semesterDto)
        {
            await _service.AddAsync(semesterDto);
            return Ok(new { message = "Semester created successfully!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SemesterDto semesterDto)
        {
            if (id != semesterDto.SemesterID) return BadRequest();
            await _service.UpdateAsync(semesterDto);
            return Ok(new { message = "Semester updated successfully!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Semester deleted successfully!" });
        }
    }
}
