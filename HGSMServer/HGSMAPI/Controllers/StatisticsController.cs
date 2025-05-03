using Application.Features.Statistics.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("school")]
        public async Task<IActionResult> GetSchoolStatistics()
        {
            var result = await _statisticsService.GetSchoolStatisticsAsync();
            return Ok(result);
        }
    }
}
