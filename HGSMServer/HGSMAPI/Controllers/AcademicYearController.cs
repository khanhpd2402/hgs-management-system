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
            // Validate dữ liệu đầu vào
            if (academicYearDto.StartDate >= academicYearDto.EndDate)
            {
                return BadRequest("Ngày bắt đầu năm học phải trước ngày kết thúc.");
            }

            if (academicYearDto.Semester1EndDate < academicYearDto.StartDate || academicYearDto.Semester1EndDate > academicYearDto.EndDate)
            {
                return BadRequest("Ngày kết thúc của Học kỳ 1 phải nằm trong khoảng thời gian của năm học.");
            }

            if (academicYearDto.Semester2StartDate < academicYearDto.StartDate || academicYearDto.Semester2StartDate > academicYearDto.EndDate)
            {
                return BadRequest("Ngày bắt đầu của Học kỳ 2 phải nằm trong khoảng thời gian của năm học.");
            }

            if (academicYearDto.Semester1EndDate >= academicYearDto.Semester2StartDate)
            {
                return BadRequest("Ngày kết thúc của Học kỳ 1 phải trước ngày bắt đầu của Học kỳ 2.");
            }

            await _service.AddAsync(academicYearDto);
            return Ok(new { message = "Academic Year created successfully!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AcademicYearDto academicYearDto)
        {
            if (id != academicYearDto.AcademicYearID)
            {
                return BadRequest("ID trong URL không khớp với ID của AcademicYear.");
            }

            // Validate dữ liệu đầu vào
            if (academicYearDto.StartDate >= academicYearDto.EndDate)
            {
                return BadRequest("Ngày bắt đầu năm học phải trước ngày kết thúc.");
            }

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