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

        public string Address { get; set; } = string.Empty;

        public string NID { get; set; } = string.Empty;

        [Required]
        public string MobileNo { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

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

        public bool IsActive { get; set; } = true;
    }
}