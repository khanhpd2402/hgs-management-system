using Application.Features.Exams.DTOs;
using Application.Features.Exams.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamProposalsController : ControllerBase
    {
        private readonly IExamProposalService _examProposalService;

        public ExamProposalsController(IExamProposalService examProposalService)
        {
            _examProposalService = examProposalService;
        }

        [HttpPost("exam-proposal")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn")]
        public async Task<IActionResult> CreateExamProposal([FromForm] ExamProposalRequestDto request)
        {
            try
            {
                Console.WriteLine("Creating exam proposal...");
                var proposal = await _examProposalService.CreateExamProposalAsync(request);
                return CreatedAtAction(nameof(GetExamProposal), new { id = proposal.ProposalId }, proposal);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating exam proposal: {ex.Message}");
                return BadRequest(ex.Message); // Trả về thông báo lỗi chi tiết
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error creating exam proposal: {ex.Message}");
                return NotFound(ex.Message); // Trả về thông báo lỗi chi tiết
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error creating exam proposal: {ex.Message}");
                return BadRequest(ex.Message); // Trả về thông báo lỗi chi tiết
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error creating exam proposal: {ex.Message}");
                return StatusCode(500, "Lỗi khi tạo đề thi.");
            }
        }

        [HttpGet("exam-proposal/{id}")]
        public async Task<IActionResult> GetExamProposal(int id)
        {
            try
            {
                Console.WriteLine("Fetching exam proposal...");
                var proposal = await _examProposalService.GetExamProposalAsync(id);
                return Ok(proposal);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error fetching exam proposal: {ex.Message}");
                return NotFound(ex.Message); // Trả về thông báo lỗi chi tiết
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching exam proposal: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin đề thi.");
            }
        }

        [HttpPut("exam-proposal/{id}/status")]
        [Authorize(Roles = "Hiệu trưởng,Trưởng bộ môn")]
        public async Task<IActionResult> UpdateExamProposalStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            try
            {
                Console.WriteLine("Updating exam proposal status...");
                await _examProposalService.UpdateExamProposalStatusAsync(id, dto.Status, dto.Comment);
                return Ok("Cập nhật trạng thái đề thi thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating exam proposal status: {ex.Message}");
                return BadRequest(ex.Message); 
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating exam proposal status: {ex.Message}");
                return NotFound(ex.Message); 
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating exam proposal status: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating exam proposal status: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật trạng thái đề thi.");
            }
        }

        [HttpGet("exam-proposals/status/{status}")]
        [Authorize(Roles = "Hiệu trưởng,Trưởng bộ môn")]
        public async Task<IActionResult> GetExamProposalsByStatus(string status)
        {
            try
            {
                Console.WriteLine("Fetching exam proposals by status...");
                var proposals = await _examProposalService.GetExamProposalsByStatusAsync(status);
                return Ok(proposals);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error fetching exam proposals: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching exam proposals: {ex.Message}");
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching exam proposals: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách đề thi theo trạng thái.");
            }
        }

        [HttpPut("exam-proposal/{id}")]
        [Authorize(Roles = "Giáo viên")]
        public async Task<IActionResult> UpdateExamProposal(int id, [FromForm] ExamProposalUpdateDto request)
        {
            try
            {
                Console.WriteLine("Updating exam proposal...");
                var updatedProposal = await _examProposalService.UpdateExamProposalAsync(id, request);
                return Ok(updatedProposal);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating exam proposal: {ex.Message}");
                return NotFound(ex.Message); 
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating exam proposal: {ex.Message}");
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating exam proposal: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật đề thi.");
            }
        }

        [HttpGet("exam-proposals")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư,Trưởng bộ môn")]
        public async Task<IActionResult> GetAllExamProposals()
        {
            try
            {
                Console.WriteLine("Fetching all exam proposals...");
                var proposals = await _examProposalService.GetAllAsync();
                return Ok(proposals);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching exam proposals: {ex.Message}");
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching exam proposals: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách tất cả đề thi.");
            }
        }

        [HttpGet("exam-proposals/teacher/{teacherId}")]
        [Authorize(Roles = "Giáo viên,Hiệu trưởng,Cán bộ văn thư,Trưởng bộ môn")]
        public async Task<IActionResult> GetExamProposalsByTeacherId(int teacherId)
        {
            try
            {
                Console.WriteLine("Fetching exam proposals by teacher...");
                var proposals = await _examProposalService.GetAllByTeacherIdAsync(teacherId);
                return Ok(proposals);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching exam proposals: {ex.Message}");
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching exam proposals: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách đề thi của giáo viên.");
            }
        }

        [HttpGet("department-head-statistics")]
        [Authorize(Roles = "Trưởng bộ môn,Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> GetDepartmentHeadExamProposalStatistics()
        {
            try
            {
                Console.WriteLine("Fetching exam proposal statistics for department head...");
                var statistics = await _examProposalService.GetDepartmentHeadExamProposalStatisticsAsync();
                return Ok(statistics);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                return Unauthorized(ex.Message); 
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching exam proposal statistics: {ex.Message}");
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching exam proposal statistics: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thống kê đề thi.");
            }
        }
    }
}