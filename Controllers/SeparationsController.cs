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
        private readonly ISeparationService _separationService;

        public SeparationsController(IErpService erpService, ISeparationService separationService)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
            _separationService = separationService ?? throw new ArgumentNullException(nameof(separationService));
        }

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

        [HttpGet("separated-employees")]
        public async Task<ActionResult<IEnumerable<EmployeeSeparationDto>>> GetSeparatedEmployees(
            [FromQuery] string? type = null,
            [FromQuery] int? companyId = null,
            [FromQuery] int? deptId = null,
            [FromQuery] int? sectionId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var filter = new SeparationFilter
            {
                SeparationType = type,
                CompanyId = companyId,
                DepartmentId = deptId,
                SectionId = sectionId,
                FromDate = fromDate,
                ToDate = toDate
            };
            return Ok(await _separationService.GetSeparatedEmployeesAsync(filter));
        }

        [HttpGet("active-employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetActiveEmployees(
            [FromQuery] int? companyId = null,
            [FromQuery] int? deptId = null,
            [FromQuery] int? sectionId = null)
            => Ok(await _separationService.GetActiveEmployeesAsync(companyId, deptId, sectionId));

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStats()
        {
            var active = await _separationService.GetActiveEmployeeCountAsync();
            var separated = await _separationService.GetSeparationCountAsync();
            var byType = await _separationService.GetSeparationSummaryByTypeAsync();
            return Ok(new { ActiveEmployees = active, SeparatedEmployees = separated, ByType = byType });
        }

        [HttpGet("eligibility/{employeeId}/{date:datetime}")]
        public async Task<ActionResult<object>> CheckEligibility(string employeeId, DateTime date)
        {
            var eligible = await _separationService.IsEligibleForProcessingAsync(employeeId, date);
            return Ok(new { EmployeeId = employeeId, Date = date.Date, Eligible = eligible });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Separation>> GetSeparation(int id)
        {
            var separation = await _erpService.GetSeparationByIdAsync(id);
            if (separation == null)
                return NotFound($"Separation with ID {id} not found.");
            return Ok(separation);
        }

        [HttpPost("record")]
        public async Task<ActionResult<SeparationResult>> RecordSeparation([FromBody] RecordSeparationRequest request)
        {
            if (request == null)
                return BadRequest("Invalid separation data.");

            var result = await _separationService.RecordSeparationAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Separation>> PostSeparation([FromBody] RecordSeparationRequest request)
        {
            if (request == null)
                return BadRequest("Invalid separation data.");

            var result = await _separationService.RecordSeparationAsync(request);
            if (!result.Success)
                return BadRequest(result.Message);

            return CreatedAtAction(nameof(GetSeparation), new { id = result.Separation!.Id }, result.Separation);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult> PostBulkSeparations([FromBody] List<RecordSeparationRequest> requests)
        {
            if (requests == null || requests.Count == 0)
                return BadRequest("No separation data provided.");

            int success = 0;
            var errors = new List<string>();

            foreach (var req in requests)
            {
                var result = await _separationService.RecordSeparationAsync(req);
                if (result.Success)
                    success++;
                else
                    errors.Add($"{req.EmployeeId}: {result.Message}");
            }

            return Ok(new { Message = $"{success} separation(s) recorded.", Success = success, Errors = errors });
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<ActionResult<SeparationResult>> CancelSeparation(int id, [FromBody] CancelSeparationRequest? request)
        {
            var result = await _separationService.CancelSeparationAsync(
                id,
                request?.CancelledBy ?? "HR",
                request?.Reason ?? "Reversal");

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
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

    public class CancelSeparationRequest
    {
        public string CancelledBy { get; set; } = "HR";
        public string Reason { get; set; } = string.Empty;
    }
}
