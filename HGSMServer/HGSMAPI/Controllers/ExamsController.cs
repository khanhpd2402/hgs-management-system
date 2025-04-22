using Application.Features.Exams.DTOs;
using Application.Features.Exams.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
                return CreatedAtAction(nameof(GetExamProposal), new { id = proposal.ProposalId }, proposal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("exam-proposal/{id}/status")]
        [Authorize(Roles = "Hiệu trưởng,Trưởng bộ môn")]
        public async Task<IActionResult> UpdateExamProposalStatus(int id, [FromBody] StatusUpdateDto dto) 
        {
            try
            {
                await _examProposalService.UpdateExamProposalStatusAsync(id, dto.Status, dto.Comment); 
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("exam-proposal/{id}")]
        [Authorize(Roles = "Giáo viên")]
        public async Task<IActionResult> UpdateExamProposal(int id, [FromForm] ExamProposalUpdateDto request)
        {
            try
            {
                var updatedProposal = await _examProposalService.UpdateExamProposalAsync(id, request);
                return Ok(updatedProposal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}