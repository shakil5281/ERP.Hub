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
    public class ManpowerRequirementsController : ControllerBase
    {
        private readonly IErpService _erpService;

        public ManpowerRequirementsController(IErpService erpService)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManpowerRequirement>>> GetManpowerRequirements()
        {
            var requirements = await _erpService.GetManpowerRequirementsAsync();
            return Ok(requirements);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ManpowerRequirement>> GetManpowerRequirement(int id)
        {
            var requirement = await _erpService.GetManpowerRequirementByIdAsync(id);
            if (requirement == null)
                return NotFound($"Manpower requirement with ID {id} not found.");
            return Ok(requirement);
        }

        [HttpPost]
        public async Task<ActionResult<ManpowerRequirement>> PostManpowerRequirement([FromBody] ManpowerRequirement requirement)
        {
            if (requirement == null)
                return BadRequest("Invalid manpower requirement data.");

            await _erpService.AddManpowerRequirementAsync(requirement);
            return CreatedAtAction(nameof(GetManpowerRequirement), new { id = requirement.Id }, requirement);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutManpowerRequirement(int id, [FromBody] ManpowerRequirement requirement)
        {
            if (requirement == null || id != requirement.Id)
                return BadRequest("Manpower requirement ID mismatch.");

            var existing = await _erpService.GetManpowerRequirementByIdAsync(id);
            if (existing == null)
                return NotFound($"Manpower requirement with ID {id} not found.");

            await _erpService.UpdateManpowerRequirementAsync(requirement);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteManpowerRequirement(int id)
        {
            var existing = await _erpService.GetManpowerRequirementByIdAsync(id);
            if (existing == null)
                return NotFound($"Manpower requirement with ID {id} not found.");

            await _erpService.DeleteManpowerRequirementAsync(id);
            return Ok($"Manpower requirement with ID {id} has been deleted.");
        }
    }
}