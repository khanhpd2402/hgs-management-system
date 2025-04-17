using Application.Features.Conducts.DTOs;
using Application.Features.Conducts.Interfaces;
using Application.Features.Conducts.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConductController : ControllerBase
    {
        private readonly IConductService _conductService;

        public ConductController(IConductService conductService)
        {
            _conductService = conductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var conducts = await _conductService.GetAllAsync();
            return Ok(conducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var conduct = await _conductService.GetByIdAsync(id);
            if (conduct == null)
            {
                return NotFound();
            }
            return Ok(conduct);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateConductDto dto)
        {
            var createdConduct = await _conductService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdConduct.Id }, createdConduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateConductDto dto)
        {
            try
            {
                var updatedConduct = await _conductService.UpdateAsync(id, dto);
                return Ok(updatedConduct);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _conductService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

}
