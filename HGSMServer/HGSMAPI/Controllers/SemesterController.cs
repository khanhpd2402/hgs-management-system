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
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching semesters: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi lấy danh sách học kỳ." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching semesters: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy danh sách học kỳ." });
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
                    return BadRequest(new { message = "AcademicYearId không hợp lệ." });
                }

                var result = await _service.GetByAcademicYearIdAsync(academicYearId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching semesters by academic year: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi lấy danh sách học kỳ." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching semesters by academic year: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy danh sách học kỳ." });
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
                    return BadRequest(new { message = "ID học kỳ không hợp lệ." });
                }

                var result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error fetching semester: {ex.Message}");
                return NotFound(new { message = "Không tìm thấy học kỳ." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching semester: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy thông tin học kỳ." });
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
                    return BadRequest(new { message = "Thông tin học kỳ không được để trống." });
                }

                await _service.AddAsync(semesterDto);
                return Ok(new { message = "Tạo học kỳ thành công!" });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating semester: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi tạo học kỳ." });
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error creating semester: {ex.Message}");
                return NotFound(new { message = "Không tìm thấy năm học." });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error creating semester: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi tạo học kỳ." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error creating semester: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi tạo học kỳ." });
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
                    return BadRequest(new { message = "ID học kỳ không hợp lệ." });
                }

                if (semesterDto == null)
                {
                    Console.WriteLine("Semester data is null.");
                    return BadRequest(new { message = "Thông tin học kỳ không được để trống." });
                }

                if (id != semesterDto.SemesterID)
                {
                    Console.WriteLine("ID mismatch in update request.");
                    return BadRequest(new { message = "ID không khớp." });
                }

                await _service.UpdateAsync(semesterDto);
                return Ok(new { message = "Cập nhật học kỳ thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating semester: {ex.Message}");
                return NotFound(new { message = "Không tìm thấy học kỳ." });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating semester: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi cập nhật học kỳ." });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating semester: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi cập nhật học kỳ." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating semester: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật học kỳ." });
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
                    return BadRequest(new { message = "ID học kỳ không hợp lệ." });
                }

                await _service.DeleteAsync(id);
                return Ok(new { message = "Xóa học kỳ thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error deleting semester: {ex.Message}");
                return NotFound(new { message = "Không tìm thấy học kỳ." });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error deleting semester: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi xóa học kỳ." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error deleting semester: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi xóa học kỳ." });
            }
        }
    }
}