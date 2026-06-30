using System;
using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name (English) is required")]
        [StringLength(100, ErrorMessage = "Company name (English) is too long")]
        public string CompanyNameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company name (Bengali) is required")]
        [StringLength(100, ErrorMessage = "Company name (Bengali) is too long")]
        public string CompanyNameBn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address (English) is required")]
        [StringLength(200, ErrorMessage = "Address (English) is too long")]
        public string AddressEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address (Bengali) is required")]
        [StringLength(200, ErrorMessage = "Address (Bengali) is too long")]
        public string AddressBn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Signature is required")]
        public string Signature { get; set; } = string.Empty;

        public int? BusinessGroupId { get; set; }
        public BusinessGroup? BusinessGroup { get; set; }
    }
}
