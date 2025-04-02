using Application.Features.Exams.DTOs;
using Application.Features.Exams.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromForm] QuestionDto questionDto, IFormFile imageFile)
        {
            var result = await _questionService.AddQuestionAsync(questionDto, imageFile);
            return CreatedAtAction(nameof(GetQuestion), new { id = result.QuestionId }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuestions(int subjectId, int grade)
        {
            var questions = await _questionService.GetAllQuestionsAsync(subjectId, grade);
            return Ok(questions);
        }

        [HttpPost("exam-proposal")]
        public async Task<IActionResult> CreateExamProposal([FromBody] ExamProposalRequestDto request)
        {
            var proposal = await _questionService.CreateExamProposalAsync(request);
            return CreatedAtAction(nameof(GetExamProposal), new { id = proposal.ProposalId }, proposal);
        }

        [HttpGet("exam-proposal/{id}")]
        public async Task<IActionResult> GetExamProposal(int id)
        {
            var proposal = await _questionService.GetExamProposalAsync(id);
            return Ok(proposal);
        }
    }
}