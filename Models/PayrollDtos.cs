namespace ERPHub.Models;

public class DailySalarySheetDto
{
    public int RecordId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int WorkingDays { get; set; }
    public decimal PresentDays { get; set; }
    public decimal AbsentDays { get; set; }
    public double OtHours { get; set; }
    public decimal DailyGross { get; set; }
    public decimal DailyBasic { get; set; }
    public decimal OtPay { get; set; }
    public decimal NightBillPay { get; set; }
    public decimal HolidayBillPay { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetPay { get; set; }
    public string AttendanceStatus { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class DailySalarySummaryDto
{
    public int TotalEmployees { get; set; }
    public decimal TotalBasicPay { get; set; }
    public decimal TotalOtPay { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetDailyPayout { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public int LeaveCount { get; set; }
}

public class DailySheetFilter
{
    public DateTime Date { get; set; } = DateTime.Today;
    public int? CompanyId { get; set; }
    public int? DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public int? LineId { get; set; }
}

public class PayrollSummaryDto
{
    public int TotalEmployees { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal TotalOvertime { get; set; }
    public decimal TotalNightBill { get; set; }
    public decimal TotalHolidayBill { get; set; }
    public decimal TotalDeduction { get; set; }
    public decimal TotalNetSalary { get; set; }
    public List<PayrollSummaryGroupDto> ByDepartment { get; set; } = [];
}

public class PayrollSummaryGroupDto
{
    public string GroupName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal TotalDeduction { get; set; }
    public decimal NetSalary { get; set; }
}

public class PayrollProcessStepDto
{
    public int StepNo { get; set; }
    public string PhaseName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}

public class SalaryStructureDto
{
    public decimal BasicSalary { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal HouseRent { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal FoodAllowance { get; set; }
    public decimal SpecialAllowance { get; set; }
    public decimal AttendanceBonus { get; set; }
    public decimal ProductionBonus { get; set; }

    public decimal TotalAllowances =>
        HouseRent + MedicalAllowance + TransportAllowance + FoodAllowance + SpecialAllowance;

    public static SalaryStructureDto FromEmployee(Employee e) => new()
    {
        BasicSalary = e.BasicSalary,
        GrossSalary = e.GrossSalary,
        HouseRent = e.HouseRent,
        MedicalAllowance = e.MedicalAllowance,
        TransportAllowance = e.TransportAllowance,
        FoodAllowance = e.FoodAllowance,
        SpecialAllowance = e.SpecialAllowance,
        AttendanceBonus = e.AttendanceBonus,
        ProductionBonus = e.ProductionBonus
    };

        public static SalaryStructureDto FromAssignment(EmployeeSalaryAssignment a) => new()
        {
            BasicSalary = a.BasicSalary,
            GrossSalary = a.GrossSalary,
            HouseRent = a.HouseRent,
            MedicalAllowance = a.MedicalAllowance,
            TransportAllowance = a.TransportAllowance,
            FoodAllowance = a.FoodAllowance,
            SpecialAllowance = a.SpecialAllowance,
            AttendanceBonus = a.AttendanceBonus,
            ProductionBonus = a.ProductionBonus
        };
    }

public class PayslipDetailDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string RoutingNumber { get; set; } = string.Empty;

    public int Year { get; set; }
    public int Month { get; set; }
    public decimal PresentDays { get; set; }
    public decimal AbsentDays { get; set; }
    public decimal LeaveDays { get; set; }
    public decimal OtHours { get; set; }

    // Earnings breakdown
    public decimal BasicSalary { get; set; }
    public decimal HouseRent { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal FoodAllowance { get; set; }
    public decimal SpecialAllowance { get; set; }
    public decimal AttendanceBonus { get; set; }
    public decimal ProductionBonus { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal NightBillPay { get; set; }
    public decimal HolidayBillPay { get; set; }
    public decimal GrossEarnings { get; set; }

    // Deductions breakdown
    public decimal AbsentDeduction { get; set; }
    public decimal LateDeduction { get; set; }
    public decimal LwopDeduction { get; set; }
    public decimal LoanDeduction { get; set; }
    public decimal AdvanceDeduction { get; set; }
    public decimal TaxDeduction { get; set; }
    public decimal OtherDeduction { get; set; }
    public decimal TotalDeductions { get; set; }

    // Net
    public decimal NetPay { get; set; }
}

public class PayslipFilterDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public int Year { get; set; } = DateTime.Today.Year;
    public int Month { get; set; } = DateTime.Today.Month;
}
