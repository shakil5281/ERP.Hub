using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models;

[Table("salary_advances")]
public class SalaryAdvance
{
    [Key]
    public int Id { get; set; }

    public int EmployeeRefId { get; set; }
    public Employee? Employee { get; set; }

    [MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string EmployeeName { get; set; } = string.Empty;

    public DateTime AdvanceDate { get; set; } = DateTime.Today;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public int InstallmentCount { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RemainingBalance { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = PayrollApprovalStatus.Pending;

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? PaidDate { get; set; }

    [MaxLength(500)]
    public string Remarks { get; set; } = string.Empty;
}

[Table("employee_loans")]
public class EmployeeLoan
{
    [Key]
    public int Id { get; set; }

    public int EmployeeRefId { get; set; }
    public Employee? Employee { get; set; }

    [MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string EmployeeName { get; set; } = string.Empty;

    public DateTime LoanDate { get; set; } = DateTime.Today;

    [Column(TypeName = "decimal(18,2)")]
    public decimal LoanAmount { get; set; }

    [Column(TypeName = "decimal(8,4)")]
    public decimal InterestRate { get; set; }

    public int InstallmentCount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyEmi { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RemainingBalance { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = PayrollApprovalStatus.Pending;

    public DateTime? DisbursedDate { get; set; }

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }

    [MaxLength(500)]
    public string Remarks { get; set; } = string.Empty;
}

[Table("salary_increments")]
public class SalaryIncrement
{
    [Key]
    public int Id { get; set; }

    public int EmployeeRefId { get; set; }
    public Employee? Employee { get; set; }

    [MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    [MaxLength(30)]
    public string IncrementType { get; set; } = "Annual";

    public DateTime EffectiveDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PreviousBasic { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PreviousGross { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal IncrementAmount { get; set; }

    [Column(TypeName = "decimal(8,4)")]
    public decimal IncrementPercent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NewBasic { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NewGross { get; set; }

    [MaxLength(500)]
    public string Remarks { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Status { get; set; } = PayrollApprovalStatus.Pending;

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
}

[Table("salary_history")]
public class SalaryHistory
{
    [Key]
    public int Id { get; set; }

    public int EmployeeRefId { get; set; }

    [MaxLength(30)]
    public string ChangeType { get; set; } = string.Empty;

    public DateTime EffectiveDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasicSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GrossSalary { get; set; }

    public int? SourceRefId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

[Table("night_bill_entries")]
public class NightBillEntry
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    public DateTime BillDate { get; set; }

    public decimal NightHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NightPay { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = PayrollApprovalStatus.Approved;
}

[Table("holiday_bill_entries")]
public class HolidayBillEntry
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    public DateTime BillDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HolidayPay { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = PayrollApprovalStatus.Approved;
}
