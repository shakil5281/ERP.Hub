using ERPHub.Data;
using ERPHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPHub.Services
{
    public class AttendanceService
    {
        private readonly ErpDbContext _context;

        public AttendanceService(ErpDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CalculateAttendanceAsync(DateTime? fromDate = null, DateTime? toDate = null, string? employeeId = null)
        {
            fromDate ??= DateTime.Today;
            toDate ??= DateTime.Today;

            var from = fromDate.Value;
            var to = toDate.Value;

            var employees = employeeId != null
                ? await _context.Employees.Include(e => e.Shift).Where(e => e.EmployeeId == employeeId).ToListAsync()
                : await _context.Employees.Include(e => e.Shift).ToListAsync();

            var punchRecords = await _context.PunchRecords
                .Where(p => p.LogDateTime >= from.AddHours(-2) && p.LogDateTime <= to.Date.AddDays(1).AddHours(2))
                .ToListAsync();

            // Pre-group punches by PunchNumber for O(1) employee lookup
            var punchesByNumber = punchRecords
                .GroupBy(p => p.PunchNumber)
                .ToDictionary(g => g.Key, g => g.OrderBy(p => p.LogDateTime).ToList());

            // Pre-group leave applications by EmployeeId
            var leaves = await _context.LeaveApplications
                .Where(l => l.Status == "Approved" && l.LeaveDate >= from && l.LeaveDate <= to)
                .ToListAsync();
            var leavesByEmployee = leaves
                .GroupBy(l => l.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            var holidays = await _context.Holidays.ToListAsync();
            var holidayDates = holidays.Select(h => h.HolidayDate.Date).ToHashSet();

            // Batch-load existing attendance records to avoid N+1 queries
            var existingRecords = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate >= from && a.AttendanceDate <= to)
                .ToListAsync();

            var existingLookup = existingRecords
                .GroupBy(a => (a.EmployeeId, a.AttendanceDate))
                .ToDictionary(g => g.Key, g => g.First());

            int calculatedCount = 0;

            foreach (var emp in employees)
            {
                var empPunches = punchesByNumber.TryGetValue(emp.PunchNumber, out var punches)
                    ? punches
                    : [];

                for (var date = from; date <= to; date = date.AddDays(1))
                {
                    if (!EmploymentEligibility.IsEligibleForProcessing(emp, date))
                        continue;

                    var result = CalculateForEmployee(emp, date, empPunches, holidayDates, leavesByEmployee);
                    if (result == null) continue;

                    var key = (emp.EmployeeId, date);
                    if (existingLookup.TryGetValue(key, out var existing))
                    {
                        existing.ShiftId = result.ShiftId;
                        existing.ActualInPunch = result.ActualInPunch;
                        existing.ActualOutPunch = result.ActualOutPunch;
                        existing.AttendanceInTime = result.AttendanceInTime;
                        existing.AttendanceOutTime = result.AttendanceOutTime;
                        existing.LateMinutes = result.LateMinutes;
                        existing.EarlyExitMinutes = result.EarlyExitMinutes;
                        existing.WorkedMinutes = result.WorkedMinutes;
                        existing.OvertimeMinutes = result.OvertimeMinutes;
                        existing.AttendanceStatus = result.AttendanceStatus;
                        existing.Remarks = result.Remarks;
                    }
                    else
                    {
                        await _context.AttendanceRecords.AddAsync(result);
                    }

                    calculatedCount++;
                }
            }

            await _context.SaveChangesAsync();
            return calculatedCount;
        }

        private AttendanceRecord? CalculateForEmployee(Employee emp, DateTime date, List<PunchRecord> empPunches,
            HashSet<DateTime> holidayDates, Dictionary<string, List<LeaveApplication>> leavesByEmployee)
        {
            var isHoliday = holidayDates.Contains(date.Date) ||
                (emp.Shift?.OffDay?.Contains(date.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase) ?? false);

            var isLeave = leavesByEmployee.TryGetValue(emp.EmployeeId, out var empLeaves) &&
                empLeaves.Any(l => l.LeaveDate.Date == date.Date);

            if (emp.Shift == null)
            {
                if (isLeave)
                    return CreateAbsentRecord(emp, date, "Leave", "Approved leave");
                if (isHoliday)
                    return CreateAbsentRecord(emp, date,
                        emp.Shift?.OffDay?.Contains(date.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase) ?? false
                            ? "Weekly Off" : "Holiday", "");
                return CreateAbsentRecord(emp, date, "Absent", "No shift assigned");
            }

            var shift = emp.Shift;
            var windowInTime = date.Date.Add(shift.InTime).AddHours(-1);
            var windowOutTime = windowInTime.AddHours(24).AddMinutes(-1);

            if (shift.OutTime < shift.InTime)
                windowOutTime = windowOutTime.AddDays(1);

            var filteredPunches = empPunches
                .Where(p => p.LogDateTime >= windowInTime && p.LogDateTime <= windowOutTime)
                .ToList();

            if (filteredPunches.Count == 0)
            {
                if (isLeave)
                    return CreateAbsentRecord(emp, date, "Leave", "Approved leave");
                if (isHoliday)
                {
                    return CreateAbsentRecord(emp, date,
                        emp.Shift.OffDay?.Contains(date.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase) ?? false
                            ? "Weekly Off" : "Holiday", "");
                }
                return CreateAbsentRecord(emp, date, "Absent", "No punch records found");
            }

            var deduped = new List<DateTime> { filteredPunches[0].LogDateTime };
            for (int i = 1; i < filteredPunches.Count; i++)
            {
                if ((filteredPunches[i].LogDateTime - deduped[^1]).TotalMinutes >= shift.DuplicateIntervalMinutes)
                    deduped.Add(filteredPunches[i].LogDateTime);
            }

            if (deduped.Count == 0)
            {
                if (isLeave)
                    return CreateAbsentRecord(emp, date, "Leave", "Approved leave");
                if (isHoliday)
                {
                    return CreateAbsentRecord(emp, date,
                        emp.Shift.OffDay?.Contains(date.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase) ?? false
                            ? "Weekly Off" : "Holiday", "");
                }
                return CreateAbsentRecord(emp, date, "Absent", "All punches filtered as duplicates");
            }

            var shiftInTime = date.Date.Add(shift.InTime);
            var shiftOutTime = date.Date.Add(shift.OutTime);
            if (shift.OutTime < shift.InTime)
                shiftOutTime = shiftOutTime.AddDays(1);

            DateTime? actualInPunch = null;
            DateTime? actualOutPunch = null;

            if (deduped.Count == 1)
            {
                var p = deduped[0];
                if (p <= shiftInTime)
                {
                    actualInPunch = p;
                }
                else if (p >= shiftOutTime)
                {
                    actualOutPunch = p;
                }
                else
                {
                    var midpoint = shiftInTime.AddMinutes((shiftOutTime - shiftInTime).TotalMinutes / 2);
                    if (p <= midpoint)
                        actualInPunch = p;
                    else
                        actualOutPunch = p;
                }
            }
            else
            {
                var first = deduped.First();
                var last = deduped.Last();

                if (last <= shiftInTime)
                {
                    actualInPunch = first;
                }
                else if (first >= shiftOutTime)
                {
                    actualOutPunch = last;
                }
                else
                {
                    actualInPunch = first;
                    actualOutPunch = last;
                }
            }

            var lateMinutes = 0;
            var earlyExitMinutes = 0;
            var overtimeMinutes = 0;
            var status = "Present";
            var remarks = "";

            var attendanceInTime = shiftInTime;
            var attendanceOutTime = shiftOutTime;

            if (actualInPunch.HasValue)
            {
                if (actualInPunch.Value < shiftInTime)
                {
                    attendanceInTime = shiftInTime;
                }
                else if (actualInPunch.Value <= shiftInTime.AddMinutes(shift.GraceInMinutes))
                {
                    status = "Present";
                }
                else
                {
                    lateMinutes = (int)(actualInPunch.Value - shiftInTime).TotalMinutes - shift.GraceInMinutes;
                    if (lateMinutes < 0) lateMinutes = 0;
                    status = "Late";
                }
            }
            else
            {
                status = "Missing In Punch";
            }

            if (actualOutPunch.HasValue)
            {
                if (actualOutPunch.Value < shiftOutTime)
                {
                    earlyExitMinutes = (int)(shiftOutTime - actualOutPunch.Value).TotalMinutes;
                    if (earlyExitMinutes < 0) earlyExitMinutes = 0;
                    attendanceOutTime = actualOutPunch.Value;
                    if (status == "Present" || status == "Late") status = "Early Exit";
                }
                else if (actualOutPunch.Value > shiftOutTime)
                {
                    overtimeMinutes = (int)(actualOutPunch.Value - shiftOutTime).TotalMinutes;
                    attendanceOutTime = shiftOutTime;
                    if (overtimeMinutes < shift.MinimumOvertimeMinutes)
                        overtimeMinutes = 0;
                    else if (status == "Present")
                        status = "Present + Overtime";
                }
            }
            else
            {
                status = "Missing Out Punch";
            }

            var workedMinutes = 0;
            if (actualInPunch.HasValue && actualOutPunch.HasValue)
            {
                workedMinutes = (int)(actualOutPunch.Value - actualInPunch.Value).TotalMinutes - shift.BreakMinutes;
                if (workedMinutes < 0) workedMinutes = 0;
            }

            if (isLeave)
            {
                status = "Leave";
                remarks = "Approved leave";
            }

            if (isHoliday)
            {
                if (workedMinutes > 0 || actualInPunch.HasValue || actualOutPunch.HasValue)
                {
                    if (workedMinutes > 0)
                        overtimeMinutes = workedMinutes;
                    
                    status = emp.Shift?.OffDay?.Contains(date.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase) ?? false
                        ? "Weekly Off Worked"
                        : "Holiday Worked";
                }
                else
                {
                    status = emp.Shift?.OffDay?.Contains(date.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase) ?? false
                        ? "Weekly Off" : "Holiday";
                }
            }

            if (!isLeave && !isHoliday && status == "Present" && workedMinutes > 0 && workedMinutes < shift.HalfDayThresholdMinutes)
                status = "Half Day";

            return new AttendanceRecord
            {
                EmployeeId = emp.EmployeeId,
                AttendanceDate = date.Date,
                ShiftId = shift.Id,
                ActualInPunch = actualInPunch,
                ActualOutPunch = actualOutPunch,
                AttendanceInTime = attendanceInTime.TimeOfDay,
                AttendanceOutTime = attendanceOutTime.TimeOfDay,
                LateMinutes = lateMinutes,
                EarlyExitMinutes = earlyExitMinutes,
                WorkedMinutes = workedMinutes,
                OvertimeMinutes = overtimeMinutes,
                AttendanceStatus = status,
                Remarks = remarks
            };
        }

        private static AttendanceRecord CreateAbsentRecord(Employee emp, DateTime date, string status, string remarks)
        {
            return new AttendanceRecord
            {
                EmployeeId = emp.EmployeeId,
                AttendanceDate = date.Date,
                ShiftId = emp.Shift?.Id ?? 0,
                ActualInPunch = null,
                ActualOutPunch = null,
                AttendanceInTime = emp.Shift?.InTime,
                AttendanceOutTime = emp.Shift?.OutTime,
                LateMinutes = 0,
                EarlyExitMinutes = 0,
                WorkedMinutes = 0,
                OvertimeMinutes = 0,
                AttendanceStatus = status,
                Remarks = remarks
            };
        }

        public async Task<List<AttendanceRecord>> GetAttendanceRecordsAsync(
            DateTime? fromDate, DateTime? toDate, string? employeeId = null, bool? activeEmployees = null)
        {
            var query = _context.AttendanceRecords.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(a => a.AttendanceDate >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(a => a.AttendanceDate <= toDate.Value);
            if (!string.IsNullOrEmpty(employeeId))
                query = query.Where(a => a.EmployeeId == employeeId);

            // Default to active employees only (exclude Resign, Left, Close)
            var includeActive = activeEmployees ?? true;
            var filteredEmpIds = includeActive
                ? await _context.Employees.CurrentlyActive().Select(e => e.EmployeeId).ToListAsync()
                : await _context.Employees.SeparatedOnly().Select(e => e.EmployeeId).ToListAsync();
            query = query.Where(a => filteredEmpIds.Contains(a.EmployeeId));

            return await query.OrderByDescending(a => a.AttendanceDate).ThenBy(a => a.EmployeeId).ToListAsync();
        }

        public async Task<List<AbsentSummaryDto>> GetAbsentSummaryAsync(DateTime fromDate, DateTime toDate, string? employeeId = null, bool? activeEmployees = null)
        {
            var employeesQuery = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Shift)
                .AsQueryable();

            if (!string.IsNullOrEmpty(employeeId))
                employeesQuery = employeesQuery.Where(e => e.EmployeeId == employeeId);
            
            if (activeEmployees.HasValue)
            {
                employeesQuery = activeEmployees.Value
                    ? employeesQuery.CurrentlyActive()
                    : employeesQuery.SeparatedOnly();
            }
            else
            {
                employeesQuery = employeesQuery.Where(e => e.Status == EmployeeStatuses.Active);
            }

            var employees = await employeesQuery.ToListAsync();

            var records = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate >= fromDate && a.AttendanceDate <= toDate)
                .ToListAsync();

            var result = new List<AbsentSummaryDto>();

            foreach (var emp in employees)
            {
                var empRecords = records.Where(r => r.EmployeeId == emp.EmployeeId).OrderBy(r => r.AttendanceDate).ToList();
                
                int totalAbsent = empRecords.Count(r => r.AttendanceStatus == "Absent" || string.IsNullOrEmpty(r.AttendanceStatus));
                
                int continuousAbsent = 0;
                for (int i = empRecords.Count - 1; i >= 0; i--)
                {
                    if (empRecords[i].AttendanceStatus == "Absent" || string.IsNullOrEmpty(empRecords[i].AttendanceStatus))
                    {
                        continuousAbsent++;
                    }
                    else if (empRecords[i].AttendanceStatus is "Leave" or "Holiday" or "Weekly Off")
                    {
                        // Optional: skip these days without breaking the streak
                        continue;
                    }
                    else
                    {
                        // Found a present/late record, break the continuous absent streak
                        break;
                    }
                }

                if (totalAbsent > 0)
                {
                    result.Add(new AbsentSummaryDto
                    {
                        EmployeeId = emp.EmployeeId,
                        EmployeeName = emp.EmployeeName,
                        Department = emp.Department?.NameEn ?? "Unknown",
                        Designation = emp.Designation?.NameEn ?? "Unknown",
                        Shift = emp.Shift?.ShiftName ?? "Unknown",
                        TotalAbsentDays = totalAbsent,
                        ContinuousAbsentDays = continuousAbsent
                    });
                }
            }

            return result.OrderByDescending(r => r.TotalAbsentDays).ThenBy(r => r.EmployeeId).ToList();
        }

        public record DepartmentSummary(string Name, int Headcount, int Present, int Absent, int Late, int Leave, double PresentRate, double AvgHours);

        public async Task<List<DepartmentSummary>> GetDepartmentSummaryAsync(DateTime date)
        {
            var departments = await _context.Departments.ToListAsync();
            var records = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate == date.Date)
                .ToListAsync();

            // Load all employee-department mappings in a single query (fix N+1)
            var employeesByDept = await _context.Employees
                .GroupBy(e => e.DepartmentId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(e => e.EmployeeId).ToList());

            var recordsByEmployee = records
                .GroupBy(r => r.EmployeeId)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var result = new List<DepartmentSummary>();
            foreach (var dept in departments)
            {
                if (!employeesByDept.TryGetValue(dept.Id, out var empIds))
                    continue;

                var headcount = empIds.Count;
                int present = 0;
                int absent = 0;
                int late = 0;
                int leave = 0;

                foreach (var empId in empIds)
                {
                    if (recordsByEmployee.TryGetValue(empId, out var r))
                    {
                        if (AttendanceAnalytics.IsPresent(r.AttendanceStatus))
                            present++;
                        if (AttendanceAnalytics.IsLate(r.AttendanceStatus))
                            late++;
                        if (AttendanceAnalytics.IsLeave(r.AttendanceStatus))
                            leave++;
                        if (AttendanceAnalytics.IsAbsent(r.AttendanceStatus))
                            absent++;
                    }
                    else
                    {
                        absent++;
                    }
                }

                var presentRate = headcount > 0 ? Math.Round((double)present / headcount * 100, 1) : 0;
                var deptRecords = records.Where(r => empIds.Contains(r.EmployeeId)).ToList();
                var avgHours = deptRecords.Count > 0
                    ? Math.Round(deptRecords.Average(r => r.WorkedMinutes) / 60.0, 1)
                    : 0;

                result.Add(new DepartmentSummary(
                    dept.NameEn, headcount, present, absent, late, leave, presentRate, avgHours));
            }

            return result.OrderByDescending(d => d.PresentRate).ToList();
        }

        public async Task<List<Holiday>> GetHolidaysAsync()
        {
            return await _context.Holidays.OrderBy(h => h.HolidayDate).ToListAsync();
        }

        public async Task AddHolidayAsync(Holiday holiday)
        {
            _context.Holidays.Add(holiday);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHolidayAsync(int id)
        {
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday != null)
            {
                _context.Holidays.Remove(holiday);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAttendanceRecordPunchAsync(int id, DateTime? inPunch, DateTime? outPunch, string reason)
        {
            var record = await _context.AttendanceRecords.FindAsync(id);
            if (record == null) return;

            record.ActualInPunch = inPunch;
            record.ActualOutPunch = outPunch;
            if (!string.IsNullOrEmpty(reason))
            {
                record.Remarks = reason;
            }

            var emp = await _context.Employees.Include(e => e.Shift).FirstOrDefaultAsync(e => e.EmployeeId == record.EmployeeId);
            if (emp != null && emp.Shift != null)
            {
                var shift = emp.Shift;
                var shiftInTime = record.AttendanceDate.Date.Add(shift.InTime);
                var shiftOutTime = record.AttendanceDate.Date.Add(shift.OutTime);
                if (shift.OutTime < shift.InTime)
                    shiftOutTime = shiftOutTime.AddDays(1);

                DateTime firstPunch = inPunch ?? shiftInTime;
                DateTime lastPunch = outPunch ?? shiftOutTime;

                var lateMinutes = 0;
                var earlyExitMinutes = 0;
                var overtimeMinutes = 0;
                var status = "Present";

                if (inPunch.HasValue)
                {
                    if (firstPunch > shiftInTime.AddMinutes(shift.GraceInMinutes))
                    {
                        lateMinutes = (int)(firstPunch - shiftInTime).TotalMinutes - shift.GraceInMinutes;
                        if (lateMinutes < 0) lateMinutes = 0;
                        status = "Late";
                    }
                }

                if (outPunch.HasValue)
                {
                    if (lastPunch < shiftOutTime)
                    {
                        earlyExitMinutes = (int)(shiftOutTime - lastPunch).TotalMinutes;
                        if (earlyExitMinutes < 0) earlyExitMinutes = 0;
                        status = "Early Exit";
                    }
                    else if (lastPunch > shiftOutTime)
                    {
                        overtimeMinutes = (int)(lastPunch - shiftOutTime).TotalMinutes;
                        if (overtimeMinutes < shift.MinimumOvertimeMinutes)
                            overtimeMinutes = 0;
                        else
                            status = "Present + Overtime";
                    }
                }

                var workedMinutes = (int)(lastPunch - firstPunch).TotalMinutes - shift.BreakMinutes;
                if (workedMinutes < 0) workedMinutes = 0;

                record.LateMinutes = lateMinutes;
                record.EarlyExitMinutes = earlyExitMinutes;
                record.WorkedMinutes = workedMinutes;
                record.OvertimeMinutes = overtimeMinutes;
                record.AttendanceStatus = status;
            }
            else
            {
                record.AttendanceStatus = "Present";
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAbsentStatusAsync(int id, string status, string remarks, string excusedBy)
        {
            var record = await _context.AttendanceRecords.FindAsync(id);
            if (record != null)
            {
                record.AttendanceStatus = status;
                record.Remarks = remarks + (string.IsNullOrEmpty(excusedBy) ? "" : $" (Excused By: {excusedBy})");
                await _context.SaveChangesAsync();
            }
        }
    }
}
