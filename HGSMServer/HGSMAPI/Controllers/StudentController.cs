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
                await _studentService.ImportStudentsFromExcelAsync(file);
                return Ok("Import danh sách học sinh thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("download-template")]
        public IActionResult DownloadExcelTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Students");

                // Định nghĩa các cột
                var headers = new List<string>
        {
            "Họ và tên", "Ngày sinh", "Giới tính", "Ngày nhập học", "Hình thức nhập học", "Dân tộc",
            "Địa chỉ thường trú", "Nơi sinh", "Tôn giáo", "Lưu ban", "Số CMND/CCCD", "Trạng thái",
            "FullNameFather", "YearOfBirthFather", "OccupationFather", "PhoneNumberFather", "EmailFather", "IdcardNumberFather",
            "FullNameMother", "YearOfBirthMother", "OccupationMother", "PhoneNumberMother", "EmailMother", "IdcardNumberMother",
            "FullNameGuardian", "YearOfBirthGuardian", "OccupationGuardian", "PhoneNumberGuardian", "EmailGuardian", "IdcardNumberGuardian"
        };

                // Thêm tiêu đề
                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                }

                // Thêm dòng ví dụ
                worksheet.Cell(2, 1).Value = "Nguyễn Văn A";
                worksheet.Cell(2, 2).Value = "25-04-2008";
                worksheet.Cell(2, 3).Value = "Nam";
                worksheet.Cell(2, 4).Value = "25-04-2008";
                worksheet.Cell(2, 5).Value = "Xét tuyển";
                worksheet.Cell(2, 6).Value = "Kinh";
                worksheet.Cell(2, 7).Value = "123 Đường ABC, Quận 1";
                worksheet.Cell(2, 8).Value = "Hà Nội";
                worksheet.Cell(2, 9).Value = "Phật giáo";
                worksheet.Cell(2, 10).Value = "Không";
                worksheet.Cell(2, 11).Value = "123456290121";
                worksheet.Cell(2, 12).Value = "Đang học";
                worksheet.Cell(2, 13).Value = "Nguyễn Văn B";
                worksheet.Cell(2, 14).Value = "25-04-1999";
                worksheet.Cell(2, 15).Value = "Kỹ sư";
                worksheet.Cell(2, 16).Value = "0987614321";
                worksheet.Cell(2, 17).Value = "nguyenvanbxcx@example.com";
                worksheet.Cell(2, 18).Value = "987654321098";
                worksheet.Cell(2, 19).Value = "Trần Thị C";
                worksheet.Cell(2, 20).Value = "25-04-1973";
                worksheet.Cell(2, 21).Value = "Giáo viên";
                worksheet.Cell(2, 22).Value = "0978123156";
                worksheet.Cell(2, 23).Value = "tranthicgg@example.com";
                worksheet.Cell(2, 24).Value = "123256789123";
                worksheet.Cell(2, 25).Value = "Lê Văn D";
                worksheet.Cell(2, 26).Value = "25-04-2000";
                worksheet.Cell(2, 27).Value = "Doanh nhân";
                worksheet.Cell(2, 28).Value = "0912342678";
                worksheet.Cell(2, 29).Value = "levand1@example.com";
                worksheet.Cell(2, 30).Value = "456789143456";

                // Tự động điều chỉnh độ rộng cột
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