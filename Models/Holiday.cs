using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class Holiday
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string HolidayName { get; set; } = string.Empty;

        [Required]
        public DateTime HolidayDate { get; set; }

        public bool IsRecurring { get; set; } = false;
    }
}
