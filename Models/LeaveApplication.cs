using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("leaveapplications")]
    public class LeaveApplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EmployeeName { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? DesignationId { get; set; }
        public Designation? Designation { get; set; }

        /// <summary>FK to LeaveType</summary>
        public int? LeaveTypeId { get; set; }
        public LeaveType? LeaveTypeNav { get; set; }

        [MaxLength(50)]
        public string LeaveType { get; set; } = string.Empty;

        /// <summary>Leave start date (was LeaveDate)</summary>
        [Required]
        public DateTime LeaveDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TotalDays { get; set; } = 1;

        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        /// <summary>Pending | Approved | Rejected | Cancelled</summary>
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [MaxLength(100)]
        public string ApprovedBy { get; set; } = string.Empty;

        public DateTime? ApprovedDate { get; set; }

        [MaxLength(500)]
        public string RejectedReason { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
