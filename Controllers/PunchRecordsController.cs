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
    public class PunchRecordsController : ControllerBase
    {
        private readonly IErpService _erpService;

        public PunchRecordsController(IErpService erpService)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PunchRecord>>> GetPunchRecords(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? employeeId)
        {
            var records = await _erpService.GetPunchRecordsAsync(fromDate, toDate, employeeId);
            return Ok(records);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PunchRecord>> GetPunchRecord(int id)
        {
            var record = await _erpService.GetPunchRecordByIdAsync(id);
            if (record == null)
                return NotFound($"Punch record with ID {id} not found.");
            return Ok(record);
        }

        [HttpPost]
        public async Task<ActionResult<PunchRecord>> PostPunchRecord([FromBody] PunchRecord record)
        {
            if (record == null)
                return BadRequest("Invalid punch record data.");

            await _erpService.AddPunchRecordAsync(record);
            return CreatedAtAction(nameof(GetPunchRecord), new { id = record.Id }, record);
        }

        [HttpPost("import")]
        public async Task<ActionResult<object>> ImportFromMdb([FromQuery] string mdbFilePath)
        {
            if (string.IsNullOrEmpty(mdbFilePath))
                return BadRequest("MDB file path is required.");

            var count = await _erpService.ImportPunchRecordsFromMdbAsync(mdbFilePath);
            return Ok(new { ImportedCount = count, Message = $"{count} records imported successfully." });
        }

        [HttpPost("sync")]
        public async Task<ActionResult<object>> SyncFromDevice([FromQuery] DateTime? syncDate = null)
        {
            try
            {
                var count = await _erpService.SyncPunchRecordsFromZKDeviceAsync(syncDate);
                return Ok(new { SyncedCount = count, Message = $"{count} records synced from ZK device." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message, Details = ex.InnerException?.Message });
            }
        }
    }
}
