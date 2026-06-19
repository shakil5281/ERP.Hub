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

        [Required]
        public DateTime LeaveDate { get; set; }

        [MaxLength(50)]
        public string LeaveType { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";
    }
}
