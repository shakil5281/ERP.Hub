using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERPHub.Models;
using ERPHub.Services;

namespace ERPHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeparationsController : ControllerBase
    {
        private readonly IErpService _erpService;

        public SeparationsController(IErpService erpService)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
        }

        // GET /api/separations?type=Resignation&status=Settled&deptId=2&fromDate=2026-01-01&toDate=2026-06-30
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Separation>>> GetSeparations(
            [FromQuery] string? type = null,
            [FromQuery] string? status = null,
            [FromQuery] int? deptId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var separations = await _erpService.GetSeparationsAsync(type, status, deptId, fromDate, toDate);
            return Ok(separations);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Separation>> GetSeparation(int id)
        {
            var separation = await _erpService.GetSeparationByIdAsync(id);
            if (separation == null)
                return NotFound($"Separation with ID {id} not found.");
            return Ok(separation);
        }

        [HttpPost]
        public async Task<ActionResult<Separation>> PostSeparation([FromBody] Separation separation)
        {
            if (separation == null)
                return BadRequest("Invalid separation data.");

            await _erpService.AddSeparationAsync(separation);
            return CreatedAtAction(nameof(GetSeparation), new { id = separation.Id }, separation);
        }

        // POST /api/separations/bulk — create multiple separations at once
        [HttpPost("bulk")]
        public async Task<ActionResult> PostBulkSeparations([FromBody] List<Separation> separations)
        {
            if (separations == null || separations.Count == 0)
                return BadRequest("No separation data provided.");

            foreach (var sep in separations)
                await _erpService.AddSeparationAsync(sep);

            return Ok(new { Message = $"{separations.Count} separation(s) recorded successfully." });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutSeparation(int id, [FromBody] Separation separation)
        {
            if (separation == null || id != separation.Id)
                return BadRequest("Separation ID mismatch.");

            var existing = await _erpService.GetSeparationByIdAsync(id);
            if (existing == null)
                return NotFound($"Separation with ID {id} not found.");

            await _erpService.UpdateSeparationAsync(separation);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSeparation(int id)
        {
            var existing = await _erpService.GetSeparationByIdAsync(id);
            if (existing == null)
                return NotFound($"Separation with ID {id} not found.");

            await _erpService.DeleteSeparationAsync(id);
            return Ok($"Separation with ID {id} has been deleted.");
        }
    }
}