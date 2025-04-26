using Application.Features.Exams.DTOs;
using Application.Features.Exams.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
                var proposal = await _examProposalService.CreateExamProposalAsync(request);
                return CreatedAtAction(nameof(GetExamProposal), new { id = proposal.ProposalId }, new { message = "Tạo đề thi thành công!", proposal });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình tạo đề thi." });
            }
        }

        [HttpGet("exam-proposal/{id}")]
        public async Task<IActionResult> GetExamProposal(int id)
        {
            try
            {
                var proposal = await _examProposalService.GetExamProposalAsync(id);
                return Ok(proposal);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy thông tin đề thi." });
            }
        }

        [HttpPut("exam-proposal/{id}/status")]
        [Authorize(Roles = "Hiệu trưởng,Trưởng bộ môn")]
        public async Task<IActionResult> UpdateExamProposalStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            try
            {
                await _examProposalService.UpdateExamProposalStatusAsync(id, dto.Status, dto.Comment);
                return Ok(new { message = "Cập nhật trạng thái đề thi thành công!" });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình cập nhật trạng thái đề thi." });
            }
        }

        [HttpGet("exam-proposals/status/{status}")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> GetExamProposalsByStatus(string status)
        {
            try
            {
                var proposals = await _examProposalService.GetExamProposalsByStatusAsync(status);
                return Ok(proposals);
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy danh sách đề thi theo trạng thái." });
            }
        }

        [HttpPut("exam-proposal/{id}")]
        [Authorize(Roles = "Giáo viên")]
        public async Task<IActionResult> UpdateExamProposal(int id, [FromForm] ExamProposalUpdateDto request)
        {
            try
            {
                var updatedProposal = await _examProposalService.UpdateExamProposalAsync(id, request);
                return Ok(new { message = "Cập nhật đề thi thành công!", proposal = updatedProposal });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình cập nhật đề thi." });
            }
        }

        [HttpGet("exam-proposals")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> GetAllExamProposals()
        {
            try
            {
                var proposals = await _examProposalService.GetAllAsync();
                return Ok(proposals);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy danh sách tất cả đề thi." });
            }
        }

        [HttpGet("exam-proposals/teacher/{teacherId}")]
        [Authorize(Roles = "Giáo viên,Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> GetExamProposalsByTeacherId(int teacherId)
        {
            try
            {
                var proposals = await _examProposalService.GetAllByTeacherIdAsync(teacherId);
                return Ok(proposals);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình lấy danh sách đề thi của giáo viên." });
            }
        }
    }
}