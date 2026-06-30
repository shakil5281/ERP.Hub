using ERPHub.Models;

namespace ERPHub.Services;

public interface ISeparationService
{
    Task<SeparationResult> RecordSeparationAsync(RecordSeparationRequest request);
    Task<SeparationResult> CancelSeparationAsync(int separationId, string cancelledBy, string reason);
    Task<List<EmployeeSeparationDto>> GetSeparatedEmployeesAsync(SeparationFilter filter);
    Task<List<Employee>> GetActiveEmployeesAsync(int? companyId = null, int? departmentId = null, int? sectionId = null);
    Task<bool> IsEligibleForProcessingAsync(string employeeId, DateTime processDate);
    Task<int> GetActiveEmployeeCountAsync();
    Task<int> GetSeparationCountAsync();
    Task<Dictionary<string, int>> GetSeparationSummaryByTypeAsync(DateTime? fromDate = null, DateTime? toDate = null);
}
