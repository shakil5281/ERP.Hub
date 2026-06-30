using ERPHub.Data;
using ERPHub.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ERPHub.Services;

public class PayrollService : IPayrollService
{
    private readonly ErpDbContext _context;
    private readonly AttendanceService _attendanceService;

    private const decimal LateDeductionAmount = 50m;

    public PayrollService(ErpDbContext context, AttendanceService attendanceService)
    {
        _context = context;
        _attendanceService = attendanceService;
    }

    public async Task<SalaryStructureDto> GetEmployeeSalaryStructureAsync(int employeeRefId, DateTime? asOfDate = null)
    {
        var date = (asOfDate ?? DateTime.Today).Date;
        var assignment = await _context.EmployeeSalaryAssignments
            .Where(a => a.EmployeeRefId == employeeRefId && a.Status == "Active"
                && a.EffectiveFrom.Date <= date
                && (a.EffectiveTo == null || a.EffectiveTo.Value.Date >= date))
            .OrderByDescending(a => a.EffectiveFrom)
            .FirstOrDefaultAsync();

        if (assignment != null)
            return SalaryStructureDto.FromAssignment(assignment);

        var emp = await _context.Employees.FindAsync(employeeRefId)
            ?? throw new InvalidOperationException("Employee not found.");
        return SalaryStructureDto.FromEmployee(emp);
    }

    public async Task AssignEmployeeSalaryFromEmployeeAsync(int employeeRefId)
    {
        var emp = await _context.Employees.FindAsync(employeeRefId);
        if (emp == null) return;

        var hasActive = await _context.EmployeeSalaryAssignments
            .AnyAsync(a => a.EmployeeRefId == employeeRefId && a.Status == "Active" && a.EffectiveTo == null);
        if (hasActive) return;

        EnsureAllowanceSplit(emp);

        await _context.EmployeeSalaryAssignments.AddAsync(new EmployeeSalaryAssignment
        {
            EmployeeRefId = employeeRefId,
            EffectiveFrom = emp.JoiningDate.Date > DateTime.Today ? emp.JoiningDate.Date : DateTime.Today,
            BasicSalary = emp.BasicSalary,
            GrossSalary = emp.GrossSalary,
            HouseRent = emp.HouseRent,
            MedicalAllowance = emp.MedicalAllowance,
            TransportAllowance = emp.TransportAllowance,
            FoodAllowance = emp.FoodAllowance,
            SpecialAllowance = emp.SpecialAllowance,
            AttendanceBonus = emp.AttendanceBonus,
            ProductionBonus = emp.ProductionBonus,
            Status = "Active"
        });
        await _context.SaveChangesAsync();
    }

    public async Task<int> CalculateDailyPayrollAsync(DateTime date, int? companyId = null)
    {
        if (await IsPayrollLockedForDateAsync(date, companyId))
            throw new InvalidOperationException("Payroll is locked for this period.");

        var employeesQuery = _context.Employees
            .Include(e => e.Shift)
            .EligibleOnDate(date);

        if (companyId.HasValue && companyId > 0)
            employeesQuery = employeesQuery.Where(e => e.CompanyId == companyId);

        var employees = await employeesQuery.ToListAsync();
        var attendanceRecords = await _context.AttendanceRecords
            .Where(a => a.AttendanceDate == date.Date)
            .ToListAsync();
        var leaves = await _context.LeaveApplications
            .Include(l => l.LeaveTypeNav)
            .Where(l => l.LeaveDate == date.Date && l.Status == "Approved")
            .ToListAsync();
        var otDeductions = await _context.OvertimeDeductions
            .Where(o => o.DeductionDate.Date == date.Date && o.Status == "Approved")
            .ToListAsync();
        var nightBills = await _context.NightBillEntries
            .Where(n => n.BillDate == date.Date && n.Status == PayrollApprovalStatus.Approved)
            .ToListAsync();
        var holidayBills = await _context.HolidayBillEntries
            .Where(h => h.BillDate == date.Date && h.Status == PayrollApprovalStatus.Approved)
            .ToListAsync();
        var advances = await _context.SalaryAdvances
            .Where(a => a.Status == PayrollApprovalStatus.Paid && a.RemainingBalance > 0)
            .ToListAsync();
        var loans = await _context.EmployeeLoans
            .Where(l => l.Status == PayrollApprovalStatus.Approved && l.RemainingBalance > 0)
            .ToListAsync();

        var existingRecords = await _context.DailySalaryRecords
            .Where(d => d.SalaryDate == date.Date)
            .ToDictionaryAsync(d => d.EmployeeId);

        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

        // Batch-load salary assignments in a single query (fix N+1)
        var employeeIds = employees.Select(e => e.Id).ToList();
        var activeAssignments = await _context.EmployeeSalaryAssignments
            .Where(a => employeeIds.Contains(a.EmployeeRefId) && a.Status == "Active"
                && a.EffectiveFrom.Date <= date.Date
                && (a.EffectiveTo == null || a.EffectiveTo.Value.Date >= date.Date))
            .GroupBy(a => a.EmployeeRefId)
            .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(a => a.EffectiveFrom).First());

        // Find employees without assignments and batch-create them
        var newAssignments = new List<EmployeeSalaryAssignment>();
        foreach (var emp in employees)
        {
            if (activeAssignments.ContainsKey(emp.Id))
                continue;
            var hasActiveOpen = await _context.EmployeeSalaryAssignments
                .AnyAsync(a => a.EmployeeRefId == emp.Id && a.Status == "Active" && a.EffectiveTo == null);
            if (!hasActiveOpen)
            {
                EnsureAllowanceSplit(emp);
                newAssignments.Add(new EmployeeSalaryAssignment
                {
                    EmployeeRefId = emp.Id,
                    EffectiveFrom = emp.JoiningDate.Date > DateTime.Today ? emp.JoiningDate.Date : DateTime.Today,
                    BasicSalary = emp.BasicSalary,
                    GrossSalary = emp.GrossSalary,
                    HouseRent = emp.HouseRent,
                    MedicalAllowance = emp.MedicalAllowance,
                    TransportAllowance = emp.TransportAllowance,
                    FoodAllowance = emp.FoodAllowance,
                    SpecialAllowance = emp.SpecialAllowance,
                    AttendanceBonus = emp.AttendanceBonus,
                    ProductionBonus = emp.ProductionBonus,
                    Status = "Active"
                });
            }
        }
        if (newAssignments.Count > 0)
        {
            _context.EmployeeSalaryAssignments.AddRange(newAssignments);
            await _context.SaveChangesAsync();
            // Reload just the new ones
            foreach (var a in newAssignments)
                activeAssignments[a.EmployeeRefId] = a;
        }

