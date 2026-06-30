using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models;

[Table("employee_salary_assignments")]
public class EmployeeSalaryAssignment
{
    [Key]
    public int Id { get; set; }

    public int EmployeeRefId { get; set; }
    public Employee? Employee { get; set; }

    public int? GradeId { get; set; }

    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
    public DateTime? EffectiveTo { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasicSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GrossSalary { get; set; }

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

    [MaxLength(30)]
    public string Status { get; set; } = "Active";

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }
}

[Table("salary_grades")]
public class SalaryGrade
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string GradeCode { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string GradeName { get; set; } = string.Empty;

    public int CompanyId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinGross { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MaxGross { get; set; }

    public bool IsActive { get; set; } = true;
}
