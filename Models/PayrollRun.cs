using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models;

[Table("payroll_runs")]
public class PayrollRun
{
    [Key]
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company? Company { get; set; }

    public int PayrollMonth { get; set; }
    public int PayrollYear { get; set; }
    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = PayrollRunStatus.Draft;

    public int TotalEmployees { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalGross { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDeductions { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalNet { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalOvertime { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalNightBill { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalHolidayBill { get; set; }

    [MaxLength(100)]
    public string? CalculatedBy { get; set; }
    public DateTime? CalculatedDate { get; set; }

    [MaxLength(100)]
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedDate { get; set; }

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }

    [MaxLength(100)]
    public string? LockedBy { get; set; }
    public DateTime? LockedDate { get; set; }

    public ICollection<PayrollLine> Lines { get; set; } = [];
}

[Table("payroll_lines")]
public class PayrollLine
{
    [Key]
    public int Id { get; set; }

    public int PayrollRunId { get; set; }
    public PayrollRun? PayrollRun { get; set; }

    public int EmployeeRefId { get; set; }

    [Required, MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string EmployeeName { get; set; } = string.Empty;

    public int DepartmentId { get; set; }
    public int DesignationId { get; set; }

    public int WorkingDays { get; set; }
    public decimal PresentDays { get; set; }
    public decimal AbsentDays { get; set; }
    public decimal LeaveDays { get; set; }
    public decimal OtHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasicSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HouseRent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MedicalAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TransportAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal FoodAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SpecialAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AttendanceBonus { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ProductionBonus { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OvertimePay { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NightBillPay { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HolidayBillPay { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GrossEarnings { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AbsentDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LateDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LwopDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LoanDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AdvanceDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OtherDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDeductions { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NetSalary { get; set; }

    [MaxLength(50)]
    public string? BankAccountNumber { get; set; }

    [MaxLength(100)]
    public string? BankName { get; set; }

    [MaxLength(100)]
    public string? BranchName { get; set; }

    [MaxLength(30)]
    public string? RoutingNumber { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = "Calculated";
}