        var advancesByEmployee = advances.GroupBy(a => a.EmployeeId).ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);
        var loansByEmployee = loans.GroupBy(l => l.EmployeeId).ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        int count = 0;

        foreach (var emp in employees)
        {
            var structure = activeAssignments.TryGetValue(emp.Id, out var assignment)
                ? SalaryStructureDto.FromAssignment(assignment)
                : SalaryStructureDto.FromEmployee(emp);

            decimal dailyBasic = Math.Round(structure.BasicSalary / daysInMonth, 2);
            decimal dailyGross = Math.Round(structure.GrossSalary / daysInMonth, 2);
            decimal dailyAllowances = Math.Round(structure.TotalAllowances / daysInMonth, 2);
            decimal hourlyRate = structure.BasicSalary / 208m;
            decimal otRate = hourlyRate * 2m;
            decimal nightRate = hourlyRate * 1.5m;

            var att = attendanceRecords.FirstOrDefault(a => a.EmployeeId == emp.EmployeeId);
            var leave = leaves.FirstOrDefault(l => l.EmployeeId == emp.EmployeeId);
            var otDed = otDeductions.FirstOrDefault(o => o.EmployeeId == emp.EmployeeId);
            var night = nightBills.FirstOrDefault(n => n.EmployeeId == emp.EmployeeId);
            var holiday = holidayBills.FirstOrDefault(h => h.EmployeeId == emp.EmployeeId);

            decimal absentDed = 0, lateDed = 0, lwopDed = 0;
            decimal earnings = 0;
            string status = att?.AttendanceStatus ?? "Absent";

            if (leave != null && leave.LeaveTypeNav != null && !leave.LeaveTypeNav.IsPaid)
            {
                lwopDed = dailyGross;
                status = "Leave";
            }
            else if (AttendanceAnalytics.IsPayableAbsence(status))
            {
                absentDed = dailyGross;
            }
            else if (status is "Leave" or "On Leave")
            {
                earnings = dailyGross;
            }
            else if (status is "Holiday Worked")
            {
                earnings = dailyGross * 2m;
            }
            else if (status is "Half Day")
            {
                earnings = dailyGross * 0.5m;
            }
            else
            {
                earnings = dailyGross;
                if (status == "Late")
                    lateDed = LateDeductionAmount;
            }

            double otHours = 0;
            decimal otPay = 0;
            if (emp.OverTimeStatus && att != null && att.OvertimeMinutes > 0)
            {
                otHours = Math.Round(att.OvertimeMinutes / 60.0, 1);
                otPay = Math.Round((decimal)otHours * otRate, 2);
            }

            decimal nightPay = night?.NightPay ?? 0;
            if (nightPay == 0 && att != null && IsNightShift(emp, att))
                nightPay = Math.Round((decimal)(att.WorkedMinutes / 60.0) * nightRate, 2);

            decimal holidayPay = holiday?.HolidayPay ?? 0;

            if (otDed != null)
                lateDed += Math.Round((decimal)otDed.DeductedHours * hourlyRate, 2);

            var empAdvances = advancesByEmployee.TryGetValue(emp.EmployeeId, out var adv) ? adv : [];
            var empLoans = loansByEmployee.TryGetValue(emp.EmployeeId, out var lo) ? lo : [];
            decimal advanceDed = empAdvances.Sum(a => Math.Round(a.MonthlyDeduction / daysInMonth, 2));
            decimal loanDed = empLoans.Sum(l => Math.Round(l.MonthlyEmi / daysInMonth, 2));

            decimal totalDed = absentDed + lateDed + lwopDed + advanceDed + loanDed;
            decimal netPay = earnings + dailyAllowances + otPay + nightPay + holidayPay - totalDed;
            if (netPay < 0) netPay = 0;

            if (existingRecords.TryGetValue(emp.EmployeeId, out var existing))
            {
                if (existing.Status == "Approved") continue;
                existing.DailyBasic = dailyBasic;
                existing.DailyGross = dailyGross;
                existing.OtHours = otHours;
                existing.OtPay = otPay;
                existing.NightBillPay = nightPay;
                existing.HolidayBillPay = holidayPay;
                existing.Allowances = dailyAllowances;
                existing.AbsentDeduction = absentDed;
                existing.LateDeduction = lateDed;
                existing.LwopDeduction = lwopDed;
                existing.AdvanceDeduction = advanceDed;
                existing.LoanDeduction = loanDed;
                existing.Deductions = totalDed;
                existing.NetPay = netPay;
                existing.Status = "Processed";
            }
            else
            {
                _context.DailySalaryRecords.Add(new DailySalaryRecord
                {
                    EmployeeId = emp.EmployeeId,
                    SalaryDate = date.Date,
                    DailyBasic = dailyBasic,
                    DailyGross = dailyGross,
                    OtHours = otHours,
                    OtPay = otPay,
                    NightBillPay = nightPay,
                    HolidayBillPay = holidayPay,
                    Allowances = dailyAllowances,
                    AbsentDeduction = absentDed,
                    LateDeduction = lateDed,
                    LwopDeduction = lwopDed,
                    AdvanceDeduction = advanceDed,
                    LoanDeduction = loanDed,
                    Deductions = totalDed,
                    NetPay = netPay,
                    Status = "Processed"
                });
            }
            count++;
        }

        await _context.SaveChangesAsync();
        return count;
    }

    public async Task<List<DailySalarySheetDto>> GetDailySalarySheetAsync(DailySheetFilter filter)
    {
        var employeesQuery = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .EligibleOnDate(filter.Date)
            .AsQueryable();

        if (filter.CompanyId.HasValue && filter.CompanyId > 0)
            employeesQuery = employeesQuery.Where(e => e.CompanyId == filter.CompanyId);
        if (filter.DepartmentId.HasValue && filter.DepartmentId > 0)
            employeesQuery = employeesQuery.Where(e => e.DepartmentId == filter.DepartmentId);
        if (filter.SectionId.HasValue && filter.SectionId > 0)
            employeesQuery = employeesQuery.Where(e => e.SectionId == filter.SectionId);
        if (filter.LineId.HasValue && filter.LineId > 0)
            employeesQuery = employeesQuery.Where(e => e.LineId == filter.LineId);

        var employees = await employeesQuery.ToListAsync();
        var records = await _context.DailySalaryRecords
            .Where(d => d.SalaryDate == filter.Date.Date)
            .ToDictionaryAsync(d => d.EmployeeId);
        var attendance = await _context.AttendanceRecords
            .Where(a => a.AttendanceDate == filter.Date.Date)
            .ToDictionaryAsync(a => a.EmployeeId);

        int daysInMonth = DateTime.DaysInMonth(filter.Date.Year, filter.Date.Month);

        return employees.Select(e =>
        {
            records.TryGetValue(e.EmployeeId, out var rec);
            attendance.TryGetValue(e.EmployeeId, out var att);
            return new DailySalarySheetDto
            {
                RecordId = rec?.Id ?? 0,
                EmployeeId = e.EmployeeId,
                EmployeeName = e.EmployeeName,
                Department = e.Department?.NameEn ?? "",
                Designation = e.Designation?.NameEn ?? "",
                WorkingDays = daysInMonth,
                PresentDays = att != null && att.AttendanceStatus != "Absent" ? 1 : 0,
                AbsentDays = att != null && att.AttendanceStatus == "Absent" ? 1 : 0,
                OtHours = rec?.OtHours ?? 0,
                DailyGross = rec?.DailyGross ?? Math.Round(e.GrossSalary / daysInMonth, 2),
                DailyBasic = rec?.DailyBasic ?? Math.Round(e.BasicSalary / daysInMonth, 2),
                OtPay = rec?.OtPay ?? 0,
                NightBillPay = rec?.NightBillPay ?? 0,
                HolidayBillPay = rec?.HolidayBillPay ?? 0,
                Allowances = rec?.Allowances ?? 0,
                Deductions = rec?.Deductions ?? 0,
                NetPay = rec?.NetPay ?? 0,
                AttendanceStatus = att?.AttendanceStatus ?? "Absent",
                Status = rec?.Status ?? "Pending"
            };
        }).OrderBy(d => d.EmployeeName).ToList();
    }

    public async Task<DailySalarySummaryDto> GetDailySalarySummaryAsync(DailySheetFilter filter)
    {
        var sheet = await GetDailySalarySheetAsync(filter);
        return new DailySalarySummaryDto
        {
            TotalEmployees = sheet.Count,
            TotalBasicPay = sheet.Sum(r => r.AttendanceStatus == "Absent" ? 0m : r.DailyBasic),
            TotalOtPay = sheet.Sum(r => r.OtPay),
            TotalAllowances = sheet.Sum(r => r.Allowances),
            TotalDeductions = sheet.Sum(r => r.Deductions),
            NetDailyPayout = sheet.Sum(r => r.NetPay),
            PresentCount = sheet.Count(r => r.AttendanceStatus == "Present"),
            AbsentCount = sheet.Count(r => r.AttendanceStatus == "Absent"),
            LateCount = sheet.Count(r => r.AttendanceStatus == "Late"),
            LeaveCount = sheet.Count(r => r.AttendanceStatus is "Leave" or "On Leave")
        };
    }

    public async Task ApproveDailySalaryRecordAsync(int id)
    {
        var record = await _context.DailySalaryRecords.FindAsync(id);
        if (record != null)
        {
            record.Status = "Approved";
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateDailySalaryRecordAsync(DailySalaryRecord record)
    {
        var existing = await _context.DailySalaryRecords.FindAsync(record.Id);
        if (existing != null)
        {
            existing.DailyBasic = record.DailyBasic;
            existing.DailyGross = record.DailyGross;
            existing.OtHours = record.OtHours;
            existing.OtPay = record.OtPay;
            existing.NightBillPay = record.NightBillPay;
            existing.HolidayBillPay = record.HolidayBillPay;
            existing.Allowances = record.Allowances;
            existing.AbsentDeduction = record.AbsentDeduction;
            existing.LateDeduction = record.LateDeduction;
            existing.LwopDeduction = record.LwopDeduction;
            existing.AdvanceDeduction = record.AdvanceDeduction;
            existing.LoanDeduction = record.LoanDeduction;
            existing.Deductions = record.Deductions;
            existing.NetPay = record.NetPay;
            existing.Status = record.Status;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PayrollRun> CreateOrGetPayrollRunAsync(int companyId, int year, int month)
    {
        var existing = await _context.PayrollRuns
            .FirstOrDefaultAsync(p => p.CompanyId == companyId && p.PayrollYear == year && p.PayrollMonth == month);

        if (existing != null)
            return existing;

        var periodFrom = new DateTime(year, month, 1);
        var periodTo = periodFrom.AddMonths(1).AddDays(-1);

        var run = new PayrollRun
        {
            CompanyId = companyId,
            PayrollYear = year,
            PayrollMonth = month,
            PeriodFrom = periodFrom,
            PeriodTo = periodTo,
            Status = PayrollRunStatus.Draft
        };
        await _context.PayrollRuns.AddAsync(run);
        await _context.SaveChangesAsync();
        return run;
    }

    public async Task<PayrollRun> ExecutePayrollCalculationAsync(int runId, string userId)
    {
        var run = await _context.PayrollRuns.FindAsync(runId)
            ?? throw new InvalidOperationException("Payroll run not found.");
        if (run.Status == PayrollRunStatus.Locked)
            throw new InvalidOperationException("Payroll is locked.");

        for (var d = run.PeriodFrom; d <= run.PeriodTo; d = d.AddDays(1))
        {
            await _attendanceService.CalculateAttendanceAsync(d, d);
            await CalculateDailyPayrollAsync(d, run.CompanyId);
        }

        var existingLines = await _context.PayrollLines.Where(l => l.PayrollRunId == runId).ToListAsync();
        _context.PayrollLines.RemoveRange(existingLines);

        var employees = await _context.Employees
            .Where(e => e.CompanyId == run.CompanyId)
            .ToListAsync();

        var dailyRecords = await _context.DailySalaryRecords
            .Where(d => d.SalaryDate >= run.PeriodFrom && d.SalaryDate <= run.PeriodTo)
            .ToListAsync();

        var attendanceRecords = await _context.AttendanceRecords
            .Where(a => a.AttendanceDate >= run.PeriodFrom && a.AttendanceDate <= run.PeriodTo)
            .ToListAsync();

        int workingDays = (run.PeriodTo - run.PeriodFrom).Days + 1;

        foreach (var emp in employees)
        {
            if (!EmploymentEligibility.IsEligibleForProcessing(emp, run.PeriodFrom) &&
                !(emp.Status == EmployeeStatuses.Separation && emp.SeparationDate >= run.PeriodFrom))
                continue;

            var empDaily = dailyRecords.Where(d => d.EmployeeId == emp.EmployeeId).ToList();
            if (!empDaily.Any()) continue;

            var empAtt = attendanceRecords.Where(a => a.EmployeeId == emp.EmployeeId).ToList();
            var structure = await GetEmployeeSalaryStructureAsync(emp.Id, run.PeriodTo);

            decimal absentDays = empAtt.Count(a => a.AttendanceStatus == "Absent");
            decimal presentDays = empAtt.Count(a => a.AttendanceStatus is "Present" or "Late" or "Present + Overtime" or "Holiday Worked");
            decimal leaveDays = empAtt.Count(a => a.AttendanceStatus == "Leave");

            var line = new PayrollLine
            {
                PayrollRunId = runId,
                EmployeeRefId = emp.Id,
                EmployeeId = emp.EmployeeId,
                EmployeeName = emp.EmployeeName,
                DepartmentId = emp.DepartmentId,
                DesignationId = emp.DesignationId,
                WorkingDays = workingDays,
                PresentDays = presentDays,
                AbsentDays = absentDays,
                LeaveDays = leaveDays,
                OtHours = (decimal)empDaily.Sum(d => d.OtHours),
                BasicSalary = structure.BasicSalary,
                HouseRent = structure.HouseRent,
                MedicalAllowance = structure.MedicalAllowance,
                TransportAllowance = structure.TransportAllowance,
                FoodAllowance = structure.FoodAllowance,
                SpecialAllowance = structure.SpecialAllowance,
                OvertimePay = empDaily.Sum(d => d.OtPay),
                NightBillPay = empDaily.Sum(d => d.NightBillPay),
                HolidayBillPay = empDaily.Sum(d => d.HolidayBillPay),
                AbsentDeduction = empDaily.Sum(d => d.AbsentDeduction),
                LateDeduction = empDaily.Sum(d => d.LateDeduction),
                LwopDeduction = empDaily.Sum(d => d.LwopDeduction),
                AdvanceDeduction = empDaily.Sum(d => d.AdvanceDeduction),
                LoanDeduction = empDaily.Sum(d => d.LoanDeduction),
                AttendanceBonus = presentDays >= workingDays * 0.95m ? structure.AttendanceBonus : 0,
                ProductionBonus = structure.ProductionBonus
            };

            line.GrossEarnings = line.BasicSalary + line.HouseRent + line.MedicalAllowance +
                line.TransportAllowance + line.FoodAllowance + line.SpecialAllowance +
                line.AttendanceBonus + line.ProductionBonus +
                line.OvertimePay + line.NightBillPay + line.HolidayBillPay;

            line.TotalDeductions = line.AbsentDeduction + line.LateDeduction + line.LwopDeduction +
                line.LoanDeduction + line.AdvanceDeduction + line.TaxDeduction + line.OtherDeduction;

            line.NetSalary = line.GrossEarnings - line.TotalDeductions;
            if (line.NetSalary < 0) line.NetSalary = 0;

            line.BankAccountNumber = emp.AccountNumber;
            line.BankName = emp.BankName;
            line.BranchName = emp.BranchName;
            line.RoutingNumber = emp.RoutingNumber;

            await _context.PayrollLines.AddAsync(line);
        }

        await _context.SaveChangesAsync();

        run = await AggregateRunTotalsAsync(runId);
        run.Status = PayrollRunStatus.Calculated;
        run.CalculatedBy = userId;
        run.CalculatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return run;
    }

    public async Task<PayrollRun> VerifyPayrollAsync(int runId, string userId)
    {
        var run = await GetRunOrThrow(runId);
        EnsureStatus(run, PayrollRunStatus.Calculated);
        run.Status = PayrollRunStatus.Verified;
        run.VerifiedBy = userId;
        run.VerifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return run;
    }

    public async Task<PayrollRun> ApprovePayrollAsync(int runId, string userId)
    {
        var run = await GetRunOrThrow(runId);
        EnsureStatus(run, PayrollRunStatus.Verified);
        run.Status = PayrollRunStatus.Approved;
        run.ApprovedBy = userId;
        run.ApprovedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return run;
    }

    public async Task<PayrollRun> LockPayrollAsync(int runId, string userId)
    {
        var run = await GetRunOrThrow(runId);
        EnsureStatus(run, PayrollRunStatus.Approved);
        run.Status = PayrollRunStatus.Locked;
        run.LockedBy = userId;
        run.LockedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return run;
    }

    public Task<List<PayrollRun>> GetPayrollRunsAsync(int? companyId = null)
    {
        var q = _context.PayrollRuns.Include(p => p.Company).AsQueryable();
        if (companyId.HasValue && companyId > 0)
            q = q.Where(p => p.CompanyId == companyId);
        return q.OrderByDescending(p => p.PayrollYear).ThenByDescending(p => p.PayrollMonth).ToListAsync();
    }

    public Task<PayrollRun?> GetPayrollRunAsync(int runId) =>
        _context.PayrollRuns.Include(p => p.Company).FirstOrDefaultAsync(p => p.Id == runId);

    public Task<List<PayrollLine>> GetPayrollLinesAsync(int runId) =>
        _context.PayrollLines.Where(l => l.PayrollRunId == runId).OrderBy(l => l.EmployeeName).ToListAsync();

    public async Task<PayrollSummaryDto> GetPayrollSummaryAsync(int runId)
    {
        var lines = await GetPayrollLinesAsync(runId);
        var depts = await _context.Departments.ToDictionaryAsync(d => d.Id, d => d.NameEn);

        return new PayrollSummaryDto
        {
            TotalEmployees = lines.Count,
            GrossSalary = lines.Sum(l => l.GrossEarnings),
            TotalOvertime = lines.Sum(l => l.OvertimePay),
            TotalNightBill = lines.Sum(l => l.NightBillPay),
            TotalHolidayBill = lines.Sum(l => l.HolidayBillPay),
            TotalDeduction = lines.Sum(l => l.TotalDeductions),
            TotalNetSalary = lines.Sum(l => l.NetSalary),
            ByDepartment = lines.GroupBy(l => l.DepartmentId).Select(g => new PayrollSummaryGroupDto
            {
                GroupName = depts.GetValueOrDefault(g.Key, "Unknown"),
                EmployeeCount = g.Count(),
                GrossSalary = g.Sum(x => x.GrossEarnings),
                TotalDeduction = g.Sum(x => x.TotalDeductions),
                NetSalary = g.Sum(x => x.NetSalary)
            }).ToList()
        };
    }

    public async Task<List<PayrollProcessStepDto>> GetProcessStepsAsync(int runId)
    {
        var run = await GetRunOrThrow(runId);
        string S(int step) => run.Status switch
        {
            PayrollRunStatus.Locked => "Completed",
            PayrollRunStatus.Approved => step <= 11 ? "Completed" : "Ready",
            PayrollRunStatus.Verified => step <= 10 ? "Completed" : "Ready",
            PayrollRunStatus.Calculated => step <= 9 ? "Completed" : "Ready",
            _ => step == 1 || step == 2 ? "Completed" : "Pending"
        };

        return
        [
            new() { StepNo = 1, PhaseName = "Select Company", Description = "Company and payroll period selected", Status = S(1) },
            new() { StepNo = 2, PhaseName = "Select Payroll Month", Description = "Payroll month configured", Status = S(2) },
            new() { StepNo = 3, PhaseName = "Calculate Attendance", Description = "Attendance calculated for all days", Status = S(3) },
            new() { StepNo = 4, PhaseName = "Calculate Leave", Description = "Leave applied to attendance", Status = S(4) },
            new() { StepNo = 5, PhaseName = "Calculate Overtime", Description = "OT from attendance records", Status = S(5) },
            new() { StepNo = 6, PhaseName = "Calculate Night Bill", Description = "Night shift bills applied", Status = S(6) },
            new() { StepNo = 7, PhaseName = "Calculate Holiday Bill", Description = "Holiday work bills applied", Status = S(7) },
            new() { StepNo = 8, PhaseName = "Calculate Deductions", Description = "Advance, loan, late, absent deductions", Status = S(8) },
            new() { StepNo = 9, PhaseName = "Generate Salary", Description = "Daily and monthly payroll lines generated", Status = S(9) },
            new() { StepNo = 10, PhaseName = "Verify Payroll", Description = "Payroll verification by HR", Status = S(10) },
            new() { StepNo = 11, PhaseName = "Approve Payroll", Description = "Management approval", Status = S(11) },
            new() { StepNo = 12, PhaseName = "Lock Payroll", Description = "Lock period — no further changes", Status = S(12) }
        ];
    }

    // Advances
    public Task<List<SalaryAdvance>> GetAdvancesAsync(string? status = null)
    {
        var q = _context.SalaryAdvances.AsQueryable();
        if (!string.IsNullOrEmpty(status) && status != "All")
            q = q.Where(a => a.Status == status);
        return q.OrderByDescending(a => a.AdvanceDate).ToListAsync();
    }

    public async Task<SalaryAdvance> CreateAdvanceAsync(SalaryAdvance advance)
    {
        var emp = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == advance.EmployeeId)
            ?? throw new InvalidOperationException("Employee not found.");
        advance.EmployeeRefId = emp.Id;
        advance.EmployeeName = emp.EmployeeName;
        advance.RemainingBalance = advance.Amount;
        advance.MonthlyDeduction = advance.InstallmentCount > 0
            ? Math.Round(advance.Amount / advance.InstallmentCount, 2) : advance.Amount;
        advance.Status = PayrollApprovalStatus.Pending;
        await _context.SalaryAdvances.AddAsync(advance);
        await _context.SaveChangesAsync();
        return advance;
    }

    public async Task ApproveAdvanceAsync(int id, string approvedBy)
    {
        var adv = await _context.SalaryAdvances.FindAsync(id)
            ?? throw new InvalidOperationException("Advance not found.");
        adv.Status = PayrollApprovalStatus.Paid;
        adv.ApprovedBy = approvedBy;
        adv.ApprovedDate = DateTime.UtcNow;
        adv.PaidDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // Loans
    public Task<List<EmployeeLoan>> GetLoansAsync(string? status = null)
    {
        var q = _context.EmployeeLoans.AsQueryable();
        if (!string.IsNullOrEmpty(status) && status != "All")
            q = q.Where(l => l.Status == status);
        return q.OrderByDescending(l => l.LoanDate).ToListAsync();
    }

    public async Task<EmployeeLoan> CreateLoanAsync(EmployeeLoan loan)
    {
        var emp = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == loan.EmployeeId)
            ?? throw new InvalidOperationException("Employee not found.");
        loan.EmployeeRefId = emp.Id;
        loan.EmployeeName = emp.EmployeeName;
        decimal totalPayable = loan.LoanAmount + (loan.LoanAmount * loan.InterestRate * loan.InstallmentCount / 12m);
        loan.MonthlyEmi = loan.InstallmentCount > 0 ? Math.Round(totalPayable / loan.InstallmentCount, 2) : totalPayable;
        loan.RemainingBalance = totalPayable;
        loan.Status = PayrollApprovalStatus.Pending;
        await _context.EmployeeLoans.AddAsync(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task ApproveLoanAsync(int id, string approvedBy)
    {
        var loan = await _context.EmployeeLoans.FindAsync(id)
            ?? throw new InvalidOperationException("Loan not found.");
        loan.Status = PayrollApprovalStatus.Approved;
        loan.ApprovedBy = approvedBy;
        loan.ApprovedDate = DateTime.UtcNow;
        loan.DisbursedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // Increments
    public Task<List<SalaryIncrement>> GetIncrementsAsync(string? status = null)
    {
        var q = _context.SalaryIncrements.AsQueryable();
        if (!string.IsNullOrEmpty(status) && status != "All")
            q = q.Where(i => i.Status == status);
        return q.OrderByDescending(i => i.EffectiveDate).ToListAsync();
    }

    public async Task<SalaryIncrement> CreateIncrementAsync(SalaryIncrement increment)
    {
        var emp = await _context.Employees.FindAsync(increment.EmployeeRefId)
            ?? throw new InvalidOperationException("Employee not found.");
        increment.EmployeeId = emp.EmployeeId;
        increment.PreviousBasic = emp.BasicSalary;
        increment.PreviousGross = emp.GrossSalary;
        if (increment.IncrementAmount > 0 && increment.PreviousBasic > 0)
            increment.IncrementPercent = Math.Round(increment.IncrementAmount / increment.PreviousBasic * 100, 4);
        increment.NewBasic = increment.PreviousBasic + increment.IncrementAmount;
        increment.NewGross = increment.PreviousGross + increment.IncrementAmount;
        increment.Status = PayrollApprovalStatus.Pending;
        await _context.SalaryIncrements.AddAsync(increment);
        await _context.SaveChangesAsync();
        return increment;
    }

    public async Task ApproveIncrementAsync(int id, string approvedBy)
    {
        var inc = await _context.SalaryIncrements.FindAsync(id)
            ?? throw new InvalidOperationException("Increment not found.");
        var emp = await _context.Employees.FindAsync(inc.EmployeeRefId)
            ?? throw new InvalidOperationException("Employee not found.");

        var currentAssignment = await _context.EmployeeSalaryAssignments
            .Where(a => a.EmployeeRefId == emp.Id && a.Status == "Active" && a.EffectiveTo == null)
            .FirstOrDefaultAsync();

        if (currentAssignment != null)
            currentAssignment.EffectiveTo = inc.EffectiveDate.AddDays(-1);

        EnsureAllowanceSplit(emp);
        emp.BasicSalary = inc.NewBasic;
        emp.GrossSalary = inc.NewGross;

        await _context.EmployeeSalaryAssignments.AddAsync(new EmployeeSalaryAssignment
        {
            EmployeeRefId = emp.Id,
            EffectiveFrom = inc.EffectiveDate,
            BasicSalary = inc.NewBasic,
            GrossSalary = inc.NewGross,
            HouseRent = emp.HouseRent,
            MedicalAllowance = emp.MedicalAllowance,
            TransportAllowance = emp.TransportAllowance,
            FoodAllowance = emp.FoodAllowance,
            SpecialAllowance = emp.SpecialAllowance,
            AttendanceBonus = emp.AttendanceBonus,
            ProductionBonus = emp.ProductionBonus,
            Status = "Active",
            ApprovedBy = approvedBy,
            ApprovedDate = DateTime.UtcNow
        });

        await _context.SalaryHistories.AddAsync(new SalaryHistory
        {
            EmployeeRefId = emp.Id,
            ChangeType = inc.IncrementType,
            EffectiveDate = inc.EffectiveDate,
            BasicSalary = inc.NewBasic,
            GrossSalary = inc.NewGross,
            SourceRefId = inc.Id
        });

        inc.Status = PayrollApprovalStatus.Approved;
        inc.ApprovedBy = approvedBy;
        inc.ApprovedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // Exports
    public async Task<byte[]> ExportDailySalaryExcelAsync(DailySheetFilter filter)
    {
        var data = await GetDailySalarySheetAsync(filter);
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Daily Salary");
        var headers = new[] { "Employee ID", "Name", "Department", "Designation", "Present", "Absent", "OT Hrs", "Daily Gross", "Net Pay", "Status" };
        for (int i = 0; i < headers.Length; i++) ws.Cells[1, i + 1].Value = headers[i];
        int row = 2;
        foreach (var d in data)
        {
            ws.Cells[row, 1].Value = d.EmployeeId;
            ws.Cells[row, 2].Value = d.EmployeeName;
            ws.Cells[row, 3].Value = d.Department;
            ws.Cells[row, 4].Value = d.Designation;
            ws.Cells[row, 5].Value = d.PresentDays;
            ws.Cells[row, 6].Value = d.AbsentDays;
            ws.Cells[row, 7].Value = d.OtHours;
            ws.Cells[row, 8].Value = d.DailyGross;
            ws.Cells[row, 9].Value = d.NetPay;
            ws.Cells[row, 10].Value = d.Status;
            row++;
        }
        ws.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportMonthlySalaryExcelAsync(int runId)
    {
        var run = await GetPayrollRunAsync(runId);
        var lines = await GetPayrollLinesAsync(runId);
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Salary Sheet");
        ws.Cells[1, 1].Value = $"Salary Sheet - {run?.PayrollMonth}/{run?.PayrollYear}";
        var headers = new[] { "Employee ID", "Name", "Dept", "Gross", "OT", "Night", "Holiday", "Deductions", "Net Salary" };
        for (int i = 0; i < headers.Length; i++) ws.Cells[3, i + 1].Value = headers[i];
        int row = 4;
        foreach (var l in lines)
        {
            ws.Cells[row, 1].Value = l.EmployeeId;
            ws.Cells[row, 2].Value = l.EmployeeName;
            ws.Cells[row, 3].Value = l.DepartmentId;
            ws.Cells[row, 4].Value = l.GrossEarnings;
            ws.Cells[row, 5].Value = l.OvertimePay;
            ws.Cells[row, 6].Value = l.NightBillPay;
            ws.Cells[row, 7].Value = l.HolidayBillPay;
            ws.Cells[row, 8].Value = l.TotalDeductions;
            ws.Cells[row, 9].Value = l.NetSalary;
            row++;
        }
        ws.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportBankSheetExcelAsync(int runId) =>
        await ExportBankSheetInternalAsync(runId, "excel");

    public async Task<byte[]> ExportBankSheetCsvAsync(int runId) =>
        await ExportBankSheetInternalAsync(runId, "csv");

    public async Task<PayslipDetailDto?> GetPayslipByEmployeeAsync(string employeeId, int year, int month)
    {
        var run = await _context.PayrollRuns
            .Where(r => r.PayrollYear == year && r.PayrollMonth == month)
            .OrderByDescending(r => r.Id)
            .FirstOrDefaultAsync();

        if (run != null)
        {
            var line = await _context.PayrollLines
                .Where(l => l.PayrollRunId == run.Id && l.EmployeeId == employeeId)
                .FirstOrDefaultAsync();

            if (line != null)
            {
                var emp = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Designation)
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                return new PayslipDetailDto
                {
                    EmployeeId = line.EmployeeId,
                    EmployeeName = line.EmployeeName,
                    Department = emp?.Department?.NameEn ?? "",
                    Designation = emp?.Designation?.NameEn ?? "",
                    BankName = line.BankName ?? "",
                    BankAccountNumber = line.BankAccountNumber ?? "",
                    BranchName = line.BranchName ?? "",
                    RoutingNumber = line.RoutingNumber ?? "",
                    Year = year,
                    Month = month,
                    PresentDays = line.PresentDays,
                    AbsentDays = line.AbsentDays,
                    LeaveDays = line.LeaveDays,
                    OtHours = line.OtHours,
                    BasicSalary = line.BasicSalary,
                    HouseRent = line.HouseRent,
                    MedicalAllowance = line.MedicalAllowance,
                    TransportAllowance = line.TransportAllowance,
                    FoodAllowance = line.FoodAllowance,
                    SpecialAllowance = line.SpecialAllowance,
                    AttendanceBonus = line.AttendanceBonus,
                    ProductionBonus = line.ProductionBonus,
                    OvertimePay = line.OvertimePay,
                    NightBillPay = line.NightBillPay,
                    HolidayBillPay = line.HolidayBillPay,
                    GrossEarnings = line.GrossEarnings,
                    AbsentDeduction = line.AbsentDeduction,
                    LateDeduction = line.LateDeduction,
                    LwopDeduction = line.LwopDeduction,
                    LoanDeduction = line.LoanDeduction,
                    AdvanceDeduction = line.AdvanceDeduction,
                    TaxDeduction = line.TaxDeduction,
                    OtherDeduction = line.OtherDeduction,
                    TotalDeductions = line.TotalDeductions,
                    NetPay = line.NetSalary
                };
            }
        }

        var employee = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

        if (employee == null) return null;

        var periodStart = new DateTime(year, month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);
        int daysInMonth = DateTime.DaysInMonth(year, month);

        var attendanceRecords = await _context.AttendanceRecords
            .Where(a => a.EmployeeId == employeeId
                && a.AttendanceDate >= periodStart
                && a.AttendanceDate <= periodEnd)
            .ToListAsync();

        decimal present = attendanceRecords.Count(a => a.AttendanceStatus is "Present" or "Late");
        decimal absent = attendanceRecords.Count(a => a.AttendanceStatus == "Absent");
        decimal leave = attendanceRecords.Count(a => a.AttendanceStatus is "Leave" or "On Leave");
        decimal otHours = attendanceRecords.Sum(a => (decimal)a.OvertimeMinutes / 60);

        decimal dailyBasic = employee.BasicSalary / daysInMonth;
        decimal grossEarnings = employee.GrossSalary
            + employee.AttendanceBonus + employee.ProductionBonus;
        decimal totalDeductions = absent * dailyBasic;

        return new PayslipDetailDto
        {
            EmployeeId = employee.EmployeeId,
            EmployeeName = employee.EmployeeName,
            Department = employee.Department?.NameEn ?? "",
            Designation = employee.Designation?.NameEn ?? "",
            BankName = employee.BankName,
            BankAccountNumber = employee.AccountNumber,
            BranchName = employee.BranchName,
            RoutingNumber = employee.RoutingNumber,
            Year = year,
            Month = month,
            PresentDays = present,
            AbsentDays = absent,
            LeaveDays = leave,
            OtHours = otHours,
            BasicSalary = employee.BasicSalary,
            HouseRent = employee.HouseRent,
            MedicalAllowance = employee.MedicalAllowance,
            TransportAllowance = employee.TransportAllowance,
            FoodAllowance = employee.FoodAllowance,
            SpecialAllowance = employee.SpecialAllowance,
            AttendanceBonus = employee.AttendanceBonus,
            ProductionBonus = employee.ProductionBonus,
            OvertimePay = 0,
            NightBillPay = 0,
            HolidayBillPay = 0,
            GrossEarnings = grossEarnings,
            AbsentDeduction = absent * dailyBasic,
            LateDeduction = 0,
            LwopDeduction = 0,
            LoanDeduction = 0,
            AdvanceDeduction = 0,
            TaxDeduction = 0,
            OtherDeduction = 0,
            TotalDeductions = totalDeductions,
            NetPay = grossEarnings - totalDeductions
        };
    }

    public async Task<byte[]> GeneratePayslipPdfAsync(int payrollLineId)
    {
        var line = await _context.PayrollLines
            .Include(l => l.PayrollRun)!.ThenInclude(r => r!.Company)
            .FirstOrDefaultAsync(l => l.Id == payrollLineId)
            ?? throw new InvalidOperationException("Payroll line not found.");

        var run = line.PayrollRun!;
        var company = run.Company;

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.Content().Column(col =>
                {
                    col.Item().Text(company?.CompanyNameEn ?? "Company").Bold().FontSize(16);
                    col.Item().Text($"Payslip — {run.PayrollMonth:00}/{run.PayrollYear}").FontSize(12);
                    col.Item().PaddingVertical(10).LineHorizontal(1);
                    col.Item().Text($"Employee: {line.EmployeeName} ({line.EmployeeId})");
                    col.Item().Text($"Present: {line.PresentDays} | Absent: {line.AbsentDays} | OT Hrs: {line.OtHours:F1}");
                    col.Item().PaddingVertical(5).Text("Earnings").Bold();
                    col.Item().Text($"Basic: {line.BasicSalary:N2} | OT: {line.OvertimePay:N2} | Night: {line.NightBillPay:N2}");
                    col.Item().Text($"Allowances: {line.HouseRent + line.MedicalAllowance + line.TransportAllowance + line.FoodAllowance:N2}");
                    col.Item().PaddingVertical(5).Text("Deductions").Bold();
                    col.Item().Text($"Total Deductions: {line.TotalDeductions:N2}");
                    col.Item().PaddingVertical(10).Text($"NET PAYABLE: {line.NetSalary:N2}").Bold().FontSize(14);
                });
            });
        });
        return doc.GeneratePdf();
    }

    private async Task<byte[]> ExportBankSheetInternalAsync(int runId, string format)
    {
        var lines = await GetPayrollLinesAsync(runId);
        if (format == "csv")
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("EmployeeId,EmployeeName,BankName,BranchName,AccountNumber,RoutingNumber,NetSalary");
            foreach (var l in lines.Where(x => x.NetSalary > 0))
                sb.AppendLine($"{l.EmployeeId},{l.EmployeeName},{l.BankName},{l.BranchName},{l.BankAccountNumber},{l.RoutingNumber},{l.NetSalary:F2}");
            return System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        }
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Bank Sheet");
        var headers = new[] { "Employee ID", "Name", "Bank", "Branch", "Account", "Routing", "Net Salary" };
        for (int i = 0; i < headers.Length; i++) ws.Cells[1, i + 1].Value = headers[i];
        int row = 2;
        foreach (var l in lines.Where(x => x.NetSalary > 0))
        {
            ws.Cells[row, 1].Value = l.EmployeeId;
            ws.Cells[row, 2].Value = l.EmployeeName;
            ws.Cells[row, 3].Value = l.BankName;
            ws.Cells[row, 4].Value = l.BranchName;
            ws.Cells[row, 5].Value = l.BankAccountNumber;
            ws.Cells[row, 6].Value = l.RoutingNumber;
            ws.Cells[row, 7].Value = l.NetSalary;
            row++;
        }
        ws.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    private async Task<PayrollRun> AggregateRunTotalsAsync(int runId)
    {
        var run = await GetRunOrThrow(runId);
        var lines = await _context.PayrollLines.Where(l => l.PayrollRunId == runId).ToListAsync();
        run.TotalEmployees = lines.Count;
        run.TotalGross = lines.Sum(l => l.GrossEarnings);
        run.TotalDeductions = lines.Sum(l => l.TotalDeductions);
        run.TotalNet = lines.Sum(l => l.NetSalary);
        run.TotalOvertime = lines.Sum(l => l.OvertimePay);
        run.TotalNightBill = lines.Sum(l => l.NightBillPay);
        run.TotalHolidayBill = lines.Sum(l => l.HolidayBillPay);
        return run;
    }

    private async Task<bool> IsPayrollLockedForDateAsync(DateTime date, int? companyId)
    {
        var q = _context.PayrollRuns.Where(p =>
            p.PayrollYear == date.Year && p.PayrollMonth == date.Month &&
            p.Status == PayrollRunStatus.Locked);
        if (companyId.HasValue && companyId > 0)
            q = q.Where(p => p.CompanyId == companyId);
        return await q.AnyAsync();
    }

    private static void EnsureAllowanceSplit(Employee emp)
    {
        if (emp.GrossSalary <= 0) return;
        if (emp.BasicSalary <= 0)
            emp.BasicSalary = Math.Round(emp.GrossSalary * 0.6m, 2);
        if (emp.HouseRent <= 0)
            emp.HouseRent = Math.Round(emp.BasicSalary * 0.5m, 2);
        if (emp.MedicalAllowance <= 0)
            emp.MedicalAllowance = Math.Round(emp.GrossSalary * 0.1m, 2);
        if (emp.TransportAllowance <= 0)
            emp.TransportAllowance = Math.Round(emp.GrossSalary * 0.05m, 2);
        if (emp.FoodAllowance <= 0)
            emp.FoodAllowance = Math.Round(emp.GrossSalary * 0.05m, 2);
        if (emp.SpecialAllowance <= 0)
        {
            var used = emp.BasicSalary + emp.HouseRent + emp.MedicalAllowance + emp.TransportAllowance + emp.FoodAllowance;
            emp.SpecialAllowance = Math.Max(0, emp.GrossSalary - used);
        }
    }

    private static bool IsNightShift(Employee emp, AttendanceRecord att)
    {
        if (emp.Shift == null) return false;
        return emp.Shift.OutTime < emp.Shift.InTime ||
               att.ActualInPunch?.Hour >= 20 || att.ActualOutPunch?.Hour <= 6;
    }

    private async Task<PayrollRun> GetRunOrThrow(int runId) =>
        await _context.PayrollRuns.FindAsync(runId)
        ?? throw new InvalidOperationException("Payroll run not found.");

    private static void EnsureStatus(PayrollRun run, string expected)
    {
        if (run.Status != expected)
            throw new InvalidOperationException($"Payroll must be in '{expected}' status. Current: {run.Status}");
    }
}
