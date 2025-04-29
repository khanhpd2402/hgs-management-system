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
                Console.WriteLine("Fetching all semesters...");
                var result = await _service.GetAllSemester();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching semesters: {ex.Message}");
                return BadRequest("Lỗi khi lấy danh sách học kỳ.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching semesters: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách học kỳ.");
            }
        }

        [HttpGet("by-academic-year/{academicYearId}")]
        public async Task<IActionResult> GetByAcademicYear(int academicYearId)
        {
            try
            {
                if (academicYearId <= 0)
                {
                    Console.WriteLine("Invalid AcademicYearId.");
                    return BadRequest("AcademicYearId không hợp lệ.");
                }

                Console.WriteLine("Fetching semesters by academic year...");
                var result = await _service.GetByAcademicYearIdAsync(academicYearId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching semesters by academic year: {ex.Message}");
                return BadRequest("Lỗi khi lấy danh sách học kỳ.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching semesters by academic year: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách học kỳ.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    Console.WriteLine("Invalid semester ID.");
                    return BadRequest("ID học kỳ không hợp lệ.");
                }

                Console.WriteLine("Fetching semester...");
                var result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error fetching semester: {ex.Message}");
                return NotFound("Không tìm thấy học kỳ.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching semester: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin học kỳ.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSemesterDto semesterDto)
        {
            try
            {
                if (semesterDto == null)
                {
                    Console.WriteLine("Semester data is null.");
                    return BadRequest("Thông tin học kỳ không được để trống.");
                }

                Console.WriteLine("Creating semester...");
                await _service.AddAsync(semesterDto);
                return Ok("Tạo học kỳ thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating semester: {ex.Message}");
                return BadRequest("Lỗi khi tạo học kỳ.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error creating semester: {ex.Message}");
                return NotFound("Không tìm thấy năm học.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error creating semester: {ex.Message}");
                return BadRequest("Lỗi khi tạo học kỳ.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error creating semester: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo học kỳ.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SemesterDto semesterDto)
        {
            try
            {
                if (id <= 0)
                {
                    Console.WriteLine("Invalid semester ID.");
                    return BadRequest("ID học kỳ không hợp lệ.");
                }

                if (semesterDto == null)
                {
                    Console.WriteLine("Semester data is null.");
                    return BadRequest("Thông tin học kỳ không được để trống.");
                }

                if (id != semesterDto.SemesterID)
                {
                    Console.WriteLine("ID mismatch in update request.");
                    return BadRequest("ID không khớp.");
                }

                Console.WriteLine("Updating semester...");
                await _service.UpdateAsync(semesterDto);
                return Ok("Cập nhật học kỳ thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating semester: {ex.Message}");
                return NotFound("Không tìm thấy học kỳ.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating semester: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật học kỳ.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating semester: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật học kỳ.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating semester: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật học kỳ.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    Console.WriteLine("Invalid semester ID.");
                    return BadRequest("ID học kỳ không hợp lệ.");
                }

                Console.WriteLine("Deleting semester...");
                await _service.DeleteAsync(id);
                return Ok("Xóa học kỳ thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error deleting semester: {ex.Message}");
                return NotFound("Không tìm thấy học kỳ.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error deleting semester: {ex.Message}");
                return BadRequest("Lỗi khi xóa học kỳ.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error deleting semester: {ex.Message}");
                return StatusCode(500, "Lỗi khi xóa học kỳ.");
            }
        }
    }
}