using ERPHub.Models;
using ERPHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERPHub.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payroll;

    public PayrollController(IPayrollService payroll) => _payroll = payroll;

    [HttpGet("daily-sheet")]
    public async Task<ActionResult<List<DailySalarySheetDto>>> GetDailySheet(
        [FromQuery] DateTime date,
        [FromQuery] int? companyId = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] int? sectionId = null,
        [FromQuery] int? lineId = null)
        => Ok(await _payroll.GetDailySalarySheetAsync(new DailySheetFilter
        {
            Date = date,
            CompanyId = companyId,
            DepartmentId = departmentId,
            SectionId = sectionId,
            LineId = lineId
        }));

    [HttpPost("calculate-daily")]
    public async Task<ActionResult<object>> CalculateDaily(
        [FromQuery] DateTime date,
        [FromQuery] int? companyId = null)
        => Ok(new { Processed = await _payroll.CalculateDailyPayrollAsync(date, companyId) });

    [HttpGet("daily-sheet/summary")]
    public async Task<ActionResult<DailySalarySummaryDto>> GetDailySummary(
        [FromQuery] DateTime date,
        [FromQuery] int? companyId = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] int? sectionId = null,
        [FromQuery] int? lineId = null)
        => Ok(await _payroll.GetDailySalarySummaryAsync(new DailySheetFilter
        {
            Date = date,
            CompanyId = companyId,
            DepartmentId = departmentId,
            SectionId = sectionId,
            LineId = lineId
        }));

    [HttpPost("daily-sheet/{id:int}/approve")]
    public async Task<IActionResult> ApproveDailyRecord(int id)
    {
        await _payroll.ApproveDailySalaryRecordAsync(id);
        return Ok();
    }

    [HttpPut("daily-sheet/{id:int}")]
    public async Task<IActionResult> UpdateDailyRecord(int id, [FromBody] DailySalaryRecord record)
    {
        if (id != record.Id) return BadRequest("ID mismatch");
        await _payroll.UpdateDailySalaryRecordAsync(record);
        return Ok();
    }

    [HttpGet("runs")]
    public async Task<ActionResult<List<PayrollRun>>> GetRuns([FromQuery] int? companyId = null)
        => Ok(await _payroll.GetPayrollRunsAsync(companyId));

    [HttpPost("runs")]
    public async Task<ActionResult<PayrollRun>> CreateRun([FromBody] CreatePayrollRunRequest req)
        => Ok(await _payroll.CreateOrGetPayrollRunAsync(req.CompanyId, req.Year, req.Month));

    [HttpGet("runs/{id:int}")]
    public async Task<ActionResult<PayrollRun>> GetRun(int id)
    {
        var run = await _payroll.GetPayrollRunAsync(id);
        return run == null ? NotFound() : Ok(run);
    }

    [HttpGet("runs/{id:int}/lines")]
    public async Task<ActionResult<List<PayrollLine>>> GetLines(int id)
        => Ok(await _payroll.GetPayrollLinesAsync(id));

    [HttpGet("runs/{id:int}/steps")]
    public async Task<ActionResult<List<PayrollProcessStepDto>>> GetSteps(int id)
        => Ok(await _payroll.GetProcessStepsAsync(id));

    [HttpGet("runs/{id:int}/summary")]
    public async Task<ActionResult<PayrollSummaryDto>> GetSummary(int id)
        => Ok(await _payroll.GetPayrollSummaryAsync(id));

    [HttpPost("runs/{id:int}/calculate")]
    public async Task<ActionResult<PayrollRun>> Calculate(int id, [FromBody] PayrollActionRequest? req)
        => Ok(await _payroll.ExecutePayrollCalculationAsync(id, req?.UserId ?? "Payroll"));

    [HttpPost("runs/{id:int}/verify")]
    public async Task<ActionResult<PayrollRun>> Verify(int id, [FromBody] PayrollActionRequest? req)
        => Ok(await _payroll.VerifyPayrollAsync(id, req?.UserId ?? "HR"));

    [HttpPost("runs/{id:int}/approve")]
    public async Task<ActionResult<PayrollRun>> Approve(int id, [FromBody] PayrollActionRequest? req)
        => Ok(await _payroll.ApprovePayrollAsync(id, req?.UserId ?? "Management"));

    [HttpPost("runs/{id:int}/lock")]
    public async Task<ActionResult<PayrollRun>> Lock(int id, [FromBody] PayrollActionRequest? req)
        => Ok(await _payroll.LockPayrollAsync(id, req?.UserId ?? "Management"));

    [HttpGet("advances")]
    public async Task<ActionResult<List<SalaryAdvance>>> GetAdvances([FromQuery] string? status = null)
        => Ok(await _payroll.GetAdvancesAsync(status));

    [HttpPost("advances")]
    public async Task<ActionResult<SalaryAdvance>> CreateAdvance([FromBody] SalaryAdvance advance)
        => Ok(await _payroll.CreateAdvanceAsync(advance));

    [HttpPost("advances/{id:int}/approve")]
    public async Task<IActionResult> ApproveAdvance(int id, [FromBody] PayrollActionRequest? req)
    {
        await _payroll.ApproveAdvanceAsync(id, req?.UserId ?? "HR");
        return Ok();
    }

    [HttpGet("loans")]
    public async Task<ActionResult<List<EmployeeLoan>>> GetLoans([FromQuery] string? status = null)
        => Ok(await _payroll.GetLoansAsync(status));

    [HttpPost("loans")]
    public async Task<ActionResult<EmployeeLoan>> CreateLoan([FromBody] EmployeeLoan loan)
        => Ok(await _payroll.CreateLoanAsync(loan));

    [HttpPost("loans/{id:int}/approve")]
    public async Task<IActionResult> ApproveLoan(int id, [FromBody] PayrollActionRequest? req)
    {
        await _payroll.ApproveLoanAsync(id, req?.UserId ?? "HR");
        return Ok();
    }

    [HttpGet("increments")]
    public async Task<ActionResult<List<SalaryIncrement>>> GetIncrements([FromQuery] string? status = null)
        => Ok(await _payroll.GetIncrementsAsync(status));

    [HttpPost("increments")]
    public async Task<ActionResult<SalaryIncrement>> CreateIncrement([FromBody] SalaryIncrement increment)
        => Ok(await _payroll.CreateIncrementAsync(increment));

    [HttpPost("increments/{id:int}/approve")]
    public async Task<IActionResult> ApproveIncrement(int id, [FromBody] PayrollActionRequest? req)
    {
        await _payroll.ApproveIncrementAsync(id, req?.UserId ?? "HR");
        return Ok();
    }

    [HttpGet("export/daily-sheet")]
    public async Task<IActionResult> ExportDailySheet([FromQuery] DateTime date, [FromQuery] int? companyId = null)
    {
        var bytes = await _payroll.ExportDailySalaryExcelAsync(new DailySheetFilter { Date = date, CompanyId = companyId });
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"DailySalary_{date:yyyyMMdd}.xlsx");
    }

    [HttpGet("export/salary-sheet/{runId:int}")]
    public async Task<IActionResult> ExportSalarySheet(int runId)
    {
        var bytes = await _payroll.ExportMonthlySalaryExcelAsync(runId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"SalarySheet_{runId}.xlsx");
    }

    [HttpGet("export/bank-sheet/{runId:int}")]
    public async Task<IActionResult> ExportBankSheet(int runId, [FromQuery] string format = "excel")
    {
        var bytes = format == "csv"
            ? await _payroll.ExportBankSheetCsvAsync(runId)
            : await _payroll.ExportBankSheetExcelAsync(runId);
        var contentType = format == "csv" ? "text/csv" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var ext = format == "csv" ? "csv" : "xlsx";
        return File(bytes, contentType, $"BankSheet_{runId}.{ext}");
    }

    [HttpGet("export/payslip/{lineId:int}")]
    public async Task<IActionResult> ExportPayslip(int lineId)
    {
        var bytes = await _payroll.GeneratePayslipPdfAsync(lineId);
        return File(bytes, "application/pdf", $"Payslip_{lineId}.pdf");
    }

    [HttpPost("payslip/detail")]
    public async Task<ActionResult<PayslipDetailDto?>> GetPayslipDetail([FromBody] PayslipFilterDto filter)
    {
        var result = await _payroll.GetPayslipByEmployeeAsync(filter.EmployeeId, filter.Year, filter.Month);
        if (result == null) return NotFound("No payslip data found for this employee and period.");
        return Ok(result);
    }
}

public class CreatePayrollRunRequest
{
    public int CompanyId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}

public class PayrollActionRequest
{
    public string UserId { get; set; } = "System";
}
