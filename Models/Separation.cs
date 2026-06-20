using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("separations")]
    public class Separation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        public int? SectionId { get; set; }

        public Section? Section { get; set; }

        public int? DesignationId { get; set; }

        public Designation? Designation { get; set; }

        [Required]
        [StringLength(30)]
        public string SeparationType { get; set; } = string.Empty;

        [Required]
        public DateTime ResignDate { get; set; }

        [Required]
        public DateTime LastWorkingDay { get; set; }

        public DateTime? ExitInterviewDate { get; set; }

        [StringLength(1000)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(500)]
        public string HandoverNotes { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Initiated";

        public int ClearanceProgress { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public string ApprovedBy { get; set; } = string.Empty;

        public DateTime? ApprovedDate { get; set; }
    }
}