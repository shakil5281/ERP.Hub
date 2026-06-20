using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("dailysalaryrecords")]
    public class DailySalaryRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        public DateTime SalaryDate { get; set; }

        public decimal DailyBasic { get; set; }

        public double OtHours { get; set; }

        public decimal OtPay { get; set; }

        public decimal Allowances { get; set; }

        public decimal Deductions { get; set; }

        public decimal NetPay { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
    }
}
