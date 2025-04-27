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
        [Authorize(Roles = "Giáo viên")]
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
                return BadRequest("Lỗi khi tạo đề thi.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error creating exam proposal: {ex.Message}");
                return NotFound("Không tìm thấy dữ liệu liên quan.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error creating exam proposal: {ex.Message}");
                return BadRequest("Lỗi khi tạo đề thi.");
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
                return NotFound("Không tìm thấy đề thi.");
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
                return BadRequest("Lỗi khi cập nhật trạng thái đề thi.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating exam proposal status: {ex.Message}");
                return NotFound("Không tìm thấy đề thi.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating exam proposal status: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật trạng thái đề thi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating exam proposal status: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật trạng thái đề thi.");
            }
        }

        [HttpGet("exam-proposals/status/{status}")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
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
                return BadRequest("Lỗi khi lấy danh sách đề thi theo trạng thái.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching exam proposals: {ex.Message}");
                return BadRequest("Lỗi khi lấy danh sách đề thi theo trạng thái.");
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
                return NotFound("Không tìm thấy đề thi.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating exam proposal: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật đề thi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating exam proposal: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật đề thi.");
            }
        }

        [HttpGet("exam-proposals")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
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
                return BadRequest("Lỗi khi lấy danh sách tất cả đề thi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching exam proposals: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách tất cả đề thi.");
            }
        }

        [HttpGet("exam-proposals/teacher/{teacherId}")]
        [Authorize(Roles = "Giáo viên,Hiệu trưởng,Cán bộ văn thư")]
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
                return BadRequest("Lỗi khi lấy danh sách đề thi của giáo viên.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching exam proposals: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách đề thi của giáo viên.");
            }
        }
    }
}