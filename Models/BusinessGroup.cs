using System;
using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class BusinessGroup
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Group name is required")]
        [StringLength(100, ErrorMessage = "Group name is too long")]
        public string GroupName { get; set; } = string.Empty;
    }
}
