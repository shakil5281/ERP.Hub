using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        public int PunchNumber { get; set; }

        [Required]
        public string EmployeeName { get; set; } = string.Empty;

        public string FatherName { get; set; } = string.Empty;

        public string MotherName { get; set; } = string.Empty;

        public string NID { get; set; } = string.Empty;

        [Required]
        public string MobileNo { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string DateOfBirth { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string SpouseName { get; set; } = string.Empty;

        public int ChildrenCount { get; set; }

        public string AccountType { get; set; } = string.Empty;

        public string AccountNumber { get; set; } = string.Empty;

        public string PresentVillage { get; set; } = string.Empty;
        public string PresentPostOffice { get; set; } = string.Empty;
        public int PresentDivisionId { get; set; }
        public int PresentDistrictId { get; set; }
        public int PresentUpazilaId { get; set; }
        public string PresentPostalCode { get; set; } = string.Empty;

        public string PermanentVillage { get; set; } = string.Empty;
        public string PermanentPostOffice { get; set; } = string.Empty;
        public int PermanentDivisionId { get; set; }
        public int PermanentDistrictId { get; set; }
        public int PermanentUpazilaId { get; set; }
        public string PermanentPostalCode { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        [Required]
        public int SectionId { get; set; }

        public Section? Section { get; set; }

        [Required]
        public int DesignationId { get; set; }

        public Designation? Designation { get; set; }

        [Required]
        public int LineId { get; set; }

        public Line? Line { get; set; }

        [Required]
        public int ShiftId { get; set; }

        public Shift? Shift { get; set; }

        public DateTime JoiningDate { get; set; } = DateTime.Today;

        public decimal BasicSalary { get; set; }

        public decimal GrossSalary { get; set; }

        public decimal HouseRent { get; set; }
        public decimal MedicalAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal FoodAllowance { get; set; }
        public decimal SpecialAllowance { get; set; }
        public decimal AttendanceBonus { get; set; }
        public decimal ProductionBonus { get; set; }

        [MaxLength(100)]
        public string BankName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string BranchName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string RoutingNumber { get; set; } = string.Empty;

        public string PhotoBase64 { get; set; } = string.Empty;

        public string SignatureBase64 { get; set; } = string.Empty;

        public string Status { get; set; } = EmployeeStatuses.Active;

        public DateTime? SeparationDate { get; set; }

        [MaxLength(30)]
        public string? SeparationType { get; set; }

        [MaxLength(1000)]
        public string? SeparationReason { get; set; }

        [MaxLength(500)]
        public string? SeparationRemarks { get; set; }

        [MaxLength(100)]
        public string? SeparationApprovedBy { get; set; }

        public DateTime? SeparationApprovedDate { get; set; }

        public int CompanyId { get; set; }

        public Company? Company { get; set; }

        public bool OverTimeStatus { get; set; }
        public string EmployeeType { get; set; } = string.Empty;

        public int? BusinessGroupId { get; set; }
        public BusinessGroup? BusinessGroup { get; set; }

        public ICollection<Separation> Separations { get; set; } = [];
    }
}
