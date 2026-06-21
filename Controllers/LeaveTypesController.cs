using Microsoft.AspNetCore.Mvc;
using ERPHub.Models;
using ERPHub.Services;

namespace ERPHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveTypesController : ControllerBase
    {
        private readonly IErpService _erpService;
        public LeaveTypesController(IErpService erpService) => _erpService = erpService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveType>>> GetLeaveTypes()
            => Ok(await _erpService.GetLeaveTypesAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LeaveType>> GetLeaveType(int id)
        {
            var lt = await _erpService.GetLeaveTypeByIdAsync(id);
            return lt == null ? NotFound() : Ok(lt);
        }

        [HttpPost]
        public async Task<ActionResult<LeaveType>> PostLeaveType([FromBody] LeaveType leaveType)
        {
            if (leaveType == null) return BadRequest();
            await _erpService.AddLeaveTypeAsync(leaveType);
            return CreatedAtAction(nameof(GetLeaveType), new { id = leaveType.Id }, leaveType);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutLeaveType(int id, [FromBody] LeaveType leaveType)
        {
            if (leaveType == null || id != leaveType.Id) return BadRequest();
            var existing = await _erpService.GetLeaveTypeByIdAsync(id);
            if (existing == null) return NotFound();
            await _erpService.UpdateLeaveTypeAsync(leaveType);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLeaveType(int id)
        {
            var existing = await _erpService.GetLeaveTypeByIdAsync(id);
            if (existing == null) return NotFound();
            await _erpService.DeleteLeaveTypeAsync(id);
            return Ok();
        }
    }
}
