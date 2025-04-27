using Application.Features.AcademicYears.DTOs;
using Application.Features.AcademicYears.Interfaces;
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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                Console.WriteLine("Fetching all academic years...");
                var academicYears = await _service.GetAllAsync();
                return Ok(academicYears);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching academic years: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách năm học.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                Console.WriteLine("Fetching academic year...");
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                {
                    Console.WriteLine("Academic year not found.");
                    return NotFound("Không tìm thấy năm học.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching academic year: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin năm học.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAcademicYearDto academicYearDto)
        {
            try
            {
                if (academicYearDto == null)
                {
                    Console.WriteLine("Academic year data is null.");
                    return BadRequest("Dữ liệu năm học không được để trống.");
                }

                Console.WriteLine("Validating academic year dates...");
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

                Console.WriteLine("Creating academic year...");
                await _service.AddAsync(academicYearDto);
                return Ok("Năm học đã được tạo thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating academic year: {ex.Message}");
                return BadRequest("Lỗi khi tạo năm học.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error creating academic year: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo năm học.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAcademicYearDto academicYearDto)
        {
            try
            {
                if (id != academicYearDto.AcademicYearId)
                {
                    Console.WriteLine("ID mismatch in update request.");
                    return BadRequest("ID không khớp.");
                }

                Console.WriteLine("Validating academic year dates...");
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

                Console.WriteLine("Updating academic year...");
                await _service.UpdateAsync(academicYearDto);
                return Ok("Năm học đã được cập nhật thành công.");
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
                return StatusCode(500, "Lỗi khi cập nhật năm học.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine("Deleting academic year...");
                await _service.DeleteAsync(id);
                return Ok("Xóa năm học thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error deleting academic year: {ex.Message}");
                return NotFound("Không tìm thấy năm học.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error deleting academic year: {ex.Message}");
                return StatusCode(500, "Lỗi khi xóa năm học.");
            }
        }
    }
}