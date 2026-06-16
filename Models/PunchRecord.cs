using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("punchrecords")]
    public class PunchRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        public DateTime LogDateTime { get; set; }

        [Required]
        [MaxLength(20)]
        public string LogType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DeviceTerminal { get; set; } = string.Empty;

        [MaxLength(50)]
        public string VerificationMode { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DeviceId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsProcessed { get; set; } = false;
    }
}
