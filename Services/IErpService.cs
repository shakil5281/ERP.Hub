using ERPHub.Models;

namespace ERPHub.Services
{
    public interface IErpService
    {
        // Products (Inventory)
        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        // Invoices (Sales)
        Task<List<Invoice>> GetInvoicesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task AddInvoiceAsync(Invoice invoice);
        Task UpdateInvoiceAsync(Invoice invoice);
        Task DeleteInvoiceAsync(int id);

        // Tasks (Kanban)
        Task<List<ProjectTask>> GetTasksAsync();
        Task<ProjectTask?> GetTaskByIdAsync(int id);
        Task AddTaskAsync(ProjectTask task);
        Task UpdateTaskAsync(ProjectTask task);
        Task UpdateTaskStatusAsync(int taskId, Models.TaskStatus status);
        Task DeleteTaskAsync(int id);

        // Companies
        Task<List<Company>> GetCompaniesAsync();
        Task<Company?> GetCompanyByIdAsync(int id);
        Task AddCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);
        Task DeleteCompanyAsync(int id);
        Task SeedDemoCompaniesAsync();
        Task RemoveDemoCompaniesAsync();

        // Groups
        Task<List<BusinessGroup>> GetBusinessGroupsAsync();
        Task<BusinessGroup?> GetBusinessGroupByIdAsync(int id);
        Task AddBusinessGroupAsync(BusinessGroup group);
        Task UpdateBusinessGroupAsync(BusinessGroup group);
        Task DeleteBusinessGroupAsync(int id);

        // Dashboard/ERP Statistics
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalStockAsync();
        Task<int> GetPendingTasksCountAsync();
        Task<int> GetPaidInvoicesCountAsync();

        // Organogram Tree
        Task<List<CompanyNodeDto>> GetOrganogramTreeAsync();

        // Shifts
        Task<List<Shift>> GetShiftsAsync();
        Task<Shift?> GetShiftByIdAsync(int id);
        Task AddShiftAsync(Shift shift);
        Task UpdateShiftAsync(Shift shift);
        Task DeleteShiftAsync(int id);

        // Employees
        Task<List<Employee>> GetEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        Task<ImportResultDto> ImportEmployeesFromExcelAsync(System.IO.Stream fileStream);
        Task<ImportResultDto> ImportOrganogramFromExcelAsync(System.IO.Stream fileStream);

        // Lookups
        Task<List<Department>> GetDepartmentsAsync();
        Task<List<Section>> GetSectionsAsync();
        Task<List<Designation>> GetDesignationsAsync();
        Task<List<Line>> GetLinesAsync();

        // Punch Records (ZK Device)
        Task<List<PunchRecord>> GetPunchRecordsAsync(DateTime? fromDate = null, DateTime? toDate = null, string? employeeId = null);
        Task<PunchRecord?> GetPunchRecordByIdAsync(int id);
        Task<int> ImportPunchRecordsFromMdbAsync(string mdbFilePath, DateTime? syncDate = null);
        Task<int> SyncPunchRecordsFromZKDeviceAsync(DateTime? syncDate = null);
        Task<int> SyncPunchRecordsFromZKDeviceAsync(DateTime? fromDate, DateTime? toDate);

        // Manual Punch Logs Operations
        Task<List<ManualPunchLog>> GetManualPunchLogsAsync();
        Task AddManualPunchLogAsync(ManualPunchLog log);
        Task ApproveManualPunchLogAsync(int id);
        Task DeleteManualPunchLogAsync(int id);

        // Overtime Deductions Operations
        Task<List<OvertimeDeduction>> GetOvertimeDeductionsAsync();
        Task AddOvertimeDeductionAsync(OvertimeDeduction deduction);
        Task UpdateOvertimeDeductionAsync(OvertimeDeduction deduction);
        Task ApproveOvertimeDeductionAsync(int id);

        // Daily Salary Records Operations
        Task<List<DailySalaryRecord>> GetDailySalaryRecordsAsync(DateTime date);
        Task CalculateDailySalariesAsync(DateTime date);
        Task UpdateDailySalaryRecordAsync(DailySalaryRecord record);
        Task ApproveDailySalaryRecordAsync(int id);

        // Manpower
        Task<List<Manpower>> GetManpowersAsync();
        Task<Manpower?> GetManpowerByIdAsync(int id);
        Task AddManpowerAsync(Manpower manpower);
        Task UpdateManpowerAsync(Manpower manpower);
        Task DeleteManpowerAsync(int id);
        Task RecalculateManpowerAsync();

        // Manpower Requirements
        Task<List<ManpowerRequirement>> GetManpowerRequirementsAsync();
        Task<ManpowerRequirement?> GetManpowerRequirementByIdAsync(int id);
        Task AddManpowerRequirementAsync(ManpowerRequirement requirement);
        Task UpdateManpowerRequirementAsync(ManpowerRequirement requirement);
        Task DeleteManpowerRequirementAsync(int id);

        // Separations
        Task<List<Separation>> GetSeparationsAsync();
        Task<Separation?> GetSeparationByIdAsync(int id);
        Task AddSeparationAsync(Separation separation);
        Task UpdateSeparationAsync(Separation separation);
        Task DeleteSeparationAsync(int id);

        // Leave Types
        Task<List<LeaveType>> GetLeaveTypesAsync();
        Task<LeaveType?> GetLeaveTypeByIdAsync(int id);
        Task AddLeaveTypeAsync(LeaveType leaveType);
        Task UpdateLeaveTypeAsync(LeaveType leaveType);
        Task DeleteLeaveTypeAsync(int id);

        // Leave Applications
        Task<List<LeaveApplication>> GetLeaveApplicationsAsync(int? month = null, int? year = null, string? status = null);
        Task<LeaveApplication?> GetLeaveApplicationByIdAsync(int id);
        Task AddLeaveApplicationAsync(LeaveApplication application);
        Task UpdateLeaveApplicationAsync(LeaveApplication application);
        Task ApproveLeaveApplicationAsync(int id, string approvedBy);
        Task RejectLeaveApplicationAsync(int id, string rejectedBy, string reason);
        Task DeleteLeaveApplicationAsync(int id);
    }
}
