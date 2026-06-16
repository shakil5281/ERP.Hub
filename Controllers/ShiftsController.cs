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
    public class ShiftsController : ControllerBase
    {
        private readonly IErpService _erpService;

        public ShiftsController(IErpService erpService)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
        }

        // GET: api/shifts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shift>>> GetShifts()
        {
            var shifts = await _erpService.GetShiftsAsync();
            return Ok(shifts);
        }

        // GET: api/shifts/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Shift>> GetShift(int id)
        {
            var shift = await _erpService.GetShiftByIdAsync(id);
            if (shift == null)
                return NotFound($"Shift with ID {id} not found.");
            return Ok(shift);
        }

        // POST: api/shifts
        [HttpPost]
        public async Task<ActionResult<Shift>> PostShift([FromBody] Shift shift)
        {
            if (shift == null)
                return BadRequest("Invalid shift data.");

            await _erpService.AddShiftAsync(shift);
            return CreatedAtAction(nameof(GetShift), new { id = shift.Id }, shift);
        }

        // PUT: api/shifts/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutShift(int id, [FromBody] Shift shift)
        {
            if (shift == null || id != shift.Id)
                return BadRequest("Shift ID mismatch.");

            var existing = await _erpService.GetShiftByIdAsync(id);
            if (existing == null)
                return NotFound($"Shift with ID {id} not found.");

            await _erpService.UpdateShiftAsync(shift);
            return NoContent();
        }

        // DELETE: api/shifts/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteShift(int id)
        {
            var existing = await _erpService.GetShiftByIdAsync(id);
            if (existing == null)
                return NotFound($"Shift with ID {id} not found.");

            await _erpService.DeleteShiftAsync(id);
            return Ok($"Shift with ID {id} has been deleted.");
        }
    }
}
