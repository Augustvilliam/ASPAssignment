using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ASPAssignment.ViewModels
{
    public class SettingsFormViewModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Role is required")]
        public string RoleId { get; set; } = null!;

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfileImage { get; set; }

        public string? ExistingProfileImagePath { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[A-Za-zÅÄÖåäö]+$", ErrorMessage = "First name may only contain letters")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[A-Za-zÅÄÖåäö]+$", ErrorMessage = "Last name may only contain letters")]
        public string LastName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [Display(Name = "Street Address")]
        public string? StreetAddress { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Postal code may only contain digits")]
        public string? PostalCode { get; set; }

        public string? City { get; set; }
    }
}
