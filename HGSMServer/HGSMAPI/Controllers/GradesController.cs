using Application.Features.Grades.DTOs;
using Application.Features.Grades.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;

        public GradesController(IGradeService gradesService)
        {
            _gradeService = gradesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGrades(int classId, int subjectId, int semesterId)
        {
            var grades = await _gradeService.GetGradesByClassSubjectSemesterAsync(classId, subjectId, semesterId);
            return Ok(grades);
        }
        [HttpPut("update-multiple-scores")]
        public async Task<IActionResult> UpdateMultipleGrades([FromBody] UpdateMultipleGradesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _gradeService.UpdateMultipleGradesAsync(dto);
            if (!result)
                return NotFound("No grades found to update");

            return Ok("Grades updated successfully");
        }
    }
}
