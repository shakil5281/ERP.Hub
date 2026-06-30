using ERPHub.Models;

namespace ERPHub.Services;

public interface IPayrollService
{
    Task<SalaryStructureDto> GetEmployeeSalaryStructureAsync(int employeeRefId, DateTime? asOfDate = null);
    Task AssignEmployeeSalaryFromEmployeeAsync(int employeeRefId);

    Task<int> CalculateDailyPayrollAsync(DateTime date, int? companyId = null);
    Task<List<DailySalarySheetDto>> GetDailySalarySheetAsync(DailySheetFilter filter);
    Task<DailySalarySummaryDto> GetDailySalarySummaryAsync(DailySheetFilter filter);
    Task ApproveDailySalaryRecordAsync(int id);
    Task UpdateDailySalaryRecordAsync(DailySalaryRecord record);

    Task<PayrollRun> CreateOrGetPayrollRunAsync(int companyId, int year, int month);
    Task<PayrollRun> ExecutePayrollCalculationAsync(int runId, string userId);
    Task<PayrollRun> VerifyPayrollAsync(int runId, string userId);
    Task<PayrollRun> ApprovePayrollAsync(int runId, string userId);
    Task<PayrollRun> LockPayrollAsync(int runId, string userId);
    Task<List<PayrollRun>> GetPayrollRunsAsync(int? companyId = null);
    Task<PayrollRun?> GetPayrollRunAsync(int runId);
    Task<List<PayrollLine>> GetPayrollLinesAsync(int runId);
    Task<PayrollSummaryDto> GetPayrollSummaryAsync(int runId);
    Task<List<PayrollProcessStepDto>> GetProcessStepsAsync(int runId);

    Task<List<SalaryAdvance>> GetAdvancesAsync(string? status = null);
    Task<SalaryAdvance> CreateAdvanceAsync(SalaryAdvance advance);
    Task ApproveAdvanceAsync(int id, string approvedBy);

    Task<List<EmployeeLoan>> GetLoansAsync(string? status = null);
    Task<EmployeeLoan> CreateLoanAsync(EmployeeLoan loan);
    Task ApproveLoanAsync(int id, string approvedBy);

    Task<List<SalaryIncrement>> GetIncrementsAsync(string? status = null);
    Task<SalaryIncrement> CreateIncrementAsync(SalaryIncrement increment);
    Task ApproveIncrementAsync(int id, string approvedBy);

    Task<byte[]> ExportDailySalaryExcelAsync(DailySheetFilter filter);
    Task<byte[]> ExportMonthlySalaryExcelAsync(int runId);
    Task<byte[]> ExportBankSheetExcelAsync(int runId);
    Task<byte[]> ExportBankSheetCsvAsync(int runId);
    Task<PayslipDetailDto?> GetPayslipByEmployeeAsync(string employeeId, int year, int month);
    Task<byte[]> GeneratePayslipPdfAsync(int payrollLineId);
}
