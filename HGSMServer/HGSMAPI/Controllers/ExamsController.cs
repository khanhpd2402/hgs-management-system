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
    }
}