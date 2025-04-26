using Application.Features.AcademicYears.DTOs;
using Application.Features.AcademicYears.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System; // Thêm using System nếu chưa có
using System.Threading.Tasks; // Thêm using Task nếu chưa có


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
            return result != null ? Ok(result) : NotFound($"Không tìm thấy năm học với ID {id}.");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAcademicYearDto academicYearDto) 
        {
            try
            {
                if (academicYearDto.StartDate >= academicYearDto.EndDate)
                {
                    return BadRequest("Ngày bắt đầu năm học phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester1StartDate >= academicYearDto.Semester1EndDate)
                {
                    return BadRequest("Ngày bắt đầu của Học kỳ 1 phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester2StartDate >= academicYearDto.Semester2EndDate)
                {
                    return BadRequest("Ngày bắt đầu của Học kỳ 2 phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester1EndDate >= academicYearDto.Semester2StartDate)
                {
                    return BadRequest("Ngày kết thúc của Học kỳ 1 phải trước ngày bắt đầu của Học kỳ 2.");
                }

                await _service.AddAsync(academicYearDto);
                return Ok(new { message = "Năm học đã được tạo thành công!" });
            }
            catch (ArgumentException ex) // Bắt lỗi validation từ Service
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAcademicYearDto academicYearDto)
        {
            if (id != academicYearDto.AcademicYearId)
            {
                return BadRequest("ID trong URL không khớp với ID của năm học trong dữ liệu gửi lên.");
            }

            try
            {
                
                if (academicYearDto.StartDate >= academicYearDto.EndDate)
                {
                    return BadRequest("Ngày bắt đầu năm học phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester1StartDate >= academicYearDto.Semester1EndDate)
                {
                    return BadRequest("Ngày bắt đầu của Học kỳ 1 phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester2StartDate >= academicYearDto.Semester2EndDate)
                {
                    return BadRequest("Ngày bắt đầu của Học kỳ 2 phải trước ngày kết thúc.");
                }
                if (academicYearDto.Semester1EndDate >= academicYearDto.Semester2StartDate)
                {
                    return BadRequest("Ngày kết thúc của Học kỳ 1 phải trước ngày bắt đầu của Học kỳ 2.");
                }
                
                await _service.UpdateAsync(academicYearDto); 
                return Ok(new { message = "Năm học đã được cập nhật thành công!" }); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { message = $"Năm học với ID {id} đã được xóa thành công!" }); // Hoặc NoContent()
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi trong quá trình xóa năm học.");
            }
        }
    }
}