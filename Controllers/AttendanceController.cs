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
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;

        public AttendanceController(AttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<object>> CalculateAttendance(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? employeeId)
        {
            try
            {
                var count = await _attendanceService.CalculateAttendanceAsync(fromDate, toDate, employeeId);
                return Ok(new { CalculatedCount = count, Message = $"{count} attendance records processed." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message, Details = ex.InnerException?.Message });
            }
        }

        [HttpGet("records")]
        public async Task<ActionResult<IEnumerable<AttendanceRecord>>> GetRecords(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? employeeId,
            [FromQuery] bool? activeEmployees = null)
        {
            var records = await _attendanceService.GetAttendanceRecordsAsync(fromDate, toDate, employeeId, activeEmployees);
            return Ok(records);
        }

        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary([FromQuery] DateTime? date)
        {
            date ??= DateTime.Today;
            var summary = await _attendanceService.GetDepartmentSummaryAsync(date.Value);
            return Ok(summary);
        }

        // GET /api/attendance/absent-counts?fromDate=2026-05-01&toDate=2026-05-31
        // Returns { "EMP-001": 3, "EMP-002": 1, ... }
        [HttpGet("absent-counts")]
        public async Task<ActionResult<Dictionary<string, int>>> GetAbsentCounts(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            // Default: last full calendar month
            var lastMonth = DateTime.Today.AddMonths(-1);
            var from = fromDate?.Date ?? new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var to   = toDate?.Date   ?? new DateTime(lastMonth.Year, lastMonth.Month,
                           DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));

            var records = await _attendanceService.GetAttendanceRecordsAsync(from, to, null, null);
            var counts  = records
                .Where(r => r.AttendanceStatus == "Absent")
                .GroupBy(r => r.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Count());

            return Ok(counts);
        }

        [HttpGet("holidays")]
        public async Task<ActionResult<IEnumerable<Holiday>>> GetHolidays()
        {
            var holidays = await _attendanceService.GetHolidaysAsync();
            return Ok(holidays);
        }

        [HttpPost("holidays")]
        public async Task<ActionResult<Holiday>> AddHoliday([FromBody] Holiday holiday)
        {
            if (holiday == null)
                return BadRequest("Invalid holiday data.");

            await _attendanceService.AddHolidayAsync(holiday);
            return CreatedAtAction(nameof(GetHolidays), new { id = holiday.Id }, holiday);
        }

        [HttpDelete("holidays/{id:int}")]
        public async Task<ActionResult> DeleteHoliday(int id)
        {
            await _attendanceService.DeleteHolidayAsync(id);
            return NoContent();
        }
    }
}
