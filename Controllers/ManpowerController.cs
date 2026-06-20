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
    public class ManpowerController : ControllerBase
    {
        private readonly IErpService _erpService;

        public ManpowerController(IErpService erpService)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manpower>>> GetManpowers()
        {
            var manpowers = await _erpService.GetManpowersAsync();
            return Ok(manpowers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Manpower>> GetManpower(int id)
        {
            var manpower = await _erpService.GetManpowerByIdAsync(id);
            if (manpower == null)
                return NotFound($"Manpower with ID {id} not found.");
            return Ok(manpower);
        }

        [HttpPost]
        public async Task<ActionResult<Manpower>> PostManpower([FromBody] Manpower manpower)
        {
            if (manpower == null)
                return BadRequest("Invalid manpower data.");

            await _erpService.AddManpowerAsync(manpower);
            return CreatedAtAction(nameof(GetManpower), new { id = manpower.Id }, manpower);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutManpower(int id, [FromBody] Manpower manpower)
        {
            if (manpower == null || id != manpower.Id)
                return BadRequest("Manpower ID mismatch.");

            var existing = await _erpService.GetManpowerByIdAsync(id);
            if (existing == null)
                return NotFound($"Manpower with ID {id} not found.");

            await _erpService.UpdateManpowerAsync(manpower);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteManpower(int id)
        {
            var existing = await _erpService.GetManpowerByIdAsync(id);
            if (existing == null)
                return NotFound($"Manpower with ID {id} not found.");

            await _erpService.DeleteManpowerAsync(id);
            return Ok($"Manpower with ID {id} has been deleted.");
        }

        [HttpPost("recalculate")]
        public async Task<ActionResult<object>> RecalculateManpower()
        {
            try
            {
                await _erpService.RecalculateManpowerAsync();
                return Ok(new { Message = "Manpower recalculated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message, Details = ex.InnerException?.Message });
            }
        }
    }
}