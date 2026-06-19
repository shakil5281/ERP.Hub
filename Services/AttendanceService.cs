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
                : await _context.Employees.Include(e => e.Shift).Where(e => e.IsActive).ToListAsync();

            var punchRecords = await _context.PunchRecords
                .Where(p => p.LogDateTime >= from.AddHours(-2) && p.LogDateTime <= to.Date.AddDays(1).AddHours(2))
                .OrderBy(p => p.LogDateTime)
                .ToListAsync();

            var holidays = await _context.Holidays.ToListAsync();
            var leaves = await _context.LeaveApplications
                .Where(l => l.Status == "Approved" && l.LeaveDate >= from && l.LeaveDate <= to)
                .ToListAsync();

            // Batch-load existing attendance records to avoid N+1 queries
            var existingRecords = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate >= from && a.AttendanceDate <= to)
                .ToListAsync();

            var existingLookup = existingRecords
                .GroupBy(a => (a.EmployeeId, a.AttendanceDate))
                .ToDictionary(g => g.Key, g => g.First());

            int calculatedCount = 0;

            for (var date = from; date <= to; date = date.AddDays(1))
            {
                foreach (var emp in employees)
                {
                    var result = CalculateForEmployee(emp, date, punchRecords, holidays, leaves);
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

        private AttendanceRecord? CalculateForEmployee(Employee emp, DateTime date, List<PunchRecord> allPunches,
            List<Holiday> holidays, List<LeaveApplication> leaves)
        {
            var isHoliday = holidays.Any(h => h.HolidayDate.Date == date.Date) ||
                (emp.Shift?.OffDay?.Contains(date.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase) ?? false);

            var isLeave = leaves.Any(l => l.EmployeeId == emp.EmployeeId && l.LeaveDate.Date == date.Date);

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

            var empPunches = allPunches
                .Where(p => p.EmployeeId == emp.EmployeeId
                    && p.LogDateTime >= windowInTime
                    && p.LogDateTime <= windowOutTime)
                .OrderBy(p => p.LogDateTime)
                .ToList();

            if (empPunches.Count == 0)
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

            var deduped = new List<DateTime> { empPunches[0].LogDateTime };
            for (int i = 1; i < empPunches.Count; i++)
            {
                if ((empPunches[i].LogDateTime - deduped[^1]).TotalMinutes >= shift.DuplicateIntervalMinutes)
                    deduped.Add(empPunches[i].LogDateTime);
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

            var firstPunch = deduped.First();
            var lastPunch = deduped.Last();

            var lateMinutes = 0;
            var earlyExitMinutes = 0;
            var overtimeMinutes = 0;
            var status = "Present";
            var remarks = "";

            var shiftInTime = date.Date.Add(shift.InTime);
            var shiftOutTime = date.Date.Add(shift.OutTime);

            if (shift.OutTime < shift.InTime)
                shiftOutTime = shiftOutTime.AddDays(1);

            var attendanceInTime = shiftInTime;
            var attendanceOutTime = shiftOutTime;

            if (firstPunch < shiftInTime)
            {
                attendanceInTime = shiftInTime;
            }
            else if (firstPunch <= shiftInTime.AddMinutes(shift.GraceInMinutes))
            {
                status = "Present";
            }
            else
            {
                lateMinutes = (int)(firstPunch - shiftInTime).TotalMinutes - shift.GraceInMinutes;
                if (lateMinutes < 0) lateMinutes = 0;
                status = "Late";
            }

            if (lastPunch < shiftOutTime)
            {
                earlyExitMinutes = (int)(shiftOutTime - lastPunch).TotalMinutes;
                if (earlyExitMinutes < 0) earlyExitMinutes = 0;
                attendanceOutTime = lastPunch;
                status = "Early Exit";
            }
            else if (lastPunch > shiftOutTime)
            {
                overtimeMinutes = (int)(lastPunch - shiftOutTime).TotalMinutes;
                attendanceOutTime = shiftOutTime;
                if (overtimeMinutes < shift.MinimumOvertimeMinutes)
                    overtimeMinutes = 0;
                else
                    status = "Present + Overtime";
            }
            else
            {
                status = "Present";
            }

            var workedMinutes = (int)(lastPunch - firstPunch).TotalMinutes - shift.BreakMinutes;
            if (workedMinutes < 0) workedMinutes = 0;

            if (isLeave)
            {
                status = "Leave";
                remarks = "Approved leave";
            }

            if (isHoliday)
            {
                if (workedMinutes > 0)
                {
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

            if (status == "Present" && workedMinutes < shift.HalfDayThresholdMinutes)
                status = "Half Day";

            if (deduped.Count == 1)
            {
                var midDay = date.Date.AddHours(12);
                if (firstPunch < midDay)
                    status = "Missing Out Punch";
                else
                    status = "Missing In Punch";
            }

            return new AttendanceRecord
            {
                EmployeeId = emp.EmployeeId,
                AttendanceDate = date.Date,
                ShiftId = shift.Id,
                ActualInPunch = firstPunch,
                ActualOutPunch = lastPunch,
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
            if (activeEmployees.HasValue)
            {
                var filteredEmpIds = await _context.Employees
                    .Where(e => e.IsActive == activeEmployees.Value)
                    .Select(e => e.EmployeeId)
                    .ToListAsync();
                query = query.Where(a => filteredEmpIds.Contains(a.EmployeeId));
            }

            return await query.OrderByDescending(a => a.AttendanceDate).ThenBy(a => a.EmployeeId).ToListAsync();
        }

        public record DepartmentSummary(string Name, int Headcount, int Present, int Absent, int Late, int Leave, double PresentRate, double AvgHours);

        public async Task<List<DepartmentSummary>> GetDepartmentSummaryAsync(DateTime date)
        {
            var departments = await _context.Departments.ToListAsync();
            var records = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate == date.Date)
                .ToListAsync();

            var result = new List<DepartmentSummary>();
            foreach (var dept in departments)
            {
                var empIds = await _context.Employees
                    .Where(e => e.DepartmentId == dept.Id)
                    .Select(e => e.EmployeeId)
                    .ToListAsync();

                var deptRecords = records.Where(r => empIds.Contains(r.EmployeeId)).ToList();
                var headcount = empIds.Count;
                var present = deptRecords.Count(r => r.AttendanceStatus is "Present" or "Present + Overtime");
                var absent = deptRecords.Count(r => r.AttendanceStatus == "Absent");
                var late = deptRecords.Count(r => r.AttendanceStatus is "Late" or "Early Exit");
                var leave = deptRecords.Count(r => r.AttendanceStatus is "Leave" or "Holiday" or "Weekly Off");
                var presentRate = headcount > 0 ? Math.Round((double)present / headcount * 100, 1) : 0;
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
    }
}
