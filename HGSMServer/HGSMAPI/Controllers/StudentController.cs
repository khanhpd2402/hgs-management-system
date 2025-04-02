using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        }

        // Lấy danh sách học sinh theo năm học
        [HttpGet("{academicYearId}")]
        public async Task<IActionResult> GetAllStudentsWithParents(int academicYearId)
        {
            var students = await _studentService.GetAllStudentsWithParentsAsync(academicYearId);
            return Ok(students);
        }

        // Lấy chi tiết một học sinh theo ID và năm học
        [HttpGet("{id}/{academicYearId}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id, int academicYearId)
        {
            var student = await _studentService.GetStudentByIdAsync(id, academicYearId);
            if (student == null) return NotFound();

            return Ok(student);
        }

        // POST: api/Student
        
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto createStudentDto)
        {
            if (createStudentDto == null) return BadRequest("Student data cannot be null.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid input data.", errors });
            }

            try
            {
                var studentId = await _studentService.AddStudentAsync(createStudentDto);
                var createdStudent = await _studentService.GetStudentByIdAsync(studentId, 1); // Giả sử AcademicYearId = 1
                return CreatedAtAction(nameof(GetStudent), new { id = studentId, academicYearId = 1 }, createdStudent);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Student/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto updateStudentDto)
        {
            if (id != updateStudentDto.StudentId) return BadRequest("Student ID mismatch.");

            try
            {
                await _studentService.UpdateStudentAsync(updateStudentDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportStudentsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file Excel!");

            try
            {
                var results = await _studentService.ImportStudentsFromExcelAsync(file);
                return Ok(new ApiResponse(true, "Import danh sách học sinh hoàn tất", results));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(false, $"Lỗi khi import: {ex.Message}"));
            }
        }

        public class ApiResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }

            public ApiResponse(bool success, string message, object data = null)
            {
                Success = success;
                Message = message;
                Data = data;
            }
        }
        [HttpGet("download-template")]
        public IActionResult DownloadExcelTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Students");

                var headers = new List<string>
        {
            "Họ và tên", "Ngày sinh", "Giới tính", "Ngày nhập học", "Hình thức nhập học", "Dân tộc",
            "Địa chỉ thường trú", "Nơi sinh", "Tôn giáo", "Lưu ban", "Số CMND/CCCD", "Trạng thái",
            "Tên lớp",
            "Họ và tên cha", "Ngày sinh cha", "Nghề nghiệp cha", "SĐT cha", "Email cha", "Số CCCD cha",
            "Họ và tên mẹ", "Ngày sinh mẹ", "Nghề nghiệp mẹ", "SĐT mẹ", "Email mẹ", "Số CCCD mẹ",
            "Họ và tên người bảo hộ", "Ngày sinh người bảo hộ", "Nghề nghiệp người bảo hộ", "SĐT người bảo hộ", "Email người bảo hộ", "Số CCCD người bảo hộ"
        };

                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                }


                worksheet.Cell(2, 1).Value = "Giàng A Páo";
                worksheet.Cell(2, 2).Value = "15-08-2010";
                worksheet.Cell(2, 3).Value = "Nữ";
                worksheet.Cell(2, 4).Value = "10-09-2022"; 
                worksheet.Cell(2, 5).Value = "Thi tuyển";
                worksheet.Cell(2, 6).Value = "Kinh";
                worksheet.Cell(2, 7).Value = "456 Đường XYZ, Quận 2";
                worksheet.Cell(2, 8).Value = "TP. Hồ Chí Minh";
                worksheet.Cell(2, 9).Value = "Không";
                worksheet.Cell(2, 10).Value = "Không";
                worksheet.Cell(2, 11).Value = "987004321012";
                worksheet.Cell(2, 12).Value = "Đang học";
                worksheet.Cell(2, 13).Value = "7B";
                worksheet.Cell(2, 14).Value = "Giàng Giàng Giàng";
                worksheet.Cell(2, 15).Value = "20-05-1985";
                worksheet.Cell(2, 16).Value = "Kỹ sư";
                worksheet.Cell(2, 17).Value = "0012115678"; // SĐT hợp lệ 10 số
                worksheet.Cell(2, 18).Value = "tranvananaaa@example.com";
                worksheet.Cell(2, 19).Value = "120056789876";
                //worksheet.Cell(2, 20).Value = "Lê Thị Hồng";
                //worksheet.Cell(2, 21).Value = "12-11-1987";
                //worksheet.Cell(2, 22).Value = "Nhân viên văn phòng";
                //worksheet.Cell(2, 23).Value = "0987654321"; // SĐT hợp lệ 10 số
                //worksheet.Cell(2, 24).Value = "lethihong@example.com";
                //worksheet.Cell(2, 25).Value = "789456123654";
                //worksheet.Cell(2, 26).Value = "Phạm Văn Bình";
                //worksheet.Cell(2, 27).Value = "25-07-1982";
                //worksheet.Cell(2, 28).Value = "Doanh nhân";
                //worksheet.Cell(2, 29).Value = "0965124789"; // SĐT hợp lệ 10 số
                //worksheet.Cell(2, 30).Value = "phamvanbinh@example.com";
                //worksheet.Cell(2, 31).Value = "321654987123"; // Căn cước công dân mới


                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentImportTemplate.xlsx");
                }
            }
        }



    }
}