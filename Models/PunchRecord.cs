using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("punchrecords")]
    public class PunchRecord
    {
        [Key]
        public int Id { get; set; }

        /// <summary>ZK CHECKINOUT.USERID</summary>
        [Required]
        public int UserPunchId { get; set; }

        /// <summary>ZK USERINFO.BADGENUMBER — links to Employee.PunchNumber</summary>
        [Required]
        public int PunchNumber { get; set; }

        [Required]
        public DateTime LogDateTime { get; set; }

        [MaxLength(50)]
        public string DeviceId { get; set; } = string.Empty;
    }
}
