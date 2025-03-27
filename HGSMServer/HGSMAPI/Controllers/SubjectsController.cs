using Application.Features.Subjects.DTOs;
using Application.Features.Subjects.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            return Ok(subjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null) return NotFound("Subject not found");
            return Ok(subject);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto createDto)
        {
            if (createDto == null) return BadRequest("Invalid data");

            var createdSubject = await _subjectService.CreateSubjectAsync(createDto);
            return CreatedAtAction(nameof(GetSubjectById), new { id = createdSubject.SubjectId }, createdSubject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjectDto updateDto)
        {
            if (updateDto == null) return BadRequest("Invalid data");

            var updated = await _subjectService.UpdateSubjectAsync(id, updateDto);
            if (!updated) return NotFound("Subject not found");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var deleted = await _subjectService.DeleteSubjectAsync(id);
            if (!deleted) return NotFound("Subject not found");

            return NoContent();
        }
    }
}
