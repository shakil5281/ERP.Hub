using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("jobcards")]
    public class JobCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string CardId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string CardType { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [MaxLength(100)]
        public string AuthorizedBy { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
    }
}
