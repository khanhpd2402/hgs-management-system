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
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy danh sách học kỳ." });
            }
        }

        [HttpGet("by-academic-year/{academicYearId}")]
        public async Task<IActionResult> GetByAcademicYear(int academicYearId)
        {
            try
            {
                if (academicYearId <= 0)
                {
                    return BadRequest(new { message = "AcademicYearId phải là một số nguyên dương." });
                }

                var result = await _service.GetByAcademicYearIdAsync(academicYearId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy danh sách học kỳ theo năm học." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID học kỳ phải là một số nguyên dương." });
                }

                var result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy thông tin học kỳ." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSemesterDto semesterDto)
        {
            try
            {
                if (semesterDto == null)
                {
                    return BadRequest(new { message = "Thông tin học kỳ không được để trống." });
                }

                await _service.AddAsync(semesterDto);
                return Ok(new { message = "Tạo học kỳ thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình tạo học kỳ." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SemesterDto semesterDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID học kỳ phải là một số nguyên dương." });
                }

                if (semesterDto == null)
                {
                    return BadRequest(new { message = "Thông tin học kỳ không được để trống." });
                }

                if (id != semesterDto.SemesterID)
                {
                    return BadRequest(new { message = "ID trong URL không khớp với ID của học kỳ trong dữ liệu gửi lên." });
                }

                await _service.UpdateAsync(semesterDto);
                return Ok(new { message = "Cập nhật học kỳ thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình cập nhật học kỳ." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID học kỳ phải là một số nguyên dương." });
                }

                await _service.DeleteAsync(id);
                return Ok(new { message = "Xóa học kỳ thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình xóa học kỳ." });
            }
        }
    }
}