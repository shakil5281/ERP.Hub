using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("overtimedeductions")]
    public class OvertimeDeduction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        public DateTime DeductionDate { get; set; }

        public double BaseOtHours { get; set; }

        public double DeductedHours { get; set; }

        [MaxLength(255)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
    }
}
