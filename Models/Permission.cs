using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Module { get; set; } = string.Empty;

        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}
