using Application.Features.AcademicYears.DTOs;
using Application.Features.AcademicYears.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
            if (result == null)
            {
                Console.WriteLine("Academic year not found.");
                return NotFound("Không tìm thấy năm học.");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAcademicYearDto academicYearDto)
        {
            try
            {
                if (academicYearDto.StartDate >= academicYearDto.EndDate)
                {
                    Console.WriteLine("Invalid academic year date range.");
                    return BadRequest("Ngày bắt đầu năm học phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester1StartDate >= academicYearDto.Semester1EndDate)
                {
                    Console.WriteLine("Invalid semester 1 date range.");
                    return BadRequest("Ngày bắt đầu của Học kỳ 1 phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester2StartDate >= academicYearDto.Semester2EndDate)
                {
                    Console.WriteLine("Invalid semester 2 date range.");
                    return BadRequest("Ngày bắt đầu của Học kỳ 2 phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester1EndDate >= academicYearDto.Semester2StartDate)
                {
                    Console.WriteLine("Invalid semester date overlap.");
                    return BadRequest("Ngày kết thúc của Học kỳ 1 phải trước ngày bắt đầu của Học kỳ 2.");
                }

                await _service.AddAsync(academicYearDto);
                return Ok(new { message = "Năm học đã được tạo thành công!" });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating academic year: {ex.Message}");
                return BadRequest("Lỗi khi tạo năm học.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error creating academic year: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi khi tạo năm học.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAcademicYearDto academicYearDto)
        {
            if (id != academicYearDto.AcademicYearId)
            {
                Console.WriteLine("ID mismatch in update request.");
                return BadRequest("ID không khớp.");
            }

            try
            {
                if (academicYearDto.StartDate >= academicYearDto.EndDate)
                {
                    Console.WriteLine("Invalid academic year date range.");
                    return BadRequest("Ngày bắt đầu năm học phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester1StartDate >= academicYearDto.Semester1EndDate)
                {
                    Console.WriteLine("Invalid semester 1 date range.");
                    return BadRequest("Ngày bắt đầu của Học kỳ 1 phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester2StartDate >= academicYearDto.Semester2EndDate)
                {
                    Console.WriteLine("Invalid semester 2 date range.");
                    return BadRequest("Ngày bắt đầu của Học kỳ 2 phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester1EndDate >= academicYearDto.Semester2StartDate)
                {
                    Console.WriteLine("Invalid semester date overlap.");
                    return BadRequest("Ngày kết thúc của Học kỳ 1 phải trước ngày bắt đầu của Học kỳ 2.");
                }

                await _service.UpdateAsync(academicYearDto);
                return Ok(new { message = "Năm học đã được cập nhật thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating academic year: {ex.Message}");
                return NotFound("Không tìm thấy năm học.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating academic year: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật năm học.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating academic year: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật năm học.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating academic year: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi khi cập nhật năm học.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { message = "Xóa năm học thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error deleting academic year: {ex.Message}");
                return NotFound("Không tìm thấy năm học.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error deleting academic year: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi khi xóa năm học.");
            }
        }
    }
}