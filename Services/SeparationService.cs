using ERPHub.Data;
using ERPHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPHub.Services;

public class SeparationService : ISeparationService
{
    private readonly ErpDbContext _context;
    private readonly IErpService _erpService;

    public SeparationService(ErpDbContext context, IErpService erpService)
    {
        _context = context;
        _erpService = erpService;
    }

    public async Task<SeparationResult> RecordSeparationAsync(RecordSeparationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EmployeeId))
            return Fail("Employee ID is required.");

        if (!SeparationTypes.IsValid(request.SeparationType))
            return Fail($"Invalid separation type. Use: {string.Join(", ", SeparationTypes.All)}.");

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId);

        if (employee == null)
            return Fail($"Employee '{request.EmployeeId}' not found.");

        if (employee.Status == EmployeeStatuses.Separation)
            return Fail($"Employee '{request.EmployeeId}' is already separated.");

        if (request.SeparationDate.Date < employee.JoiningDate.Date)
            return Fail("Separation date cannot be before joining date.");

        var hasActiveSeparation = await _context.Separations
            .AnyAsync(s => s.EmployeeRefId == employee.Id && !s.IsCancelled);

        if (hasActiveSeparation)
            return Fail("An active separation record already exists for this employee.");

        var approvedDate = request.ApprovedDate ?? DateTime.UtcNow;

        var separation = new Separation
        {
            EmployeeRefId = employee.Id,
            EmployeeId = employee.EmployeeId,
            EmployeeName = employee.EmployeeName,
            DepartmentId = employee.DepartmentId,
            SectionId = employee.SectionId,
            DesignationId = employee.DesignationId,
            CompanyId = employee.CompanyId,
            SeparationType = request.SeparationType,
            SeparationDate = request.SeparationDate.Date,
            Reason = request.Reason,
            Remarks = request.Remarks,
            ApprovedBy = request.ApprovedBy,
            ApprovedDate = approvedDate,
            CreatedBy = request.CreatedBy,
            CreatedDate = DateTime.UtcNow,
            Status = SeparationWorkflowStatus.Recorded
        };

        employee.Status = EmployeeStatuses.Separation;
        employee.SeparationDate = request.SeparationDate.Date;
        employee.SeparationType = request.SeparationType;
        employee.SeparationReason = request.Reason;
        employee.SeparationRemarks = request.Remarks;
        employee.SeparationApprovedBy = request.ApprovedBy;
        employee.SeparationApprovedDate = approvedDate;

        await CancelFutureLeaveAsync(employee.EmployeeId, request.SeparationDate.Date);

        await _context.Separations.AddAsync(separation);
        await _context.SaveChangesAsync();
        await _erpService.RecalculateManpowerAsync();

        return new SeparationResult
        {
            Success = true,
            Message = "Separation recorded successfully.",
            Separation = separation
        };
    }

    public async Task<SeparationResult> CancelSeparationAsync(int separationId, string cancelledBy, string reason)
    {
        var separation = await _context.Separations
            .Include(s => s.Employee)
            .FirstOrDefaultAsync(s => s.Id == separationId);

        if (separation == null)
            return Fail("Separation record not found.");

        if (separation.IsCancelled)
            return Fail("Separation is already cancelled.");

        if (separation.IsSettled)
            return Fail("Cannot cancel a settled separation.");

        separation.IsCancelled = true;
        separation.Status = SeparationWorkflowStatus.Cancelled;
        separation.Remarks = string.IsNullOrEmpty(separation.Remarks)
            ? $"Cancelled by {cancelledBy}: {reason}"
            : $"{separation.Remarks} | Cancelled by {cancelledBy}: {reason}";
        separation.UpdatedAt = DateTime.UtcNow;

        var employee = separation.Employee
            ?? await _context.Employees.FindAsync(separation.EmployeeRefId);

        if (employee != null)
        {
            employee.Status = EmployeeStatuses.Active;
            employee.SeparationDate = null;
            employee.SeparationType = null;
            employee.SeparationReason = null;
            employee.SeparationRemarks = null;
            employee.SeparationApprovedBy = null;
            employee.SeparationApprovedDate = null;
        }

        await _context.SaveChangesAsync();
        await _erpService.RecalculateManpowerAsync();

        return new SeparationResult
        {
            Success = true,
            Message = "Separation cancelled. Employee restored to Active.",
            Separation = separation
        };
    }

    public async Task<List<EmployeeSeparationDto>> GetSeparatedEmployeesAsync(SeparationFilter filter)
    {
        var query = _context.Separations
            .Include(s => s.Department)
            .Include(s => s.Section)
            .Include(s => s.Employee)!.ThenInclude(e => e!.Company)
            .AsQueryable();

        if (!filter.IncludeCancelled)
            query = query.Where(s => !s.IsCancelled);

        if (!string.IsNullOrEmpty(filter.SeparationType) && filter.SeparationType != "All")
            query = query.Where(s => s.SeparationType == filter.SeparationType);

        if (filter.CompanyId.HasValue && filter.CompanyId > 0)
            query = query.Where(s => s.CompanyId == filter.CompanyId);

        if (filter.DepartmentId.HasValue && filter.DepartmentId > 0)
            query = query.Where(s => s.DepartmentId == filter.DepartmentId);

        if (filter.SectionId.HasValue && filter.SectionId > 0)
            query = query.Where(s => s.SectionId == filter.SectionId);

        if (filter.FromDate.HasValue)
            query = query.Where(s => s.SeparationDate >= filter.FromDate.Value.Date);

        if (filter.ToDate.HasValue)
            query = query.Where(s => s.SeparationDate <= filter.ToDate.Value.Date);

        return await query
            .OrderByDescending(s => s.SeparationDate)
            .Select(s => new EmployeeSeparationDto
            {
                SeparationId = s.Id,
                EmployeeId = s.EmployeeId,
                EmployeeName = s.EmployeeName,
                SeparationType = s.SeparationType,
                SeparationDate = s.SeparationDate,
                Reason = s.Reason,
                DepartmentName = s.Department != null ? s.Department.NameEn : "",
                SectionName = s.Section != null ? s.Section.NameEn : "",
                CompanyName = s.Employee != null && s.Employee.Company != null ? s.Employee.Company.CompanyNameEn : "",
                IsSettled = s.IsSettled,
                CreatedDate = s.CreatedDate
            })
            .ToListAsync();
    }

    public async Task<List<Employee>> GetActiveEmployeesAsync(int? companyId = null, int? departmentId = null, int? sectionId = null)
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Section)
            .Include(e => e.Designation)
            .CurrentlyActive()
            .AsQueryable();

        if (companyId.HasValue && companyId > 0)
            query = query.Where(e => e.CompanyId == companyId);

        if (departmentId.HasValue && departmentId > 0)
            query = query.Where(e => e.DepartmentId == departmentId);

        if (sectionId.HasValue && sectionId > 0)
            query = query.Where(e => e.SectionId == sectionId);

        return await query.OrderBy(e => e.EmployeeName).ToListAsync();
    }

    public async Task<bool> IsEligibleForProcessingAsync(string employeeId, DateTime processDate)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        return employee != null && EmploymentEligibility.IsEligibleForProcessing(employee, processDate);
    }

    public Task<int> GetActiveEmployeeCountAsync() =>
        _context.Employees.CurrentlyActive().CountAsync();

    public Task<int> GetSeparationCountAsync() =>
        _context.Employees.SeparatedOnly().CountAsync();

    public async Task<Dictionary<string, int>> GetSeparationSummaryByTypeAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.Separations.Where(s => !s.IsCancelled);

        if (fromDate.HasValue)
            query = query.Where(s => s.SeparationDate >= fromDate.Value.Date);
        if (toDate.HasValue)
            query = query.Where(s => s.SeparationDate <= toDate.Value.Date);

        var grouped = await query
            .GroupBy(s => s.SeparationType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        return SeparationTypes.All.ToDictionary(
            t => t,
            t => grouped.FirstOrDefault(g => g.Type == t)?.Count ?? 0);
    }

    private async Task CancelFutureLeaveAsync(string employeeId, DateTime separationDate)
    {
        var pendingLeave = await _context.LeaveApplications
            .Where(l => l.EmployeeId == employeeId
                && l.Status == "Pending"
                && l.LeaveDate.Date > separationDate)
            .ToListAsync();

        foreach (var leave in pendingLeave)
        {
            leave.Status = "Cancelled";
            leave.UpdatedAt = DateTime.UtcNow;
            leave.Reason = string.IsNullOrEmpty(leave.Reason)
                ? "Cancelled due to employee separation"
                : $"{leave.Reason} (Cancelled due to separation)";
        }
    }

    private async Task<decimal> CalculateLeaveEncashmentAsync(Employee employee, decimal dailyRate)
    {
        var annualLeaveType = await _context.LeaveTypes
            .FirstOrDefaultAsync(lt => lt.Code == "AL" || lt.Name.Contains("Annual"));

        if (annualLeaveType == null)
            return 0;

        var yearStart = new DateTime(DateTime.Today.Year, 1, 1);
        var usedDays = await _context.LeaveApplications
            .Where(l => l.EmployeeId == employee.EmployeeId
                && l.Status == "Approved"
                && l.LeaveTypeId == annualLeaveType.Id
                && l.LeaveDate >= yearStart)
            .SumAsync(l => (decimal?)l.TotalDays) ?? 0;

        decimal unusedDays = Math.Max(0, annualLeaveType.MaxDaysPerYear - usedDays);
        return Math.Round(unusedDays * dailyRate, 2);
    }

    private static SeparationResult Fail(string message) =>
        new() { Success = false, Message = message };
}
