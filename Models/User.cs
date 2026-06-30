using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public ICollection<UserRole>? UserRoles { get; set; }
    }
}
