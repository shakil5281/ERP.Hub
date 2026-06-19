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
        public DateTime LogDateTime { get; set; }

        [MaxLength(50)]
        public string DeviceId { get; set; } = string.Empty;
    }
}
